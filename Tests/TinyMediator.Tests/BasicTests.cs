using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TinyMediator.Abstractions;
using TinyMediator.Implementation;
using TinyMediator.Tests.Common;

namespace TinyMediator.Tests
{
    public class BasicTests
    {
        [Fact]
        public async Task SimpleMediator_resolves_handler_and_returns_value()
        {
            using var sp = TestServicesExtensions.BuildWithSimpleMediator();
            var m = sp.GetRequiredService<IMediator>();

            var result = await m.Send(new TestCommand("x"));

            result.Should().Be("Result: x");
        }

        [Fact]
        public async Task PipelineMediator_runs_behaviors_in_registration_order_outer_to_inner()
        {
            var log = new List<string>();
            using var sp = TestServicesExtensions.BuildWithPipelineMediatorAndBehaviors(log);
            var m = sp.GetRequiredService<IMediator>();

            var _ = await m.Send(new TestCommand("y"));

            var expected = new[] { "Behavior1-in", "Behavior2-in", "Behavior2-out", "Behavior1-out" };

            log.Should().Equal(expected);
        }

        [Fact]
        public async Task PipelineMediator_works_without_any_behavior()
        {
            var sc = new ServiceCollection();
            sc.AddScoped<IMediator, PipelineMediator>();
            sc.AddScoped<IRequestHandler<TestCommand, string>, TestHandler>();
            await using var sp = sc.BuildServiceProvider();

            var m = sp.GetRequiredService<IMediator>();
            var s = await m.Send(new TestCommand("z"));
            s.Should().Be("Result: z");
        }

        [Fact]
        public async Task CancellationToken_is_propagated()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(10));

            var sc = new ServiceCollection();
            sc.AddScoped<IMediator, SimpleMediator>();
            sc.AddScoped<IRequestHandler<TestCommand, string>>(_ => new TestHandlerWithDelay());
            await using var sp = sc.BuildServiceProvider();

            var m = sp.GetRequiredService<IMediator>();
            Func<Task> act = () => m.Send(new TestCommand("delay"), cts.Token);

            await act.Should().ThrowAsync<TaskCanceledException>();
        }

        private sealed class TestHandlerWithDelay : IRequestHandler<TestCommand, string>
        {
            public async Task<string> Handle(TestCommand request, CancellationToken ct)
            {
                await Task.Delay(1000, ct); // CT 반영되는지 확인
                return "never";
            }
        }
    }
}
