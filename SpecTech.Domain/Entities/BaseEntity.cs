namespace SpecTech.Domain.Entities;

public class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}