using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projeto.Data;

namespace projeto.Pages.Contas;

public class CadastroModel : PageModel
{
    private readonly AppDbContext _db;

    public CadastroModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    [Required(ErrorMessage = "Informe o nome")]
    public string Nome { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Informe a senha")]
    [MinLength(6, ErrorMessage = "A senha deve ter ao menos 6 caracteres")]
    public string Senha { get; set; } = string.Empty;

    public string? ErroMensagem { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (_db.Usuarios.Any(u => u.str_Email == Email))
        {
            ErroMensagem = "Este e-mail já está cadastrado.";
            return Page();
        }

        var usuario = new Usuario
        {
            str_Nome = Nome,
            str_Email = Email,
            str_Senha = BCrypt.Net.BCrypt.HashPassword(Senha)
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.int_Id.ToString()),
            new(ClaimTypes.Name, usuario.str_Nome),
            new(ClaimTypes.Email, usuario.str_Email),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToPage("/Projetos/Projetos");
    }
}
