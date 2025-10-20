using TinyMediator.Abstractions;

namespace TinyMediator.Sample.Application.Items.UpdateItems
{
    public sealed record UpdateItemCommand(Guid Id, string Name) : IRequest<bool>;

}
