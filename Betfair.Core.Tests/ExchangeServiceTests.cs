namespace Betfair.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Betfair.Core.Tests.TestDoubles;
    using Betfair.Exchange.Interfaces;
    using Xunit;

    public class ExchangeServiceTests : IDisposable
    {
        private readonly HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock();

        private readonly ExchangeService exchange = new ExchangeService();

        private bool disposedValue;

        public ExchangeServiceTests()
        {
            this.httpMessageHandler.WithReturnContent(new LoginResponseStub());
            this.exchange.WithHandler(this.httpMessageHandler.Build());
        }

        [Fact]
        public void ClientShouldImplementIExchangeService()
        {
            Assert.True(typeof(IExchangeService).IsAssignableFrom(typeof(ExchangeService)));
        }

        [Fact]
        public async Task TheSpecifiedEndpointIsCalled()
        {
            await this.exchange.SendAsync<string>("endpoint", "method", "parameters");
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://api.betfair.com/exchange/betting/json-rpc/v1"));
        }

        //[Fact]
        //public async Task KeepAliveIsInHttpRequestHeader()
        //{
        //    await this.client.SendAsync<string>("endpoint", "method", "parameters");
        //    this.httpMessageHandler.VerifyHeaderValues("Connection", "keep-alive");
        //}

        //[Fact]
        //public async Task SessionTokenIsInHttpRequestHeader()
        //{
        //    await this.SetSessionToken("NewToken");
        //    this.client.WithHandler(this.httpMessageHandler.Build());
        //    await this.client.SendAsync<string>("endpoint", "method", "parameters");
        //this.httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        //}

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue) return;
            if (disposing)
            {
                this.httpMessageHandler.Dispose();
            }

            //this.exchange.Dispose();

            this.disposedValue = true;
        }

        //private async Task SetSessionToken(string sessionToken)
        //{
        //    this.httpMessageHandler.WithReturnContent(
        //        new LoginResponseStub().WithSessionToken(sessionToken));
        //    this.session.WithHandler(this.httpMessageHandler.Build());
        //    await this.session.LoginAsync();
        //}
    }
}
