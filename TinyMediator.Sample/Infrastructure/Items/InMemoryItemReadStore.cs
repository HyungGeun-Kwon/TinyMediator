using TinyMediator.Sample.Application.Common;
using TinyMediator.Sample.Application.Ports;

namespace TinyMediator.Sample.Infrastructure.Items
{
    public sealed class InMemoryItemReadStore(InMemoryItemsDb db) : IItemReadStore
    {
        public Task<PageResult<ItemSummaryDto>> GetPageAsync(int page, int size, CancellationToken ct)
        {
            var all = db.Items.Values
                .Select(i => new ItemSummaryDto(i.Id.Value, i.Name.Value))
                .OrderBy(x => x.Name)
                .ToList();

            var total = all.Count;
            var slice = all.Skip((page - 1) * size).Take(size).ToList();

            return Task.FromResult(new PageResult<ItemSummaryDto>
            {
                Items = slice,
                Page = page,
                Size = size,
                Total = total
            });
        }

        public Task<ItemDetailDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var dto = db.Items.TryGetValue(id, out var item)
                ? new ItemDetailDto(item.Id.Value, item.Name.Value)
                : null;

            return Task.FromResult(dto);
        }
    }
}
