using System.ComponentModel.DataAnnotations;

namespace Portifolio.Models;

public class AdminUser
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    public string SenhaSalt { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}

public class AlterarCredenciaisInput
{
    [Required(ErrorMessage = "Informe a senha atual.")]
    [DataType(DataType.Password)]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o novo usuário.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O usuário deve ter entre 3 e 100 caracteres.")]
    public string NovoUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A nova senha deve ter pelo menos 8 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NovaSenha), ErrorMessage = "A confirmação não confere com a nova senha.")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
