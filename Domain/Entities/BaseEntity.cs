namespace Domain.Entities;

public class BaseEntity
{
    public DateTimeOffset CreateAt { get; set; } = DateTime.UtcNow;
    public DateTimeOffset UpdateAt { get; set; } = DateTime.UtcNow;

}