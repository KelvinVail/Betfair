namespace Betfair.Tests.Account
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Account;
    using Betfair.Tests.Account.TestDoubles;
    using Betfair.Tests.TestDoubles;
    using Newtonsoft.Json;
    using Xunit;

    public class StatementTests : IDisposable
    {
        private readonly Statement statement;

        private readonly SessionSpy session = new SessionSpy();

        private readonly HttpMessageHandlerMock handler = new HttpMessageHandlerMock();

        public StatementTests()
        {
            var responseContent = new AccountStatementResponseStub();
            responseContent.AddItem("refId", DateTime.Parse("2020-01-26", new DateTimeFormatInfo()), 0, 0);
            var response = new ResponseStub<AccountStatementResponseStub>(responseContent);
            this.handler.WithReturnContent(response);
            this.statement = new Statement(this.session);
            this.statement.WithHandler(this.handler.Build());
        }

        [Fact]
        public void StatementIsSealed()
        {
            Assert.True(typeof(Statement).IsSealed);
        }

        [Fact]
        public async Task OnRefreshGetTokenOnSessionIsCalledAsync()
        {
            await this.statement.RefreshAsync();
            Assert.Equal(1, this.session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnRefreshPostRequestIsMade()
        {
            await this.statement.RefreshAsync();
            this.handler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnRefreshAccountUriIsCalled()
        {
            await this.statement.RefreshAsync();
            this.handler.VerifyRequestUri(new Uri("https://api.betfair.com/exchange/account/json-rpc/v1"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnRefreshAppKeyIsInRequestHeader(string appKey)
        {
            this.session.AppKey = appKey;
            await this.statement.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnRefreshSessionTokenIsInRequestHeader(string sessionToken)
        {
            this.session.SessionToken = sessionToken;
            await this.statement.RefreshAsync();
            this.handler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnRefreshRequestContentIsSet()
        {
            await this.statement.RefreshAsync();
            this.handler.VerifyRequestContent(JsonConvert.SerializeObject(new GetAccountStatementRequestStub()));
        }

        [Fact]
        public async Task OnRefreshStatementItemRefIdIsSet()
        {
            await this.statement.RefreshAsync();
            Assert.Equal("refId", this.statement.Items[0].RefId);
        }

        [Fact]
        public async Task OnRefreshStatementItemDateIsSet()
        {
            await this.statement.RefreshAsync();
            Assert.Equal(DateTime.Parse("2020-01-26", new DateTimeFormatInfo()), this.statement.Items[0].ItemDate);
        }

        [Fact]
        public async Task OnRefreshStatementItemAmountIsSet()
        {
            await this.statement.RefreshAsync();
            Assert.Equal(0, this.statement.Items[0].Amount);
        }

        [Fact]
        public async Task OnRefreshStatementItemBalanceIsSet()
        {
            await this.statement.RefreshAsync();
            Assert.Equal(0, this.statement.Items[0].Balance);
        }

        [Fact]
        public void WhenInitializedItemsIsReadOnlyList()
        {
            Assert.True(this.statement.Items is IReadOnlyList<StatementItem>);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.handler.Dispose();
            this.statement.Dispose();
        }
    }
}
