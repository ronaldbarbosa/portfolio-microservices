using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Project.Domain.Entities;

[Table("Projects")]
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; private set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Frontend { get; set; }
    public required string Backend { get; set; }
    public required string Tools { get; set; }
    public required string Url { get; set; }
    public required string Code { get; set; }
    public required string Image { get; set; }
    public bool Finished { get; set; }
}