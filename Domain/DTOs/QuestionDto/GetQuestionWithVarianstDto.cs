using Domain.DTOs.VariantDto;

namespace Domain.DTOs.QuestionDto;

public class GetQuestionWithVarianstDto
{
    public int  Id { get; set; }
    public required string Text { get; set; }
    public required  string QuestionType  { get; set; }
    public required string Topic { get; set; }
    public List<GetVariantDto>? Tests  { get; set; }
}