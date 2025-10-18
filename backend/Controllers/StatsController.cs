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
    public class StatsController : ControllerBase
    {
        private readonly IMongoCollection<Word> _words;
        private readonly IMongoCollection<Quiz> _quizzes;
        public StatsController(IMongoClient mongoClient, IConfiguration config)
        {
            var database = mongoClient.GetDatabase(config["MongoDb:Database"] ?? "DicoDb");
            _words = database.GetCollection<Word>("Words");
            _quizzes = database.GetCollection<Quiz>("Quizzes");
        }

        [HttpGet]
        public IActionResult GetStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wordCount = _words.CountDocuments(w => w.UserId == userId);
            var quizCount = _quizzes.CountDocuments(q => q.UserId == userId);
            var quizzes = _quizzes.Find(q => q.UserId == userId).ToList();
            var progression = quizzes.Select(q => new QuizProgressionDto
            {
                QuizId = q.Id,
                Date = q.CreatedAt,
                Score = q.Score,
                MaxScore = q.MaxScore,
                LanguagePair = q.LanguagePair
            }).ToList();

            var stats = new UserStatsDto
            {
                WordCount = (int)wordCount,
                QuizCount = (int)quizCount,
                Progression = progression
            };
            return Ok(stats);
        }
    }

    public class UserStatsDto
    {
        public int WordCount { get; set; }
        public int QuizCount { get; set; }
        public List<QuizProgressionDto> Progression { get; set; } = new();
    }

    public class QuizProgressionDto
    {
        public string? QuizId { get; set; }
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public string? LanguagePair { get; set; }
    }
}
