using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Domain.Entities;
using StudioMilIdeias.Domain.Enums;

namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class DataSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AdminSeedOptions _adminSeedOptions;

    public DataSeeder(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<AdminSeedOptions> adminSeedOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _adminSeedOptions = adminSeedOptions.Value;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = _adminSeedOptions.Email.Trim().ToLowerInvariant();
        var exists = await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        if (exists)
        {
            return;
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = _adminSeedOptions.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(_adminSeedOptions.Password),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(admin, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
