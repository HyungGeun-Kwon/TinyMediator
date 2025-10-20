using TinyMediator.Abstractions;
using TinyMediator.Sample.Application.Common;
using TinyMediator.Sample.Application.Ports;

namespace TinyMediator.Sample.Application.Items.GetItems
{
    public sealed class GetItemsHandler(IItemReadStore readStore)
        : IRequestHandler<GetItemsQuery, PageResult<ItemSummaryDto>>
    {
        public Task<PageResult<ItemSummaryDto>> Handle(GetItemsQuery request, CancellationToken ct)
            => readStore.GetPageAsync(request.Page, request.Size, ct);
    }
}
