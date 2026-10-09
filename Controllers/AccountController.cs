using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portifolio.Data;
using Portifolio.Models;
using Portifolio.Services;

namespace Portifolio.Controllers;

public class AccountController(
    PortifolioDbContext db,
    IConfiguration configuration,
    EmailService emailService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Admin");

        ViewData["ReturnUrl"] = returnUrl;
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string usuario, string senha, string? returnUrl = null)
    {
        var admin = await db.AdminUsers.AsNoTracking().FirstOrDefaultAsync();
        if (admin is null)
        {
            ModelState.AddModelError(string.Empty, "Nenhum administrador configurado.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var valid =
            string.Equals(usuario, admin.Usuario, StringComparison.Ordinal) &&
            PasswordHasher.Verify(senha, admin.SenhaSalt, admin.SenhaHash);

        if (!valid)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Usuario),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Admin");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AlterarCredenciais()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction(nameof(Login));

        var atual = User.Identity?.Name ?? string.Empty;
        return View(new AlterarCredenciaisInput { NovoUsuario = atual });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarCredenciais(AlterarCredenciaisInput input)
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction(nameof(Login));

        var admin = await db.AdminUsers.FirstOrDefaultAsync();
        if (admin is null)
            return RedirectToAction(nameof(Login));

        if (!PasswordHasher.Verify(input.SenhaAtual, admin.SenhaSalt, admin.SenhaHash))
        {
            ModelState.AddModelError(nameof(input.SenhaAtual), "Senha atual incorreta.");
            return View(input);
        }

        if (!string.Equals(input.NovoUsuario, admin.Usuario, StringComparison.Ordinal) &&
            await db.AdminUsers.AnyAsync(a => a.Usuario == input.NovoUsuario))
        {
            ModelState.AddModelError(nameof(input.NovoUsuario), "Já existe um usuário com esse nome.");
            return View(input);
        }

        var (salt, hash) = PasswordHasher.Hash(input.NovaSenha);
        var usuarioAntigo = admin.Usuario;

        admin.Usuario = input.NovoUsuario.Trim();
        admin.SenhaSalt = salt;
        admin.SenhaHash = hash;
        admin.AtualizadoEm = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Usuario),
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            });

        var avisoPara = configuration["Email:AvisarPara"];
        var emailEnviado = false;
        if (!string.IsNullOrWhiteSpace(avisoPara))
        {
            emailEnviado = emailService.TryEnviar(
                avisoPara!,
                "Portifolio — credenciais atualizadas",
                $"""
                 <p>As credenciais do painel da newsletter foram alteradas.</p>
                 <ul>
                   <li><b>Usuário antigo:</b> {usuarioAntigo}</li>
                   <li><b>Usuário novo:</b> {admin.Usuario}</li>
                   <li><b>Quando (UTC):</b> {admin.AtualizadoEm:yyyy-MM-dd HH:mm:ss}</li>
                 </ul>
                 <p>Se não foi você, revise o acesso ao servidor imediatamente.</p>
                 """);
        }

        TempData["Sucesso"] = emailEnviado
            ? "Credenciais alteradas. Enviamos um e-mail de confirmação."
            : "Credenciais alteradas com sucesso.";

        return RedirectToAction("Index", "Admin");
    }
}
