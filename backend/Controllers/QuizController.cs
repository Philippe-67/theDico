using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Word> _words;
        public QuizController(IMongoClient mongoClient, IConfiguration config)
        {
            var database = mongoClient.GetDatabase(config["MongoDb:Database"] ?? "DicoDb");
            _quizzes = database.GetCollection<Quiz>("Quizzes");
            _words = database.GetCollection<Word>("Words");
        }

        // Génère un quiz à partir des mots de l'utilisateur
        [HttpPost("generate")]
        public IActionResult Generate([FromBody] QuizGenerateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var words = _words.Find(w => w.UserId == userId && w.SourceLanguage == dto.SourceLanguage && w.TargetLanguage == dto.TargetLanguage).ToList();
            if (words.Count < dto.QuestionCount)
                return BadRequest("Pas assez de mots pour générer le quiz.");

            var selectedWords = words.OrderBy(_ => Guid.NewGuid()).Take(dto.QuestionCount).ToList();
            var questions = selectedWords.Select(w => new QuizQuestion
            {
                WordId = w.Id,
                SourceWord = w.SourceWord,
                TargetWord = w.TargetWord,
                GrammaticalClass = w.GrammaticalClass,
                SemanticFamily = w.SemanticFamily
            }).ToList();

            var quiz = new Quiz
            {
                UserId = userId,
                LanguagePair = dto.SourceLanguage + "-" + dto.TargetLanguage,
                CreatedAt = DateTime.UtcNow,
                Questions = questions,
                MaxScore = questions.Count,
                Score = 0
            };
            _quizzes.InsertOne(quiz);
            return Ok(quiz);
        }

        // Soumet les réponses d'un quiz
        [HttpPost("submit/{id}")]
        public IActionResult Submit(string id, [FromBody] List<QuizAnswerDto> answers)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quiz = _quizzes.Find(q => q.Id == id && q.UserId == userId).FirstOrDefault();
            if (quiz == null)
                return NotFound();

            int score = 0;
            foreach (var answer in answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.WordId == answer.WordId);
                if (question != null)
                {
                    question.UserAnswer = answer.UserAnswer;
                    question.IsCorrect = string.Equals(question.TargetWord, answer.UserAnswer, StringComparison.OrdinalIgnoreCase);
                    if (question.IsCorrect) score++;
                }
            }
            quiz.Score = score;
            _quizzes.ReplaceOne(q => q.Id == id, quiz);
            return Ok(new { score, maxScore = quiz.MaxScore });
        }

        // Récupère les quiz de l'utilisateur
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quizzes = _quizzes.Find(q => q.UserId == userId).ToList();
            return Ok(quizzes);
        }
    }

    public class QuizGenerateDto
    {
        public string? SourceLanguage { get; set; }
        public string? TargetLanguage { get; set; }
        public int QuestionCount { get; set; }
    }

    public class QuizAnswerDto
    {
        public string? WordId { get; set; }
        public string? UserAnswer { get; set; }
    }
}
