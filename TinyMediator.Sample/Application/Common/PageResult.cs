namespace TinyMediator.Sample.Application.Common
{
    public sealed class PageResult<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public required int Page { get; init; }
        public required int Size { get; init; }
        public required int Total { get; init; }
    }
}
