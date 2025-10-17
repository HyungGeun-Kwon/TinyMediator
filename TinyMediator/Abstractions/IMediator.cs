namespace TinyMediator.Abstractions
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    }
}
