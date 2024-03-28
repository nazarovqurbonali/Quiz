using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class SaveTrial:BaseEntity
{
    [Key]
    public int Id { get; set; }

    public int  UserId { get; set; }

    public int  QuestionId { get; set; }
    public required string SolveTask { get; set; }
    public bool IsSolve { get; set; }
}