namespace Betfair.Tests.Account
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Account;
    using Betfair.Tests.TestDoubles;
    using Newtonsoft.Json;
    using Xunit;

    public class FundsTests : IDisposable
    {
        private readonly Funds funds;

        private readonly SessionSpy session = new SessionSpy();

        private readonly HttpMessageHandlerMock handler = new HttpMessageHandlerMock();

        private bool disposedValue;

        public FundsTests()
        {
            var response = new ResponseStub<AccountFundsResponseStub>(new AccountFundsResponseStub());
            this.handler.WithReturnContent(response);
            this.funds = new Funds(this.session);
            this.funds.WithHandler(this.handler.Build());
        }

        [Fact]
        public void AccountFundsClassIsSealed()
        {
            Assert.True(typeof(Funds).IsSealed);
        }

        [Fact]
        public async Task OnRefreshGetTokenOnSessionIsCalledAsync()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(1, this.session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnRefreshPostRequestIsMade()
        {
            await this.funds.RefreshAsync();
            this.handler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnRefreshAccountUriIsCalled()
        {
            await this.funds.RefreshAsync();
            this.handler.VerifyRequestUri(new Uri("https://api.betfair.com/exchange/account/json-rpc/v1"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnRefreshAppKeyIsInRequestHeader(string appKey)
        {
            this.session.AppKey = appKey;
            await this.funds.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnRefreshSessionTokenIsInRequestHeader(string sessionToken)
        {
            this.session.SessionToken = sessionToken;
            await this.funds.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnRefreshRequestContentIsSet()
        {
            await this.funds.RefreshAsync();
            this.handler.VerifyRequestContent(JsonConvert.SerializeObject(new GetAccountFundsRequestStub()));
        }

        [Fact]
        public async Task OnRefreshAvailableToBetBalanceIsSet()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(1000, this.funds.AvailableToBetBalance);
        }

        [Fact]
        public async Task OnRefreshExposureIsSet()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(100, this.funds.Exposure);
        }

        [Fact]
        public async Task OnRefreshRetainedCommissionIsSet()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(10, this.funds.RetainedCommission);
        }

        [Fact]
        public async Task OnRefreshExposureLimitIsSet()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(10000, this.funds.ExposureLimit);
        }

        [Fact]
        public async Task OnRefreshDiscountRateIsSet()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(0.1, this.funds.DiscountRate);
        }

        [Fact]
        public async Task OnRefreshPointsBalanceIsSet()
        {
            await this.funds.RefreshAsync();
            Assert.Equal(9, this.funds.PointsBalance);
        }

        [Fact]
        public async Task OnRefreshThrowIfBetfairReturnsError()
        {
            var response = new ResponseStub<AccountFundsResponseStub>(new AccountFundsResponseStub()) { Error = "Error" };
            this.handler.WithReturnContent(response);
            this.funds.WithHandler(this.handler.Build());
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => this.funds.RefreshAsync());
            Assert.Equal("Error", exception.Message);
        }

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
            }

            this.funds.Dispose();
            this.handler.Dispose();
            this.disposedValue = true;
        }
    }
}
