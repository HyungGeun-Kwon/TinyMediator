using TinyMediator.Abstractions;
using TinyMediator.Sample.Application.Common;
using TinyMediator.Sample.Application.Ports;

namespace TinyMediator.Sample.Application.Items.GetItems
{
    public sealed record GetItemsQuery(int Page = 1, int Size = 10) : IRequest<PageResult<ItemSummaryDto>>;
}
