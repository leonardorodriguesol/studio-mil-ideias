using Microsoft.EntityFrameworkCore;
using StudioMilIdeias.Application.Abstractions;

namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
