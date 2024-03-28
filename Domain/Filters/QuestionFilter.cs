using Domain.Enums;

namespace Domain.Filters;

public class QuestionFilter:PaginationFilter
{
    public string? Text { get; set; }
    public LevelQuestion? LevelQuestion { get; set; }
    public Topic? Topic { get; set; }
}