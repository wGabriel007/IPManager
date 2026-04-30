using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projeto.Models;

[Table("projetos")]
public class Projeto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("ip")]
    public string Ip { get; set; } = string.Empty;

    [Required]
    [Column("tipo_ip")]
    public string TipoIp { get; set; } = string.Empty;

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }
}
