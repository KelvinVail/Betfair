namespace Betfair.Sports
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class MarketCatalogue : IDisposable
    {
        private readonly ExchangeService client;

        public MarketCatalogue(ISession session)
        {
            this.client = new ExchangeService(session, "betting");
        }

        public MarketCatalogue WithHandler(HttpClientHandler handler)
        {
            this.client.WithHandler(handler);
            return this;
        }

        public async Task RefreshAsync()
        {
            await this.client.SendAsync<dynamic>("listMarketCatalogue");
        }

        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
