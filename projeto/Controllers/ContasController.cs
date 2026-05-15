using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using projeto.Data;
using projeto.ViewModels;

namespace projeto.Controllers;

public class ContasController : Controller
{
    private readonly AppDbContext _db;

    public ContasController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Contas/Login
    [HttpGet]
    public IActionResult Login() => View();

    // POST /Contas/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var usuario = _db.Usuarios.FirstOrDefault(u => u.str_Email == vm.Email);

        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "E-mail não encontrado.");
            return View(vm);
        }

        bool senhaValida;
        try
        {
            senhaValida = BCrypt.Net.BCrypt.Verify(vm.Senha, usuario.str_Senha);
        }
        catch
        {
            senhaValida = usuario.str_Senha == vm.Senha;
            if (senhaValida)
            {
                usuario.str_Senha = BCrypt.Net.BCrypt.HashPassword(vm.Senha);
                await _db.SaveChangesAsync();
            }
        }

        if (!senhaValida)
        {
            ModelState.AddModelError(string.Empty, "Senha incorreta.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.int_Id.ToString()),
            new(ClaimTypes.Name, usuario.str_Nome),
            new(ClaimTypes.Email, usuario.str_Email),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Projetos");
    }

    // GET /Contas/Cadastro
    [HttpGet]
    public IActionResult Cadastro() => View();

    // POST /Contas/Cadastro
    [HttpPost]
    public async Task<IActionResult> Cadastro(CadastroViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (_db.Usuarios.Any(u => u.str_Email == vm.Email))
        {
            ModelState.AddModelError(string.Empty, "Este e-mail já está cadastrado.");
            return View(vm);
        }

        var usuario = new Usuario
        {
            str_Nome  = vm.Nome,
            str_Email = vm.Email,
            str_Senha = BCrypt.Net.BCrypt.HashPassword(vm.Senha)
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

        return RedirectToAction("Index", "Projetos");
    }

    // POST /Contas/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
