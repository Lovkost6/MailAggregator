using MailAggregator.Models;
using MailAggregator.Service;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MailService = MailAggregator.Service.MailService;

namespace MailAggregator.Controllers;

public class ConnectionController : Controller
{
    private readonly ServerService _serverService;
    private readonly MailService _mailService;

    public ConnectionController(ServerService serverService, MailService mailService)
    {
        _serverService = serverService;
        _mailService = mailService;
    }

    public async Task<ActionResult> CreateEmailConnection(string server, string email, string password)
    {
        var host = await _serverService.GetServerAsync(server);
        using var client = new ImapClient ();
        try
        {
            await client.ConnectAsync(host.Server, host.Port, true);
            Console.WriteLine(client.IsConnected);
            await client.AuthenticateAsync (email, password);
            Console.WriteLine(client.IsAuthenticated);
        }
        catch (Exception e)
        {
            return View( (object)e.Message);
        }

        await _mailService.CreateAsync(new Mail{Email = email, Password=password,Server = server});

        await client.DisconnectAsync (true);
        object str = "Connection to " + email + server + " success";
        return View(str);
    }

    public async Task<ActionResult> Index()
    {
        var servers = new EmailServerViewModel
        {
           ServerName = new SelectList((await _serverService.GetAsync()).Where(x=> x.Type == "inbox").Select(x=>x.Name))
        };
            
        return View(servers);
    }
}