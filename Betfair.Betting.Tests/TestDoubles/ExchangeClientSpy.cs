namespace Betfair.Betting.Tests.TestDoubles
{
    using System.Threading.Tasks;

    public class ExchangeClientSpy : IExchangeClient
    {
        public string Endpoint { get; private set; }

        public string Parameters { get; private set; }

        public string BetfairMethod { get; private set; }

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            this.Endpoint = endpoint;
            this.BetfairMethod = betfairMethod;
            this.Parameters = parameters;
            return await Task.FromResult((T)default);
        }
    }
}
