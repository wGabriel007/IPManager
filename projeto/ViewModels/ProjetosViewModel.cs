using System.ComponentModel.DataAnnotations;

namespace projeto.ViewModels;

public class ProjetosViewModel
{
    // Listagem
    public List<Projeto> ListaProjetos { get; set; } = new();
    public string? ErroMensagem       { get; set; }

    // Filtros
    public string? FiltroNome   { get; set; }
    public string? FiltroIp     { get; set; }
    public string? FiltroTipoIp { get; set; }

    // Campos para novo projeto
    [Required(ErrorMessage = "Informe o nome")]
    public string NovoNome   { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe o IP")]
    public string NovoIp     { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe o tipo de IP")]
    public string NovoTipoIp { get; set; } = string.Empty;

    // Campos para editar projeto
    public int    EditarId    { get; set; }
    [Required(ErrorMessage = "Informe o nome")]
    public string EditarNome   { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe o IP")]
    public string EditarIp     { get; set; } = string.Empty;
    [Required(ErrorMessage = "Informe o tipo de IP")]
    public string EditarTipoIp { get; set; } = string.Empty;
}
