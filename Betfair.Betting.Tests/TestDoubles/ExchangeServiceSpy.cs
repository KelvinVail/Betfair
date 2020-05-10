namespace Betfair.Betting.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public class ExchangeServiceSpy : IExchangeService
    {
        public string Endpoint { get; private set; }

        public string BetfairMethod { get; private set; }

        public Dictionary<string, string> SentParameters { get; } = new Dictionary<string, string>();

        private Dictionary<string, string> ReturnContent { get; } = new Dictionary<string, string>();

        public ExchangeServiceSpy WithReturnContent(string method, string content)
        {
            this.ReturnContent.Add(method, content);
            return this;
        }

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            this.Endpoint = endpoint;
            this.BetfairMethod = betfairMethod;
            this.SentParameters.Add(betfairMethod, parameters);
            return await Task.FromResult(this.ReturnContent.ContainsKey(betfairMethod) ? JsonSerializer.Deserialize<T>(this.ReturnContent[betfairMethod], StandardResolver.AllowPrivateExcludeNull) : default);
        }
    }
}
