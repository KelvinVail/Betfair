using System.Threading.Tasks;
using Betfair.Identity;

namespace Betfair.Core.Tests.TestDoubles
{
    public sealed class SessionStub : ISession
    {
        public string Token { get; set; } = "Token";

        public string AppKey { get; } = "AppKey";

        public async Task<string> GetTokenAsync()
        {
            return await Task.FromResult(Token);
        }
    }
}
