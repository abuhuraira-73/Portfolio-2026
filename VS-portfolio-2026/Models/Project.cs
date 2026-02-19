using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = null!;

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [BsonElement("displayOrder")]
        public int DisplayOrder { get; set; }
    }
}
