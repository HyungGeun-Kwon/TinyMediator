using TinyMediator.Sample.Domain.Items;

namespace TinyMediator.Sample.Application.Ports
{
    public interface IItemRepository
    {
        Task<Item?> LoadAsync(ItemId id, CancellationToken ct);
        Task AddAsync(Item item, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
