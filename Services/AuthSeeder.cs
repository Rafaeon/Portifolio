using Microsoft.EntityFrameworkCore;
using Portifolio.Data;
using Portifolio.Models;

namespace Portifolio.Services;

public static class AuthSeeder
{
    public static async Task EnsureAdminAsync(IServiceProvider services, IConfiguration configuration)
    {
        var db = services.GetRequiredService<PortifolioDbContext>();

        if (await db.AdminUsers.AnyAsync())
            return;

        var usuario = configuration["Auth:Usuario"];
        var salt = configuration["Auth:Salt"];
        var hash = configuration["Auth:Hash"];

        if (string.IsNullOrWhiteSpace(usuario) ||
            string.IsNullOrWhiteSpace(salt) ||
            string.IsNullOrWhiteSpace(hash))
        {
            var (novaSalt, novaHash) = PasswordHasher.Hash("Portifolio#2026");
            usuario = string.IsNullOrWhiteSpace(usuario) ? "rafael" : usuario;
            salt = novaSalt;
            hash = novaHash;
        }

        db.AdminUsers.Add(new AdminUser
        {
            Usuario = usuario!,
            SenhaSalt = salt!,
            SenhaHash = hash!,
            AtualizadoEm = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
}
