namespace Betfair.Stream.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Betfair.Identity;

    public class SessionSpy : ISession
    {
        public int TimesGetSessionTokenAsyncCalled { get; private set; }

        public string SessionToken { get; set; } = "SessionToken";

        public string AppKey { get; set; } = "AppKey";

        public async Task<string> GetTokenAsync()
        {
            this.TimesGetSessionTokenAsyncCalled++;
            return await Task.Run(() => this.SessionToken);
        }
    }
}
