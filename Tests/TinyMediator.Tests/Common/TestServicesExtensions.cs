using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TinyMediator.Abstractions;
using TinyMediator.Implementation;

namespace TinyMediator.Tests.Common
{
    public static class TestServicesExtensions
    {
        public static ServiceProvider BuildWithSimpleMediator()
        {
            var sc = new ServiceCollection();
            sc.AddScoped<IMediator, SimpleMediator>();
            sc.AddScoped<IRequestHandler<TestCommand, string>, TestHandler>();
            return sc.BuildServiceProvider();
        }

        public static ServiceProvider BuildWithPipelineMediatorAndBehaviors(List<string> log)
        {
            var sc = new ServiceCollection();
            sc.AddScoped<IMediator, PipelineMediator>();
            sc.AddScoped<IRequestHandler<TestCommand, string>, TestHandler>();
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behavior1<,>));
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behavior2<,>));
            sc.AddSingleton(log); // 순서 검증 로그 공유
            return sc.BuildServiceProvider();
        }
    }
}
