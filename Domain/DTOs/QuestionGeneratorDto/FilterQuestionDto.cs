using Domain.Enums;

namespace Domain.DTOs.QuestionGeneratorDto;

public class FilterQuestionDto
{
    public DateTimeOffset FromTime { get; set; }
    public DateTimeOffset ToTime { get; set; }
    public int  CountOfEasyTask { get; set; }
    public int CountOfMediumTask { get; set; }
    public int CountOfHardTask { get; set; }
    public int CountOfSuperHardTask { get; set; }
    public Topic Topic { get; set; }
}