using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class Education
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("year")]
        public string Year { get; set; } = null!;

        [BsonElement("course")]
        public string Course { get; set; } = null!;

        [BsonElement("college")]
        public string College { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("displayOrder")]
        public int DisplayOrder { get; set; }
    }
}
