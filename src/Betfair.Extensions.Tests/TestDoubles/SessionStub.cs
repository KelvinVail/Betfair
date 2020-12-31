using System.Threading.Tasks;
using Betfair.Identity;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class SessionStub : ISession
    {
        public string AppKey { get; } = "AppKey";

        public async Task<string> GetTokenAsync()
        {
            return await Task.FromResult("Token");
        }
    }
}
