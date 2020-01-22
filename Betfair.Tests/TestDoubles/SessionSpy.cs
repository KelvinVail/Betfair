namespace Betfair.Tests.TestDoubles
{
    using System.Threading.Tasks;

    public class SessionSpy : ISession
    {
        public int TimesGetSessionTokenAsyncCalled { get; private set; }

        public string SessionToken { get; set; } = "SessionToken";

        public string AppKey { get; set; }

        public async Task<string> GetSessionTokenAsync()
        {
            this.TimesGetSessionTokenAsyncCalled++;
            return await Task.Run(() => this.SessionToken);
        }
    }
}
