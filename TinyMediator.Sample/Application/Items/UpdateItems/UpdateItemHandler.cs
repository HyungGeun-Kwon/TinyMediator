using TinyMediator.Abstractions;
using TinyMediator.Sample.Application.Ports;
using TinyMediator.Sample.Domain.Items;

namespace TinyMediator.Sample.Application.Items.UpdateItems
{
    public sealed class UpdateItemHandler(IItemRepository repo) : IRequestHandler<UpdateItemCommand, bool>
    {
        public async Task<bool> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var id = new ItemId(request.Id);
            var item = await repo.LoadAsync(id, ct) ?? throw new KeyNotFoundException($"Item({request.Id}) not found."); ;
            var name = new ItemName(request.Name);
            item.Rename(name);
            if (item.Id.Value == Guid.Empty) await repo.AddAsync(item, ct);
            await repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
