using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using projeto.Data;
using projeto.Models;

namespace projeto.Pages.Projetos
{
    [Authorize]
    public class ProjetosModel : PageModel
    {
        private readonly AppDbContext _db;

        public ProjetosModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Projeto> ListaProjetos { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public string? FiltroNome { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FiltroIp {  get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FiltroTipoIp { get; set; }

        // Campos do formulário de cadastro
        [BindProperty]
        [Required(ErrorMessage = "Informe o nome do projeto")]
        public string NovoNome { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Informe o endereço IP")]
        [RegularExpression(@"^(\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "IP inválido (ex: 192.168.0.1)")]
        public string NovoIp { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Selecione o tipo de IP")]
        public string NovoTipoIp { get; set; } = string.Empty;

        // Campos do formulário de editar
        [BindProperty]
        public int EditarId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Informe o nome do projeto")]
        public string EditarNome { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Informe o endereço IP")]
        [RegularExpression(@"^(\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "IP inválido (ex: 192.168.0.1)")]
        public string EditarIp { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Selecione o tipo de IP")]
        public string EditarTipoIp { get; set; } = string.Empty;

        public string? ErroMensagem { get; set; }

        private int? UsuarioLogadoId
        {
            get
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(claim, out var id) ? id : null;
            }

        }

        public async Task OnGetAsync()
        {
            var query = _db.Projetos.Include(p => p.Usuario).AsQueryable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
                query = query.Where(p => p.Nome.ToLower().Contains(FiltroNome.ToLower()));

            if (!string.IsNullOrWhiteSpace(FiltroIp))
                query = query.Where(p => p.Ip.Contains(FiltroIp));

            if (!string.IsNullOrWhiteSpace(FiltroTipoIp))
                query = query.Where(p => p.TipoIp == FiltroTipoIp);

            ListaProjetos = await query.OrderBy(p => p.Nome).ToListAsync();
        }

        // Cadastrar
        public async Task<IActionResult> OnPostCadastrarAsync()
        {
            ModelState.Remove(nameof(EditarNome));
            ModelState.Remove(nameof(EditarIp));
            ModelState.Remove(nameof(EditarTipoIp));

            if (!ModelState.IsValid)
            {
                ErroMensagem = string.Join(" | ", ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors[0].ErrorMessage}"));
                await OnGetAsync();
                return Page();
            }

            if (UsuarioLogadoId is null)
                return RedirectToPage("/Contas/Login");

            _db.Projetos.Add(new Projeto
            {
                Nome = NovoNome,
                Ip = NovoIp,
                TipoIp = NovoTipoIp,
                UsuarioId = UsuarioLogadoId.Value
            });

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        // Editar
        public async Task<IActionResult> OnPostEditarAsync()
        {
            ModelState.Remove(nameof(NovoNome));
            ModelState.Remove(nameof(NovoIp));
            ModelState.Remove(nameof(NovoTipoIp));

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var projeto = await _db.Projetos.FindAsync(EditarId);

            if (projeto is null || projeto.UsuarioId != UsuarioLogadoId)
                return Forbid();

            projeto.Nome = EditarNome;
            projeto.Ip = EditarIp;
            projeto.TipoIp = EditarTipoIp;

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        // Excluir
        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            var projeto = await _db.Projetos.FindAsync(id);

            if (projeto is null || projeto.UsuarioId != UsuarioLogadoId)
                return Forbid();

            _db.Projetos.Remove(projeto);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
