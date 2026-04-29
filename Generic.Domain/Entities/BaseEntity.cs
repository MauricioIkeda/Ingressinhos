using System.ComponentModel.DataAnnotations;

namespace Generic.Domain.Entities;

public class BaseEntity
    : ValidatableObject
{
    [Key]
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
