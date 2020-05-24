namespace Betfair.Extensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public interface IStrategy
    {
        public MarketDataFilter DataFilter { get; }

        public void WithCancellationToken(CancellationToken token);

        public Task OnChange(ChangeMessage change);
    }
}