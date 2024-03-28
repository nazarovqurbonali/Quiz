using Domain.Enums;

namespace Domain.DTOs.QuestionDto;

public class CreateQuestionDto
{
    public required string Text { get; set; }
    public required  LevelQuestion LevelQuestion  { get; set; }
    public required Topic Topic { get; set; }
}