using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models;

namespace Portifolio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult CRM()
    {
        return View();
    }

    public IActionResult HomeLab()
    {
        return View();
    }

    public IActionResult Contato()
    {
        return View();
    }

    public IActionResult SobreMim()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
