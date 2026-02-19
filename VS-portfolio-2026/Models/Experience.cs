using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class Experience
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("year")]
        public string Year { get; set; } = null!;

        [BsonElement("role")]
        public string Role { get; set; } = null!;

        [BsonElement("company")]
        public string Company { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("displayOrder")]
        public int DisplayOrder { get; set; }
    }
}
