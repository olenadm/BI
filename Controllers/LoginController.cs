using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BIMagistral.Helpers;
using BIMagistral.Services;
using BIMagistral.Models;

namespace BIMagistral.Controllers;

public class LoginController : Controller
{
    private readonly ApiProxy _apiProxy;

    public LoginController(ApiProxy apiProxy)
    {
        _apiProxy = apiProxy;
    }

    public IActionResult Index()
    {
        HttpContext.Session.Clear();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Auth(string email, string password)
    {
        var result = await _apiProxy.Login(new Credenciais(email, password));
        if (result is not null)
        {
            HttpContext.Session.SetString("USR_DATA", JsonSerializer.Serialize(result));
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewData["ErrorMsg"] = "Login/senha inválidos";
            return View("Index");
        }
    }
}