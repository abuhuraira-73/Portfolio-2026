using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class BlogPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("linkedInEmbedUrl")]
        public string LinkedInEmbedUrl { get; set; } = null!;

        [BsonElement("postDate")]
        public DateTime PostDate { get; set; }

        [BsonElement("displayOrder")]
        public int DisplayOrder { get; set; }
    }
}
