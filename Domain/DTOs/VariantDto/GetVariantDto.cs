namespace Domain.DTOs.VariantDto;

public class GetVariantDto
{
    public int Id { get; set; }
    public required string Input { get; set; }
    public required string Output{ get; set; }
    public int QuestionId { get; set; }
}