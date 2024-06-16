namespace MailAggregator.Models;

public class MessageViewModel
{
    public List<string>? FromEmails { get; set; }
    public string? SelectedFromEmail { get; set; }
    public string? ToEmail { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}