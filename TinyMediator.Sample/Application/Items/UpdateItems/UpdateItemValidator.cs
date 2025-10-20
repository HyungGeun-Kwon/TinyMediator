using FluentValidation;
using TinyMediator.Sample.Domain.Items;

namespace TinyMediator.Sample.Application.Items.UpdateItems
{
    public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Name).Custom((name, ctx) =>
            {
                if (!ItemName.IsValid(name, out var errMsg))
                {
                    ctx.AddFailure(nameof(UpdateItemCommand.Name),
                        errMsg ?? $"유저명은 {ItemName.MinLength}~{ItemName.MaxLength}자여야 함.");
                }
            });
        }
    }
}
