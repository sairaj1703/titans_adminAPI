using Microsoft.AspNetCore.Mvc;

namespace titans_admin.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Register() => View();

    [HttpGet]
    public IActionResult Login() => View();
}
