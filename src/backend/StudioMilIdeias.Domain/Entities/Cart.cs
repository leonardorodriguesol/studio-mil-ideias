namespace StudioMilIdeias.Domain.Entities;

public sealed class Cart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public User? User { get; set; }
}
