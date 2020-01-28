namespace Betfair.Tests.Sports
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Sports;
    using Betfair.Tests.TestDoubles;
    using Newtonsoft.Json;
    using Xunit;

    public class MarketCatalogueTests : IDisposable
    {
        private readonly SessionSpy session = new SessionSpy();

        private readonly HttpMessageHandlerMock handler = new HttpMessageHandlerMock();

        private readonly MarketCatalogue catalogue;

        public MarketCatalogueTests()
        {
            this.catalogue = new MarketCatalogue(this.session);
            this.catalogue.WithHandler(this.handler.Build());
        }

        [Fact]
        public void MarketCatalogueIsSealed()
        {
            Assert.True(typeof(MarketCatalogue).IsSealed);
        }

        [Fact]
        public async Task OnRefreshGetTokenOnSessionIsCalledAsync()
        {
            await this.catalogue.RefreshAsync();
            Assert.Equal(1, this.session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnRefreshPostRequestIsMade()
        {
            await this.catalogue.RefreshAsync();
            this.handler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public void MarketCatalogueIsDisposable()
        {
            Assert.True(typeof(IDisposable).IsAssignableFrom(typeof(MarketCatalogue)));
        }

        [Fact]
        public async Task OnRefreshBettingUriIsCalled()
        {
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestUri(new Uri("https://api.betfair.com/exchange/betting/json-rpc/v1"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnRefreshAppKeyIsInRequestHeader(string appKey)
        {
            this.session.AppKey = appKey;
            await this.catalogue.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnRefreshSessionTokenIsInRequestHeader(string sessionToken)
        {
            this.session.SessionToken = sessionToken;
            await this.catalogue.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnRefreshRequestMethodIsListMarketCatalogue()
        {
            await this.catalogue.RefreshAsync();
            var expectedRequest = JsonConvert.SerializeObject(new RequestBuilder().WithMethod("SportsAPING/v1.0/listMarketCatalogue"));
            this.handler.VerifyRequestContent(expectedRequest);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.handler.Dispose();
            this.catalogue.Dispose();
        }
    }
}
