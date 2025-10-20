using TinyMediator.Abstractions;
using TinyMediator.Sample.Application.Items.GetItems;
using TinyMediator.Sample.Application.Items.UpdateItems;

namespace TinyMediator.Sample.Presentation
{
    public static class ItemsEndpoints
    {
        public static IEndpointRouteBuilder MapItems(this IEndpointRouteBuilder app)
        {
            app.MapGet("/items", async (IMediator mediator, int page = 1, int size = 10, CancellationToken ct = default) =>
            {
                var result = await mediator.Send(new GetItemsQuery(page, size), ct);
                return Results.Ok(result);
            });

            app.MapPost("/items/{id:guid}", async (IMediator mediator, Guid id, string name, CancellationToken ct = default) =>
            {
                var ok = await mediator.Send(new UpdateItemCommand(id, name), ct);
                return ok ? Results.NoContent() : Results.BadRequest();
            });

            return app;
        }
    }
}
