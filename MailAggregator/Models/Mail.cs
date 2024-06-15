using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MailAggregator.Models;

public class Mail
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    
    public required string Server { get; set; }
}