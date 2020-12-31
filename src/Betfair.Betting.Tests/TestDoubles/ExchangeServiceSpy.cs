using System.Collections.Generic;
using System.Threading.Tasks;
using Betfair.Exchange.Interfaces;
using Newtonsoft.Json;

namespace Betfair.Betting.Tests.TestDoubles
{
    public class ExchangeServiceSpy : IExchangeService
    {
        public string Endpoint { get; private set; }

        public string BetfairMethod { get; private set; }

        public Dictionary<string, string> SentParameters { get; } = new Dictionary<string, string>();

        private Dictionary<string, string> ReturnContent { get; } = new Dictionary<string, string>();

        public ExchangeServiceSpy WithReturnContent(string method, string content)
        {
            ReturnContent.Add(method, content);
            return this;
        }

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            Endpoint = endpoint;
            BetfairMethod = betfairMethod;
            SentParameters.Add(betfairMethod, parameters);
            return await Task.FromResult(ReturnContent.ContainsKey(betfairMethod) ? JsonConvert.DeserializeObject<T>(ReturnContent[betfairMethod]) : default);
        }
    }
}
