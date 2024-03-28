using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Domain.Enums;

namespace Domain.Entities;

public class Question:BaseEntity
{
    [Key]
    public int  Id { get; set; }
    public required string Text { get; set; }
    public required LevelQuestion LevelQuestion { get; set; }
    public required Topic Topic { get; set; }
    public List<Variant> Variants { get; set; }
    public List<QuestionGenerator> QuestionGenerators { get; set; }
   
}