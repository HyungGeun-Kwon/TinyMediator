using System.Diagnostics;
using TinyMediator.Abstractions;

namespace TinyMediator.Sample.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            Func<TRequest, CancellationToken, Task<TResponse>> next,
            CancellationToken ct = default)
        {
            var name = typeof(TRequest).Name;
            var sw = Stopwatch.StartNew();
            logger.LogInformation("Handling {Request}", name);
            try
            {
                var res = await next(request, ct);
                sw.Stop();
                logger.LogInformation("Handling Success. {Request} in {ElapsedMilliseconds}ms", name, sw.ElapsedMilliseconds);
                return res;
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(ex, "Error Handling {Request} after {ElapsedMilliseconds}ms", name, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
