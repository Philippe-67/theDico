using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    public class Word
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? SourceLanguage { get; set; } // ex: "fr"
    public string? TargetLanguage { get; set; } // ex: "en"
    public string? SourceWord { get; set; }
    public string? TargetWord { get; set; }
    public string? GrammaticalClass { get; set; } // nom, verbe, adjectif, etc.
    public string? SemanticFamily { get; set; } // maison, objet, etc.
    public DateTime AddedAt { get; set; }
    }
}
