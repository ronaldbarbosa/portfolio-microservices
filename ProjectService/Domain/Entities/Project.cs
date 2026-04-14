using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectService.Domain.Entities;

[Table("Projects")]
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Frontend { get; set; }
    public string Backend { get; set; }
    public string Tools { get; set; }
    public string Url { get; set; }
    public string Code { get; set; }
    public string Image { get; set; }
    public bool Finished { get; set; }
}