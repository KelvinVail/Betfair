namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public class StrategySpy : IStrategy
    {
        public string ClocksProcessed { get; private set; }

        public MarketDataFilter DataFilter { get; } = new MarketDataFilter();

        public CancellationToken Token { get; private set; }

        public void WithCancellationToken(CancellationToken token)
        {
            this.Token = token;
        }

        public async Task OnChange(ChangeMessage change)
        {
            this.ClocksProcessed += change?.Clock;
            await Task.CompletedTask;
        }
    }
}
