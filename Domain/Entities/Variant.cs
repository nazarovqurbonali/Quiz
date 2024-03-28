using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Variant:BaseEntity
{
    [Key]
    public int Id { get; set; }
    public required string Input { get; set; }
    public required string Output{ get; set; }

    [ForeignKey("QuestionId")]
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    

    
}