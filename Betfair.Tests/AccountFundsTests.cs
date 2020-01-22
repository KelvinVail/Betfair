namespace Betfair.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Tests.TestDoubles;
    using Newtonsoft.Json;
    using Xunit;

    public class AccountFundsTests : IDisposable
    {
        private readonly AccountFunds accountFunds;

        private readonly SessionSpy session = new SessionSpy();

        private readonly HttpMessageHandlerMock handler = new HttpMessageHandlerMock();

        private bool disposedValue;

        public AccountFundsTests()
        {
            var response = new ResponseStub<AccountFundsResponseStub>(new AccountFundsResponseStub());
            this.handler.WithReturnContent(response);
            this.accountFunds = new AccountFunds(this.session);
            this.accountFunds.WithHandler(this.handler.Build());
        }

        [Fact]
        public void AccountFundsClassIsSealed()
        {
            Assert.True(typeof(AccountFunds).IsSealed);
        }

        [Fact]
        public void WhenInitializedInheritsHttpClientBase()
        {
            Assert.True(typeof(HttpClientBase).IsAssignableFrom(typeof(AccountFunds)));
        }

        [Fact]
        public async Task OnRefreshGetTokenOnSessionIsCalledAsync()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(1, this.session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnRefreshPostRequestIsMade()
        {
            await this.accountFunds.RefreshAsync();
            this.handler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnRefreshAccountUriIsCalled()
        {
            await this.accountFunds.RefreshAsync();
            this.handler.VerifyRequestUri(new Uri("https://api.betfair.com/exchange/account/json-rpc/v1"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnRefreshAppKeyIsInRequestHeader(string appKey)
        {
            this.session.AppKey = appKey;
            await this.accountFunds.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnRefreshSessionTokenIsInRequestHeader(string sessionToken)
        {
            this.session.SessionToken = sessionToken;
            await this.accountFunds.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnRefreshRequestContentIsSet()
        {
            await this.accountFunds.RefreshAsync();
            this.handler.VerifyRequestContent(JsonConvert.SerializeObject(new GetAccountFundsRequestStub()));
        }

        [Fact]
        public async Task OnRefreshAvailableToBetBalanceIsSet()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(1000, this.accountFunds.AvailableToBetBalance);
        }

        [Fact]
        public async Task OnRefreshExposureIsSet()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(100, this.accountFunds.Exposure);
        }

        [Fact]
        public async Task OnRefreshRetainedCommissionIsSet()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(10, this.accountFunds.RetainedCommission);
        }

        [Fact]
        public async Task OnRefreshExposureLimitIsSet()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(10000, this.accountFunds.ExposureLimit);
        }

        [Fact]
        public async Task OnRefreshDiscountRateIsSet()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(0.1, this.accountFunds.DiscountRate);
        }

        [Fact]
        public async Task OnRefreshPointsBalanceIsSet()
        {
            await this.accountFunds.RefreshAsync();
            Assert.Equal(9, this.accountFunds.PointsBalance);
        }

        [Fact]
        public async Task OnRefreshThrowIfBetfairReturnsError()
        {
            var response = new ResponseStub<AccountFundsResponseStub>(new AccountFundsResponseStub()) { Error = "Error" };
            this.handler.WithReturnContent(response);
            this.accountFunds.WithHandler(this.handler.Build());
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => this.accountFunds.RefreshAsync());
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

            this.accountFunds.Dispose();
            this.handler.Dispose();
            this.disposedValue = true;
        }
    }
}
