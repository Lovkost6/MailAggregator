using MimeKit;

namespace MailAggregator.Models;

public class IncomingMail
{
    public string? From { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? GetAt { get; set; }
}