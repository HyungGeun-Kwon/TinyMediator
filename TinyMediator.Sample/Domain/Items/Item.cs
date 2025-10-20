namespace TinyMediator.Sample.Domain.Items
{
    public sealed class Item
    {
        public ItemId Id { get; private set; } = new(Guid.NewGuid());
        public ItemName Name { get; private set; } = default!;

        private Item() { }
        public Item(ItemName name)
        {
            Rename(name);
        }

        public void Rename(ItemName name)
        {
            Name = name;
        }
    }
}
