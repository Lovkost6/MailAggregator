using MailAggregator.Models;
using MailAggregator.Service;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MongoDB.Driver.Linq;

namespace MailAggregator.Controllers;

public class MessageController : Controller
{
    private readonly MailService _mailService;
    private readonly ServerService _serverService;


    public MessageController(MailService mailService, ServerService serverService)
    {
        _mailService = mailService;
        _serverService = serverService;
    }

    public async Task<ActionResult> Index()
    {
        var _fromEmails = (await _mailService.GetAsync()).Select(x => x.Email + " " + x.Server).ToList();
        
        var model = new MessageViewModel
        {
            FromEmails = _fromEmails
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Index(MessageViewModel model)
    {
        var fromSelected = model.SelectedFromEmail.Split(" ");
        var selectedMail = (await _mailService.GetAsync())
            .FirstOrDefault(x => x.Email == fromSelected[0]);
        var server = (await _serverService.GetAsync()).FirstOrDefault(x => x.Type == "send" && x.Name == fromSelected[1]);
        var message = new MimeMessage();
        var from =  selectedMail.Email.Contains("@") ? selectedMail.Email : selectedMail.Email + selectedMail.Server;
        message.From.Add(new MailboxAddress("", from));
        message.To.Add(new MailboxAddress("", model.ToEmail));
        message.Subject = model.Subject;

        message.Body = new TextPart("plain")
        {
            Text = model.Body
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(server.Server, server.Port, true);
            Console.WriteLine(client.IsConnected);
            // Note: only needed if the SMTP server requires authentication
            await client.AuthenticateAsync(selectedMail.Email, selectedMail.Password);
            Console.WriteLine(client.IsAuthenticated);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            return RedirectToAction("Index", "Home");
        }
    }
}

