using FluentValidation;
using TinyMediator.Sample.Application.Common;

namespace TinyMediator.Sample.Application.Items.GetItems
{
    public sealed class GetItemsValidator : AbstractValidator<GetItemsQuery>
    {
        public GetItemsValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(Paging.MinPage);
            RuleFor(x => x.Size).InclusiveBetween(Paging.MinSize, Paging.MaxSize);
        }
    }
}
