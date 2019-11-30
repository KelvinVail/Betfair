namespace Betfair.Tests
{
    using System;

    using Xunit;

    public class BetfairFactoryTests
    {
        private readonly BetfairFactory factory = new BetfairFactory("AppKey", "Username", "Password");

        [Fact]
        public void BuildReturnsInstanceOfBetfair()
        {
            Assert.IsType<BetfairClient>(this.factory.Build());
        }

        [Fact]
        public void DefaultTimeoutIsThirtySeconds()
        {
            this.AssertTimeoutIs(30);
        }

        [Fact]
        public void SetTimeoutToTenSeconds()
        {
            this.factory.IdentityTimeout = 10;
            this.AssertTimeoutIs(10);
        }

        [Fact]
        public void IdentityUriIsCorrect()
        {
            Assert.Equal(
                new Uri("https://identitysso-cert.betfair.com/"), 
                this.factory.IdentityBaseAddress);
        }

        [Fact]
        public void AcceptJsonMediaType()
        {
            Assert.Equal("application/json", this.factory.IdentityAcceptMediaType);
        }

        private void AssertTimeoutIs(int timeoutSeconds)
        {
            Assert.Equal(
                timeoutSeconds, 
                this.factory.IdentityTimeout);
        }
    }
}
