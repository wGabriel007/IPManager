using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("usuarios")]
public class Usuario
{
    [Key]
    [Column("id")]
    public int int_Id { get; set; }

    [Required]
    [Column("nome")]
    public string str_Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Column("email")]
    public string str_Email { get; set; } = string.Empty;

    [Required]
    [Column("senha")]
    public string str_Senha { get; set; } = string.Empty;
}