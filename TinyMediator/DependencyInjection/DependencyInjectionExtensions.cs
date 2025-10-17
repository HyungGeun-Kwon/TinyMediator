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
                foreach (var itf in impl.GetInterfaces())
                {
                    if (itf.IsGenericType && itf.GetGenericTypeDefinition() == handlerOpen)
                        services.Add(new ServiceDescriptor(itf, impl, opts.HandlerLifetime));
                    
                    if (itf.IsGenericType && itf.GetGenericTypeDefinition() == behaviorOpen)
                        services.Add(new ServiceDescriptor(itf, impl, opts.BehaviorLifetime));
                }
            }

            return services;
        }

        public static IServiceCollection AddMediatorFromAssemblyContaining<T>(this IServiceCollection services)
            => services.AddMediator(typeof(T).Assembly);
    }
}
