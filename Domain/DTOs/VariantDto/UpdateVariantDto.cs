namespace Domain.DTOs.VariantDto;

public class UpdateVariantDto
{
    public int Id { get; set; }
    public required string Input { get; set; }
    public required string Output{ get; set; }
    public int QuestionId { get; set; }
    
}