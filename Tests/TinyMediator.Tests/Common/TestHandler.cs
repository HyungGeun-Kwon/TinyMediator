using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMediator.Abstractions;

namespace TinyMediator.Tests.Common
{
    public sealed record TestCommand(string Msg) : IRequest<string>;
    public sealed class TestHandler : IRequestHandler<TestCommand, string>
    {
        public Task<string> Handle(TestCommand request, CancellationToken ct = default)
            => Task.FromResult($"Result: {request.Msg}");
    }
}
