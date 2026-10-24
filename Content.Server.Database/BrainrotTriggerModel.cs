using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database;

[Table("brainrot_triggers")]
[Index(nameof(Trigger), IsUnique = true)]
public sealed class BrainrotTrigger
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("trigger")]
    public string Trigger { get; set; } = string.Empty;
}
