using Domain.DTOs.QuestionDto;

namespace Domain.DTOs.QuestionGeneratorDto;

public class GenerateQuestionDto
{
    public List<GetQuestionDto> GetQuestion { get; set; }
    public int CountOfTasks { get; set; }
    public DateTimeOffset  FromTime { get; set; }
    public DateTimeOffset ToTime { get; set; }
}