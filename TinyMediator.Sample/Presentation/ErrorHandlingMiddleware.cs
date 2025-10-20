using FluentValidation;

namespace TinyMediator.Sample.Presentation
{
    public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await next(ctx);
            }
            catch (ValidationException vex)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    error = "validation_failed",
                    details = vex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (TaskCanceledException)
            {
                ctx.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error");
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new { error = "internal_error" });
            }
        }
    }
}
