using MailAggregator.Models;
using MailAggregator.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace MailAggregator.Controllers;

public class LoginController : Controller
{
    private readonly UserService _userService;

    public LoginController(UserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse")
        });
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        {
            claim.Type,
            claim.Value
        });
        var newUser = new User();
        foreach (var item in claims)
        {
            if (item.Type.Split("/").LastOrDefault() == "name")
            {
                newUser.Name = item.Value;
            }

            if (item.Type.Split("/").LastOrDefault() == "emailaddres")
            {
                newUser.Email = item.Value;
            }
        }

        var findUser = await _userService.GetByEmailAsync(newUser.Email);
        
        if (findUser == null)
        {
            await _userService.CreateAsync(newUser);
        }
        return RedirectToAction("Index","Home");
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index","Home");
    }
}