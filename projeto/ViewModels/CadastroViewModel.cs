using System.ComponentModel.DataAnnotations;

namespace projeto.ViewModels;

public class CadastroViewModel
{
    [Required(ErrorMessage = "Informe o nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha")]
    [MinLength(6, ErrorMessage = "A senha deve ter ao menos 6 caracteres")]
    public string Senha { get; set; } = string.Empty;
}
