using System.ComponentModel.DataAnnotations;

namespace Portifolio.Models;

public class NewsletterPost
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o título.")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o conteúdo.")]
    public string Conteudo { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Resumo { get; set; }

    public bool Publico { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? AtualizadoEm { get; set; }
}

public class NewsletterPostInput
{
    [Required(ErrorMessage = "Informe o título.")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o conteúdo.")]
    public string Conteudo { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Resumo { get; set; }

    public bool Publico { get; set; } = true;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o usuário.")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;
}
