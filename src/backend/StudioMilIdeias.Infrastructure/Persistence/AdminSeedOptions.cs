namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class AdminSeedOptions
{
    public const string SectionName = "Seed:Admin";

    public string Name { get; set; } = "Admin";
    public string Email { get; set; } = "admin@studiomilideias.local";
    public string Password { get; set; } = "Admin@123456";
}
