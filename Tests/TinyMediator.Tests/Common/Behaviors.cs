using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMediator.Abstractions;

namespace TinyMediator.Tests.Common
{
    public sealed class Behavior1<TReq, TRes>(List<string> log) : IPipelineBehavior<TReq, TRes>
        where TReq : IRequest<TRes>
    {
        public async Task<TRes> Handle(TReq request, Func<TReq, CancellationToken, Task<TRes>> next, CancellationToken ct)
        {
            log.Add("Behavior1-in");
            var res = await next(request, ct);
            log.Add("Behavior1-out");
            return res;
        }
    }

    public sealed class Behavior2<TReq, TRes>(List<string> log) : IPipelineBehavior<TReq, TRes>
        where TReq : IRequest<TRes>
    {
        public async Task<TRes> Handle(TReq request, Func<TReq, CancellationToken, Task<TRes>> next, CancellationToken ct)
        {
            log.Add("Behavior2-in");
            var res = await next(request, ct);
            log.Add("Behavior2-out");
            return res;
        }
    }
}
