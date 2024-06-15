using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MailAggregator.Models;

public class EmailServer
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Server { get; set; }
    public string Type { get; set; }
    public int Port { get; set; }
}