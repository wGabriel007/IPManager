using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projeto.Data;
using projeto.ViewModels;

namespace projeto.Controllers;

[Authorize]
public class ProjetosController : Controller
{
    private readonly AppDbContext _db;

    public ProjetosController(AppDbContext db)
    {
        _db = db;
    }

    private int UsuarioLogadoId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /Projetos
    public async Task<IActionResult> Index(string? filtroNome, string? filtroIp, string? filtroTipoIp)
    {
        var query = _db.Projetos.Include(p => p.Usuario).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNome))
            query = query.Where(p => p.str_Nome.ToLower().Contains(filtroNome.ToLower()));

        if (!string.IsNullOrWhiteSpace(filtroIp))
            query = query.Where(p => p.str_Ip.Contains(filtroIp));

        if (!string.IsNullOrWhiteSpace(filtroTipoIp))
            query = query.Where(p => p.str_TipoIp == filtroTipoIp);

        var vm = new ProjetosViewModel
        {
            ListaProjetos = await query.OrderBy(p => p.str_Nome).ToListAsync(),
            FiltroNome    = filtroNome,
            FiltroIp      = filtroIp,
            FiltroTipoIp  = filtroTipoIp
        };

        return View(vm);
    }

    // POST /Projetos/Cadastrar
    [HttpPost]
    public async Task<IActionResult> Cadastrar(ProjetosViewModel vm)
    {
        ModelState.Remove(nameof(vm.EditarId));
        ModelState.Remove(nameof(vm.EditarNome));
        ModelState.Remove(nameof(vm.EditarIp));
        ModelState.Remove(nameof(vm.EditarTipoIp));
        ModelState.Remove(nameof(vm.EditarAmbiente));
        ModelState.Remove(nameof(vm.ListaProjetos));

        if (!ModelState.IsValid)
        {
            vm.ListaProjetos = await _db.Projetos.Include(p => p.Usuario)
                                        .OrderBy(p => p.str_Nome).ToListAsync();
            vm.ErroMensagem  = string.Join(" | ", ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .Select(x => $"{x.Key}: {x.Value!.Errors[0].ErrorMessage}"));
            return View("Index", vm);
        }

        _db.Projetos.Add(new Projeto
        {
            str_Nome      = vm.NovoNome,
            str_Ip        = vm.NovoIp,
            str_TipoIp    = vm.NovoTipoIp,
            str_Ambiente  = vm.Ambiente,
            bool_VPN      = vm.NovoVPN,
            int_UsuarioId = UsuarioLogadoId
        });

        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // POST /Projetos/Editar
    [HttpPost]
    public async Task<IActionResult> Editar(ProjetosViewModel vm)
    {
        ModelState.Remove(nameof(vm.NovoNome));
        ModelState.Remove(nameof(vm.NovoIp));
        ModelState.Remove(nameof(vm.NovoTipoIp));
        ModelState.Remove(nameof(vm.Ambiente));
        ModelState.Remove(nameof(vm.ListaProjetos));

        if (!ModelState.IsValid)
        {
            vm.ListaProjetos = await _db.Projetos.Include(p => p.Usuario)
                                        .OrderBy(p => p.str_Nome).ToListAsync();
            return View("Index", vm);
        }

        var projeto = await _db.Projetos.FindAsync(vm.EditarId);

        if (projeto is null || projeto.int_UsuarioId != UsuarioLogadoId)
            return Forbid();

        projeto.str_Nome     = vm.EditarNome;
        projeto.str_Ip       = vm.EditarIp;
        projeto.str_TipoIp   = vm.EditarTipoIp;
        projeto.str_Ambiente = vm.EditarAmbiente;
        projeto.bool_VPN     = vm.EditarVPN;

        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // POST /Projetos/Excluir/5
    [HttpPost]
    public async Task<IActionResult> Excluir(int id)
    {
        var projeto = await _db.Projetos.FindAsync(id);

        if (projeto is null || projeto.int_UsuarioId != UsuarioLogadoId)
            return Forbid();

        _db.Projetos.Remove(projeto);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
