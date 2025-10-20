using TinyMediator.Sample.Application.Ports;
using TinyMediator.Sample.Domain.Items;

namespace TinyMediator.Sample.Infrastructure.Items
{
    public sealed class InMemoryItemRepository(InMemoryItemsDb db) : IItemRepository
    {
        public Task<Item?> LoadAsync(ItemId id, CancellationToken ct)
        {
            db.Items.TryGetValue(id.Value, out var item);
            return Task.FromResult(item);
        }

        public Task AddAsync(Item item, CancellationToken ct)
        {
            db.Items[item.Id.Value] = item;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
    }
}
