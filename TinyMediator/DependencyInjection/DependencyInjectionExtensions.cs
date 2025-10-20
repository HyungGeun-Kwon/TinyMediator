using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TinyMediator.Abstractions;
using TinyMediator.Implementation;

namespace TinyMediator.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public sealed class MediatorOptions
        {
            public ServiceLifetime HandlerLifetime { get; set; } = ServiceLifetime.Scoped;
            public ServiceLifetime BehaviorLifetime { get; set; } = ServiceLifetime.Scoped;
            public bool UsePipelineMediator { get; set; } = true;
        }
        public static IServiceCollection AddMediator(
            this IServiceCollection services,
            params Assembly[] assemblies)
            => services.AddMediator(_ => { }, assemblies);

        public static IServiceCollection AddMediator(
            this IServiceCollection services,
            Action<MediatorOptions> configure,
            params Assembly[] assemblies)
        {
            if (assemblies is null || assemblies.Length == 0)
                throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));

            var opts = new MediatorOptions();
            configure?.Invoke(opts);

            // IMediator 등록 (PipelineMediator 기본)
            if (opts.UsePipelineMediator)
                services.TryAddScoped<IMediator, PipelineMediator>();
            else
                services.TryAddScoped<IMediator, SimpleMediator>();

            // 스캔 대상 타입
            var types = assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .ToArray();

            var handlerOpen = typeof(IRequestHandler<,>);
            var behaviorOpen = typeof(IPipelineBehavior<,>);
            foreach (var impl in types)
            {
                if (impl.IsAbstract || impl.IsInterface) continue;

                foreach (var itf in impl.GetInterfaces())
                {
                    if (!itf.IsGenericType) continue;

                    var def = itf.GetGenericTypeDefinition();

                    // Handlers
                    if (def == handlerOpen)
                    {
                        // 구현이 오픈 제네릭이면: 서비스도 오픈으로 등록
                        // (예: GenericHandler<TReq,TRes> : IRequestHandler<TReq,TRes>)
                        var serviceType = impl.IsGenericTypeDefinition ? handlerOpen : itf;
                        services.Add(new ServiceDescriptor(serviceType, impl, opts.HandlerLifetime));
                    }

                    // Behaviors
                    if (def == behaviorOpen)
                    {
                        // 비헤이비어는 보통 오픈 제네릭 구현(LoggingBehavior<,>)이므로,
                        // 구현이 오픈이면 서비스 타입은 반드시 typeof(IPipelineBehavior<,>)로 등록해야 함.
                        var serviceType = impl.IsGenericTypeDefinition ? behaviorOpen : itf;
                        services.Add(new ServiceDescriptor(serviceType, impl, opts.BehaviorLifetime));
                    }
                }
            }

            return services;
        }

        public static IServiceCollection AddMediatorFromAssemblyContaining<T>(this IServiceCollection services)
            => services.AddMediator(typeof(T).Assembly);
    }
}
