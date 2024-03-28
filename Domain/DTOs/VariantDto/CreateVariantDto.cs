namespace Domain.DTOs.VariantDto;

public class CreateVariantDto
{
    public required string Input { get; set; }
    public required string Output{ get; set; }
    public int  QuestionId { get; set; }
}