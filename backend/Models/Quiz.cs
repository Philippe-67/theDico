using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    public class Quiz
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? LanguagePair { get; set; } // ex: "fr-en"
        public DateTime CreatedAt { get; set; }
        public List<QuizQuestion> Questions { get; set; } = new();
        public int Score { get; set; }
        public int MaxScore { get; set; }
    }

    public class QuizQuestion
    {
        public string? WordId { get; set; }
        public string? SourceWord { get; set; }
        public string? TargetWord { get; set; }
        public string? UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string? GrammaticalClass { get; set; }
        public string? SemanticFamily { get; set; }
    }
}
