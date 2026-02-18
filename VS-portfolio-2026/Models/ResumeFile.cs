using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class ResumeFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("filename")]
        public string FileName { get; set; } = null!;

        [BsonElement("contentType")]
        public string ContentType { get; set; } = "application/pdf";

        [BsonElement("content")]
        public byte[] Content { get; set; } = null!;
    }
}
