namespace Betfair.Tests.Sports
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Sports;
    using Betfair.Tests.TestDoubles;
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
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}}}";
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithEventTypeIdMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":" +
                                   "{\"eventTypeIds\":[\"7\"]}}}";
            this.catalogue.EventTypeIds.Add("7");
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithCountryMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":" +
                                   "{\"marketCountries\":[\"GB\"]}}}";
            this.catalogue.Countries.Add("GB");
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithDateRangeFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":" +
                                   "{\"marketStartTime\":{\"from\":\"2020-02-09T00:00:00Z\",\"to\":\"2020-02-10T00:00:00Z\"}}}}";
            var from = DateTime.Parse("2020-02-09 17:00:00", new DateTimeFormatInfo());
            var to = DateTime.Parse("2020-02-10 17:00:00", new DateTimeFormatInfo());
            this.catalogue.WithDateRange(from, to);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithMarketTypeCodeMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":" +
                                   "{\"marketTypeCodes\":[\"WIN\"]}}}";
            this.catalogue.WithMarketTypeCode("WIN");
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithMaxResultsMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"maxResults\":\"100\"}}";
            this.catalogue.WithMaxResults(100);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithSortByMinimumTradedMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"sort\":\"MINIMUM_TRADED\"}}";
            this.catalogue.WithSortOrder(CatalogueSort.MinimumTraded);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithSortByMaximumTradedMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"sort\":\"MAXIMUM_TRADED\"}}";
            this.catalogue.WithSortOrder(CatalogueSort.MaximumTraded);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithSortByMinimumAvailableMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"sort\":\"MINIMUM_AVAILABLE\"}}";
            this.catalogue.WithSortOrder(CatalogueSort.MinimumAvailable);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithSortByMaximumAvailableMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"sort\":\"MAXIMUM_AVAILABLE\"}}";
            this.catalogue.WithSortOrder(CatalogueSort.MaximumAvailable);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithSortByFirstToStartMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"sort\":\"FIRST_TO_START\"}}";
            this.catalogue.WithSortOrder(CatalogueSort.FirstToStart);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithSortByLastToStartMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"sort\":\"LAST_TO_START\"}}";
            this.catalogue.WithSortOrder(CatalogueSort.LastToStart);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithCompetitionMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"COMPETITION\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.Competition);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithEventMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"EVENT\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.Event);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithEventTypeMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"EVENT_TYPE\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.EventType);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithMarketDescriptionMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"MARKET_DESCRIPTION\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.MarketDescription);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithMarketStartTimeMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"MARKET_START_TIME\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.MarketStartTime);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithRunnerDescriptionMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"RUNNER_DESCRIPTION\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.RunnerDescription);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
        }

        [Fact]
        public async Task OnWithRunnerRunnerMetadataMarketFilterIsSet()
        {
            const string request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"SportsAPING/v1.0/listMarketCatalogue\",\"params\":{\"filter\":{}," +
                                   "\"marketProjection\":[\"RUNNER_METADATA\"]}}";
            this.catalogue.WithMarketProjection(MarketProjection.RunnerMetadata);
            await this.catalogue.RefreshAsync();
            this.handler.VerifyRequestContent(request);
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
