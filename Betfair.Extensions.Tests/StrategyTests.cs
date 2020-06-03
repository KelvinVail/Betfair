namespace Betfair.Extensions.Tests
{
    using System.Threading.Tasks;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream;
    using Betfair.Stream.Responses;
    using Xunit;

    public class StrategyTests : StrategyBase
    {
        private readonly ExchangeStub exchange = new ExchangeStub();

        public StrategyTests()
        {
            this.AddExchangeService(this.exchange);
        }

        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter().WithBestPrices();

        [Fact]
        public void StrategyBaseIsAbstract()
        {
            Assert.True(typeof(StrategyBase).IsAbstract);
        }

        [Fact]
        public async Task AChangeMessageCanBePassedToTheBase()
        {
            await this.OnChange(new ChangeMessage());
        }
    }
}
