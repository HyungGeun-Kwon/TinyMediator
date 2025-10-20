using TinyMediator.Sample.Application.Common;

namespace TinyMediator.Sample.Application.Ports
{
    public sealed record ItemSummaryDto(Guid Id, string Name);
    public sealed record ItemDetailDto(Guid Id, string Name);

    public interface IItemReadStore
    {
        Task<PageResult<ItemSummaryDto>> GetPageAsync(int page, int size, CancellationToken ct);
        Task<ItemDetailDto?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
