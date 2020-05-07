namespace Betfair.Core
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;

    public class ExchangeService : IExchangeService
    {
        private readonly ExchangeHttpClient client = new ExchangeHttpClient(new Uri("https://api.betfair.com/exchange/betting/json-rpc/v1"));

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            await this.client.SendAsync<dynamic>(new HttpRequestMessage());
            return default;
        }

        public ExchangeService WithHandler(HttpClientHandler handler)
        {
            this.client.WithHandler(handler);
            return this;
        }
    }
}
