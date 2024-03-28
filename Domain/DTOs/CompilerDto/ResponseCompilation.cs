namespace Domain.DTOs.CompilerDto;

public class ResponseCompilation
{
    
    public int Id { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset Time { get; set; }
    public int CountAllTests { get; set; }
    public int CountSuccessTests { get; set; }
    public string? Error { get; set; }
}

public enum Status
{
    Correct = 1,
    Fail = 2
}