using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VS_portfolio_2026.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [BsonElement("email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = null!;

        [BsonElement("subject")]
        public string? Subject { get; set; } // Optional

        [BsonElement("message")]
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; } = null!;

        [BsonElement("submittedAt")]
        public DateTime SubmittedAt { get; set; }
    }
}
