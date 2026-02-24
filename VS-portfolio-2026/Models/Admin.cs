using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class Admin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonElement("Username")]
        public string Username { get; set; } = default!;

        [BsonElement("Password")]
        public string Password { get; set; } = default!;
    }
}
