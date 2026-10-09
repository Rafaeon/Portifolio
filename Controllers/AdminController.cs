using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portifolio.Data;
using Portifolio.Models;

namespace Portifolio.Controllers;

[Authorize]
public class AdminController(PortifolioDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var posts = await db.NewsletterPosts
            .AsNoTracking()
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        return View(posts);
    }

    [HttpGet]
    public IActionResult Create() => View(new NewsletterPostInput());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsletterPostInput input)
    {
        if (!ModelState.IsValid)
            return View(input);

        var post = new NewsletterPost
        {
            Titulo = input.Titulo.Trim(),
            Conteudo = input.Conteudo.Trim(),
            Resumo = string.IsNullOrWhiteSpace(input.Resumo) ? null : input.Resumo.Trim(),
            Publico = input.Publico,
            CriadoEm = DateTime.UtcNow
        };

        db.NewsletterPosts.Add(post);
        await db.SaveChangesAsync();

        TempData["Sucesso"] = "Post criado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await db.NewsletterPosts.FindAsync(id);
        if (post is null)
            return NotFound();

        var input = new NewsletterPostInput
        {
            Titulo = post.Titulo,
            Conteudo = post.Conteudo,
            Resumo = post.Resumo,
            Publico = post.Publico
        };

        ViewBag.Id = post.Id;
        return View(input);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NewsletterPostInput input)
    {
        var post = await db.NewsletterPosts.FindAsync(id);
        if (post is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Id = id;
            return View(input);
        }

        post.Titulo = input.Titulo.Trim();
        post.Conteudo = input.Conteudo.Trim();
        post.Resumo = string.IsNullOrWhiteSpace(input.Resumo) ? null : input.Resumo.Trim();
        post.Publico = input.Publico;
        post.AtualizadoEm = DateTime.UtcNow;

        await db.SaveChangesAsync();

        TempData["Sucesso"] = "Post atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await db.NewsletterPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
            return NotFound();

        return View(post);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await db.NewsletterPosts.FindAsync(id);
        if (post is null)
            return NotFound();

        db.NewsletterPosts.Remove(post);
        await db.SaveChangesAsync();

        TempData["Sucesso"] = "Post excluído.";
        return RedirectToAction(nameof(Index));
    }
}
