namespace Betfair.Core.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Betfair.Identity;

    public sealed class SessionStub : ISession
    {
        public string Token { get; set; } = "Token";

        public string AppKey { get; } = "AppKey";

        public async Task<string> GetTokenAsync()
        {
            return await Task.FromResult(this.Token);
        }
    }
}
