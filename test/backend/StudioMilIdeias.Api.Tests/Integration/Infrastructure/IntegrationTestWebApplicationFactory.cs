using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Domain.Entities;
using StudioMilIdeias.Infrastructure.Persistence;

namespace StudioMilIdeias.Api.Tests.Integration.Infrastructure;

public sealed class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"studio-mil-ideias-tests-{Guid.NewGuid():N}.db");

    private string ConnectionString => $"Data Source={_databasePath}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString,
                ["Jwt:Issuer"] = "StudioMilIdeias.Tests",
                ["Jwt:Audience"] = "StudioMilIdeias.Tests.Client",
                ["Jwt:SecretKey"] = "TEST_ONLY_SUPER_SECRET_KEY_WITH_AT_LEAST_32_CHARS",
                ["Jwt:ExpiresInMinutes"] = "60",
                ["DigitalDelivery:TokenSecret"] =
                    "TEST_ONLY_DIGITAL_DELIVERY_SUPER_SECRET_WITH_AT_LEAST_32_CHARS",
                ["DigitalDelivery:TokenTtlMinutes"] = "30",
                ["Seed:Admin:Name"] = "Admin Test",
                ["Seed:Admin:Email"] = "admin@tests.local",
                ["Seed:Admin:Password"] = "Admin@Test123",
                ["DisableAutoMigration"] = "true"
            };

            configBuilder.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<IApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(ConnectionString));
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            services.RemoveAll<ICheckoutPaymentGateway>();
            services.AddScoped<ICheckoutPaymentGateway, FakeCheckoutPaymentGateway>();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        await SeedCatalogAsync(dbContext);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_databasePath))
        {
            try
            {
                File.Delete(_databasePath);
            }
            catch (IOException)
            {
                // Ignore temporary file cleanup failures caused by file locks.
            }
        }
    }

    private static async Task SeedCatalogAsync(ApplicationDbContext dbContext)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Design",
            Slug = "design"
        };

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Template Kit",
            Description = "Template kit for integration tests.",
            Slug = "template-kit",
            Price = 49.90m,
            IsActive = true,
            CategoryId = category.Id,
            DigitalFileUrl = "s3://private/tests/template-kit.zip",
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Categories.AddAsync(category);
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
    }
}
