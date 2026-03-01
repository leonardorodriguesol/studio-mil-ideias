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
        await SeedAdminAsync(cancellationToken);
        await SeedCatalogAsync(cancellationToken);
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
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

    private async Task SeedCatalogAsync(CancellationToken cancellationToken)
    {
        var defaultCategories = new[]
        {
            new Category { Id = Guid.NewGuid(), Name = "Design", Slug = "design" },
            new Category { Id = Guid.NewGuid(), Name = "Marketing", Slug = "marketing" },
            new Category { Id = Guid.NewGuid(), Name = "Produtividade", Slug = "produtividade" }
        };

        foreach (var category in defaultCategories)
        {
            var categoryExists = await _dbContext.Categories
                .AnyAsync(x => x.Slug == category.Slug, cancellationToken);
            if (!categoryExists)
            {
                await _dbContext.Categories.AddAsync(category, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var categoriesBySlug = await _dbContext.Categories
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Slug, x => x.Id, cancellationToken);

        var defaultProducts = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Pack de Templates para Redes Sociais",
                Description = "Colecao com templates editaveis para Instagram e Facebook.",
                Slug = "pack-templates-redes-sociais",
                Price = 97.00m,
                IsActive = true,
                CategoryId = categoriesBySlug["design"],
                DigitalFileUrl = "s3://private/products/pack-templates-redes-sociais.zip",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Guia Pratico de Lancamentos Digitais",
                Description = "Metodo passo a passo para planejar e executar lancamentos.",
                Slug = "guia-pratico-lancamentos-digitais",
                Price = 147.00m,
                IsActive = true,
                CategoryId = categoriesBySlug["marketing"],
                DigitalFileUrl = "s3://private/products/guia-pratico-lancamentos-digitais.pdf",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Notion Workspace para Criadores",
                Description = "Workspace completo para organizar rotina, tarefas e conteudo.",
                Slug = "notion-workspace-criadores",
                Price = 79.00m,
                IsActive = true,
                CategoryId = categoriesBySlug["produtividade"],
                DigitalFileUrl = "s3://private/products/notion-workspace-criadores.zip",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var product in defaultProducts)
        {
            var productExists = await _dbContext.Products
                .AnyAsync(x => x.Slug == product.Slug, cancellationToken);
            if (!productExists)
            {
                await _dbContext.Products.AddAsync(product, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
