using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MailAggregator.Models;

public class EmailServerViewModel
{
    
    public SelectList? ServerName { get; set; }
    public string? Server { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}