using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Job
{
    public Job(int id, string title, string cron, string jobType, string jobKey, string jobGroup, bool startNow, bool isActive, string description, DateTimeOffset createdAt, DateTimeOffset dateUpdate)
    {
        Id = id;
        Title = title;
        Cron = cron;
        JobType = jobType;
        JobKey = jobKey;
        JobGroup = jobGroup;
        StartNow = startNow;
        IsActive = isActive;
        Description = description;
        CreatedAt = createdAt;
        DateUpdate = dateUpdate;
    }

    public Job(){}
    
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public string Title { get; set; } = null!;
    [MaxLength(200)]
    public string Cron { get; set; } = null!;
    [MaxLength(500)]
    public string JobType { get; set; } = null!;

    [MaxLength(200)]
    public string JobKey { get; set; } = null!;
    [MaxLength(200)]
    public string JobGroup { get; set; } = null!;
    public bool StartNow { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    [MaxLength(100)]
    public DateTimeOffset DateUpdate { get; set; }
}