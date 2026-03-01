using Microsoft.EntityFrameworkCore;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders.AddAsync(order, cancellationToken).AsTask();
    }

    public Task AddItemsAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
    {
        return _dbContext.OrderItems.AddRangeAsync(items, cancellationToken);
    }

    public Task<Order?> GetByPaymentIdTrackedAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders.FirstOrDefaultAsync(x => x.PaymentId == paymentId, cancellationToken);
    }
}
