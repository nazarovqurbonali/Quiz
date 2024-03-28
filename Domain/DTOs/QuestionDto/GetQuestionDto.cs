namespace Domain.DTOs.QuestionDto;

public class GetQuestionDto
{
    public int  Id { get; set; }
    public required string Text { get; set; }
    public required  string QuestionType  { get; set; }
    public required string Topic { get; set; }
}