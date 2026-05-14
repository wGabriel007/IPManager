using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using projeto.Data;

namespace projeto.Pages.Contas;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;

    public LoginModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Informe a senha")]
    public string Senha { get; set; } = string.Empty;

    public string? ErroMensagem { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var usuario = _db.Usuarios
            .FirstOrDefault(u => u.str_Email == Email);

        if (usuario is null)
        {
            ErroMensagem = "E-mail não encontrado.";
            return Page();
        }

        bool senhaValida;
        try
        {
            senhaValida = BCrypt.Net.BCrypt.Verify(Senha, usuario.str_Senha);
        }
        catch
        {
            senhaValida = usuario.str_Senha == Senha;
            if (senhaValida)
            {
                usuario.str_Senha = BCrypt.Net.BCrypt.HashPassword(Senha);
                await _db.SaveChangesAsync();
            }
        }

        if (!senhaValida)
        {
            ErroMensagem = "Senha incorreta.";
            return Page();
        }

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
