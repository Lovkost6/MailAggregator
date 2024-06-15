using Microsoft.AspNetCore.Mvc.Rendering;

namespace MailAggregator.Models;

public class MailViewModel
{
    public List<Mail>? Mails { get; set; }
    
    public List<IncomingMail>? IncomingMails { get; set; }
    public string? SelectedEmail { get; set; }
    public string? Server { get; set; }

    public string? FolderName { get; set; } = "INBOX";

    public int CurrentPage { get; set; } = 1;
    public int PagesCount { get; set; }
    public int PageSize { get; set; } = 10;
}