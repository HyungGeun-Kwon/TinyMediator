using System.Collections.Concurrent;
using TinyMediator.Sample.Domain.Items;

namespace TinyMediator.Sample.Infrastructure.Items
{
    public sealed class InMemoryItemsDb
    {
        public ConcurrentDictionary<Guid, Item> Items { get; } = [];

        public InMemoryItemsDb()
        {
            Seed("Alpha", "Beta", "Gamma");
        }

        private void Seed(params string[] names)
        {
            foreach (var name in names)
            {
                var item = new Item(new ItemName(name));
                Items[item.Id.Value] = item;
            }
        }
    }
}
