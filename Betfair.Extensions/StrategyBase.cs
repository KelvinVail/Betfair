namespace Betfair.Extensions
{
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public abstract class StrategyBase
    {
        public abstract MarketDataFilter DataFilter { get; }

        public void AddExchangeService(IExchangeService exchangeStub)
        {
        }

        public async Task OnChange(ChangeMessage change)
        {
        }
    }
}
