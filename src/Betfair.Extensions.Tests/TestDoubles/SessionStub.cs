namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Betfair.Identity;

    public class SessionStub : ISession
    {
        public string AppKey { get; } = "AppKey";

        public async Task<string> GetTokenAsync()
        {
            return await Task.FromResult("Token");
        }
    }
}
