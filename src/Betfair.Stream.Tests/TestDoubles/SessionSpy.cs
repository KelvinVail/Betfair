using System.Threading.Tasks;
using Betfair.Identity;

namespace Betfair.Stream.Tests.TestDoubles
{
    public class SessionSpy : ISession
    {
        public int TimesGetSessionTokenAsyncCalled { get; private set; }

        public string SessionToken { get; set; } = "SessionToken";

        public string AppKey { get; set; } = "AppKey";

        public async Task<string> GetTokenAsync()
        {
            TimesGetSessionTokenAsyncCalled++;
            return await Task.Run(() => SessionToken);
        }
    }
}
