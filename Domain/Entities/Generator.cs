using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Generator:BaseEntity
{
    [Key]
    public int Id { get; set; }
    public DateTimeOffset FromTime { get; set; }
    public DateTimeOffset ToTime { get; set; }
    public bool IsActive { get; set; }
    public List<QuestionGenerator>? QuestionGenerators { get; set; }
}