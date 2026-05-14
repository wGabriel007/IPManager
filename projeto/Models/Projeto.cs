using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("projetos")]
public class Projeto
{
    [Key]
    [Column("id")]
    public int int_Id { get; set; }

    [Required]
    [Column("nome")]
    public string str_Nome { get; set; } = string.Empty;

    [Required]
    [Column("ip")]
    public string str_Ip { get; set; } = string.Empty;

    [Required]
    [Column("tipo_ip")]
    public string str_TipoIp { get; set; } = string.Empty;

    [Column("usuario_id")]
    public int int_UsuarioId { get; set; }

    [ForeignKey(nameof(int_UsuarioId))]
    public Usuario? Usuario { get; set; }
}