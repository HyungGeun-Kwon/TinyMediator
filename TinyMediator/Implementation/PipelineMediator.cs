using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TinyMediator.Abstractions;

namespace TinyMediator.Implementation
{
    public sealed class PipelineMediator(IServiceProvider sp) : IMediator
    {
        private static readonly MethodInfo OpenInvokeGeneric =
            typeof(PipelineMediator).GetMethod(nameof(InvokeGeneric), BindingFlags.NonPublic | BindingFlags.Static)
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

            var behaviors = sp.GetServices<IPipelineBehavior<TRequest, TResponse>>().ToArray();

            // 바깥 > 안쪽 순서가 되도록 역순으로 감싸기
            Array.Reverse(behaviors);

            Func<TRequest, CancellationToken, Task<TResponse>> next = handler.Handle;
            foreach (var b in behaviors)
            {
                var current = next;
                next = (req, token) => b.Handle(req, current, token);
            }

            return next(request, ct);
        }

        // dynamic 버전
        // public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        // {
        //     var reqType = request.GetType();
        //     var resType = typeof(TResponse);
        // 
        //     var handlerType = typeof(IRequestHandler<,>).MakeGenericType(reqType, resType);
        //     var handler = sp.GetService(handlerType)
        //         ?? throw new InvalidOperationException($"Handler not found: {handlerType.FullName}");
        // 
        //     var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(reqType, resType);
        // 
        //     var behaviors = sp.GetServices(behaviorType).Cast<object>().Reverse().ToList();
        // 
        //     Func<IRequest<TResponse>, CancellationToken, Task<TResponse>> next =
        //         (req, token) => ((dynamic)handler).Handle((dynamic)req, token);
        // 
        //     foreach (var b in behaviors)
        //     {
        //         var current = next;
        //         next = (req, token) => ((dynamic)b).Handle((dynamic)req, (Func<dynamic, CancellationToken, Task<TResponse>>)current, token);
        //     }
        // 
        //     return next(request, ct);
        // }
    }
}
