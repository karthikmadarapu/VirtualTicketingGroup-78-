using Microsoft.AspNetCore.Mvc;
using VirtualTicketing.Data;

namespace VirtualTicketing.Controllers;

public class DbCheckController(ApplicationDbContext db) : Controller
{
    [HttpGet("/dbcheck")]
    public IActionResult Index()
    {
        var cats = db.Categories.Count();
        var evts = db.Events.Count();
        return Content($"DB OK — Categories: {cats}, Events: {evts}");
    }
}