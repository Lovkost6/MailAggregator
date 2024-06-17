using System.Text.Json;
using MailAggregator.Models;
using Microsoft.AspNetCore.Mvc;

namespace MailAggregator.Controllers;

public class OpenBodyController:Controller
{
    public IActionResult Index()
    {

        var messageJson = HttpContext.Session.GetString("MessageBody");
        var message = JsonSerializer.Deserialize<OpenBodyViewModel>(messageJson);
        return View(message);
    }
}