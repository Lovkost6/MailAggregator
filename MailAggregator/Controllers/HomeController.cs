using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MailAggregator.Models;
using MailAggregator.Service;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Mvc.Rendering;
using StackExchange.Redis;
using MailService = MailAggregator.Service.MailService;

namespace MailAggregator.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MailService _mailService;
    private readonly ServerService _serverService;
    private readonly IDatabase _redis;

    public HomeController(ILogger<HomeController> logger, MailService mailService, ServerService serverService,
        IConnectionMultiplexer muxer)
    {
        _logger = logger;
        _mailService = mailService;
        _serverService = serverService;
        _redis = muxer.GetDatabase();
    }


    public async Task<IActionResult> Index(MailViewModel mailViewModel)
    {
        mailViewModel.Mails = await _mailService.GetAsync();
        if (mailViewModel.SelectedEmail == null || mailViewModel.Server == null)
        {
            return View(mailViewModel);
        }

        try
        {
            var key =
                    mailViewModel.SelectedEmail +
                    mailViewModel.Server +
                    mailViewModel.CurrentPage +
                    mailViewModel.FolderName
                ;
            if (_redis.KeyExists(key))
            {
                var value = JsonSerializer.Deserialize<CacheMailValue>(
                    (await _redis.StringGetAsync(key)));
                mailViewModel.IncomingMails = value.IncomingMails;
                mailViewModel.PagesCount = value.PagesCount;
                return View(mailViewModel);
            }

            mailViewModel.IncomingMails = await GetIncomingMail(mailViewModel, key)!;
        }
        catch (Exception e)
        {
            return View((object)e.Message);
        }

        return View(mailViewModel);
    }


    private async Task<List<IncomingMail>>? GetIncomingMail(MailViewModel mailViewModel, string key)
    {
        var host = (await _serverService.GetAsync())
            .FirstOrDefault(x => x.Name == mailViewModel.Server && x.Type == "inbox");

        var email = mailViewModel.Mails.FirstOrDefault(x =>
            x.Server == mailViewModel.Server && x.Email == mailViewModel.SelectedEmail);

        var inboxKey = host.Server + host.Port + email.Email + mailViewModel.FolderName;
        using (var client = new ImapClient())
        {
            await client.ConnectAsync(host.Server, host.Port, true);
            await client.AuthenticateAsync(mailViewModel.SelectedEmail, email.Password);


            // The Inbox folder is always available on all IMAP servers...
            var inbox = await client.GetFolderAsync(mailViewModel.FolderName);
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            int inboxCount;

            if (_redis.KeyExists(inboxKey))
            {
                inboxCount = Convert.ToInt32(await _redis.StringGetAsync(inboxKey));
            }
            else
            {
                inboxCount = inbox.Count;
                _redis.StringSetAsync(inboxKey, inboxCount);
            }
            
            var pages = Math.Ceiling(inboxCount/ (double)mailViewModel.PageSize);
            mailViewModel.PagesCount = (int)pages;


            var incomingMail = new List<IncomingMail>();

            var i = (inboxCount - 1) - ((mailViewModel.CurrentPage - 1) * mailViewModel.PageSize);
            var border = i > mailViewModel.PageSize ? i - mailViewModel.PageSize : 0;
            for (; i > border; i--)
            {
                var message = await inbox.GetMessageAsync(i);
                incomingMail.Add(new IncomingMail
                {
                    Body = message.HtmlBody, From = message.From.ToString(), GetAt = message.Date.ToString()[..16],
                    Subject = message.Subject
                });
            }

            //Не забыть как-то реализовать Body @Html.Raw(item.Body)
            await client.DisconnectAsync(true);
            var value = new CacheMailValue { IncomingMails = incomingMail, PagesCount = mailViewModel.PagesCount };
            var jsonValue = JsonSerializer.Serialize(value);
            _redis.StringSetAsync(key, jsonValue);

            return incomingMail;
        }
    }
}