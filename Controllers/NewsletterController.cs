using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portifolio.Data;
using Portifolio.Models;

namespace Portifolio.Controllers;

public class NewsletterController(PortifolioDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var query = db.NewsletterPosts.AsNoTracking();

        if (User.Identity?.IsAuthenticated != true)
            query = query.Where(p => p.Publico);

        var posts = await query
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        return View(posts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var post = await db.NewsletterPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
            return NotFound();

        if (!post.Publico && User.Identity?.IsAuthenticated != true)
            return NotFound();

        return View(post);
    }
}
