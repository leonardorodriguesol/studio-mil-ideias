namespace StudioMilIdeias.Application.Abstractions;

public interface ICategoryRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
