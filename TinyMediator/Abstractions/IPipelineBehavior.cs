namespace TinyMediator.Abstractions
{
    public interface IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            Func<TRequest, CancellationToken, Task<TResponse>> next,
            CancellationToken ct = default);
    }
}
