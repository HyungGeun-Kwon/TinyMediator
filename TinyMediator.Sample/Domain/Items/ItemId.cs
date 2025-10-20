namespace TinyMediator.Sample.Domain.Items
{
    public readonly record struct ItemId(Guid Value)
    {
        public static implicit operator Guid(ItemId id) => id.Value;
        public static ItemId New() => new(Guid.NewGuid());
    }
}
