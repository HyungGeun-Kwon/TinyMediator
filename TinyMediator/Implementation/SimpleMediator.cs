using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TinyMediator.Abstractions;

namespace TinyMediator.Implementation
{
    public sealed class SimpleMediator(IServiceProvider sp) : IMediator
    {
        private static readonly MethodInfo OpenInvokeGeneric =
            typeof(SimpleMediator).GetMethod(nameof(InvokeGeneric), BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new MissingMethodException($"{nameof(InvokeGeneric)} not found.");

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        {
            var reqType = request.GetType();
            var resType = typeof(TResponse);

            // 요청 타입이 IRequest<TResponse>인지 확인
            var expected = typeof(IRequest<>).MakeGenericType(resType);
            if (!expected.IsAssignableFrom(reqType))
                throw new InvalidOperationException($"Request type {reqType} does not implement {expected}.");

            var closed = OpenInvokeGeneric.MakeGenericMethod(reqType, resType);

            return (Task<TResponse>)closed.Invoke(null, [sp, request, ct])!;
        }

        private static Task<TResponse> InvokeGeneric<TRequest, TResponse>(
            IServiceProvider sp,
            TRequest request,
            CancellationToken ct)
            where TRequest : IRequest<TResponse>
        {
            var handler = sp.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            return handler.Handle(request, ct);
        }

        // dynamic 버전
        // public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        // {
        //     var reqType = request.GetType();
        //     var handlerType = typeof(IRequestHandler<,>).MakeGenericType(reqType, typeof(TResponse));
        //     var handler = sp.GetService(handlerType)
        //         ?? throw new InvalidOperationException($"Handler not found: {handlerType.FullName}");
        // 
        //     return ((dynamic)handler).Handle((dynamic)request, ct);
        // }
    }
}
