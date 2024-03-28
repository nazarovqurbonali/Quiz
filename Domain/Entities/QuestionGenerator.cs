namespace Domain.Entities;

public class QuestionGenerator
{
    public int GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public int  QuestionId { get; set; }
    public Question? Question { get; set; }
}