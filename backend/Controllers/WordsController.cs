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
    public class WordsController : ControllerBase
    {
        private readonly IMongoCollection<Word> _words;
        private readonly Services.WordCategorizationService _categorizer = new();
        public WordsController(IMongoClient mongoClient, IConfiguration config)
        {
            var database = mongoClient.GetDatabase(config["MongoDb:Database"] ?? "DicoDb");
            _words = database.GetCollection<Word>("Words");
        }

        // Récupère tous les mots de l'utilisateur
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var words = _words.Find(w => w.UserId == userId).ToList();
            return Ok(words);
        }

        // Ajoute un mot
        [HttpPost]
        public IActionResult Add([FromBody] WordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(dto.SourceWord) || string.IsNullOrWhiteSpace(dto.TargetWord))
                return BadRequest("Mot source et traduction requis.");

            var grammaticalClass = dto.GrammaticalClass;
            if (string.IsNullOrWhiteSpace(grammaticalClass))
                grammaticalClass = _categorizer.Categorize(dto.SourceWord ?? "");

            var word = new Word
            {
                UserId = userId,
                SourceLanguage = dto.SourceLanguage,
                TargetLanguage = dto.TargetLanguage,
                SourceWord = dto.SourceWord,
                TargetWord = dto.TargetWord,
                GrammaticalClass = grammaticalClass,
                SemanticFamily = dto.SemanticFamily,
                AddedAt = DateTime.UtcNow
            };
            _words.InsertOne(word);
            return Ok(word);
        }

        // Supprime un mot
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _words.DeleteOne(w => w.Id == id && w.UserId == userId);
            if (result.DeletedCount == 0)
                return NotFound();
            return NoContent();
        }

        // Met à jour un mot
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] WordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var update = Builders<Word>.Update
                .Set(w => w.SourceWord, dto.SourceWord)
                .Set(w => w.TargetWord, dto.TargetWord)
                .Set(w => w.GrammaticalClass, dto.GrammaticalClass)
                .Set(w => w.SemanticFamily, dto.SemanticFamily)
                .Set(w => w.SourceLanguage, dto.SourceLanguage)
                .Set(w => w.TargetLanguage, dto.TargetLanguage);
            var result = _words.UpdateOne(w => w.Id == id && w.UserId == userId, update);
            if (result.ModifiedCount == 0)
                return NotFound();
            return NoContent();
        }

        // Filtre par classe grammaticale ou famille sémantique
        [HttpGet("filter")]
        public IActionResult Filter([FromQuery] string? grammaticalClass, [FromQuery] string? semanticFamily)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var filter = Builders<Word>.Filter.Eq(w => w.UserId, userId);
            if (!string.IsNullOrWhiteSpace(grammaticalClass))
                filter &= Builders<Word>.Filter.Eq(w => w.GrammaticalClass, grammaticalClass);
            if (!string.IsNullOrWhiteSpace(semanticFamily))
                filter &= Builders<Word>.Filter.Eq(w => w.SemanticFamily, semanticFamily);
            var words = _words.Find(filter).ToList();
            return Ok(words);
        }
    }

    public class WordDto
    {
        public string? SourceLanguage { get; set; }
        public string? TargetLanguage { get; set; }
        public string? SourceWord { get; set; }
        public string? TargetWord { get; set; }
        public string? GrammaticalClass { get; set; }
        public string? SemanticFamily { get; set; }
    }
}
