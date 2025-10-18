using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<string> LanguagePairs { get; set; } // ex: ["fr-en", "fr-es"]
        public UserStats Stats { get; set; }
    }

    public class UserStats
    {
        public int WordCount { get; set; }
        public int QuizCount { get; set; }
        public int TestCount { get; set; }
        public List<Progression> Progressions { get; set; }
    }

    public class Progression
    {
        public string LanguagePair { get; set; }
        public int SuccessRate { get; set; }
        public DateTime Date { get; set; }
    }
}
