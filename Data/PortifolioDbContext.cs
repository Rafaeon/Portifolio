using Microsoft.EntityFrameworkCore;
using Portifolio.Models;

namespace Portifolio.Data;

public class PortifolioDbContext(DbContextOptions<PortifolioDbContext> options) : DbContext(options)
{
    public DbSet<NewsletterPost> NewsletterPosts => Set<NewsletterPost>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NewsletterPost>(e =>
        {
            e.ToTable("NewsletterPosts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            e.Property(x => x.Resumo).HasMaxLength(300);
            e.Property(x => x.Conteudo).IsRequired();
            e.Property(x => x.Publico).HasDefaultValue(true);
            e.Property(x => x.CriadoEm).HasDefaultValueSql("GETUTCDATE()");
            e.HasIndex(x => x.Publico);
            e.HasIndex(x => x.CriadoEm);
        });

        modelBuilder.Entity<AdminUser>(e =>
        {
            e.ToTable("AdminUsers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Usuario).HasMaxLength(100).IsRequired();
            e.Property(x => x.SenhaSalt).IsRequired();
            e.Property(x => x.SenhaHash).IsRequired();
            e.Property(x => x.AtualizadoEm).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
