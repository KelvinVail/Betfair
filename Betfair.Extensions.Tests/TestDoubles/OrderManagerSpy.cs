namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream.Responses;

    public class OrderManagerSpy : OrderManagerBase
    {
        public OrderManagerSpy(IOrderService orderService)
            : base(orderService)
        {
        }

        public ChangeMessage LastChangeMessage { get; private set; }

        public bool OnMarketCloseCalled { get; private set; }

        public MarketCache MarketCache => this.Market;

        public override async Task OnChange(ChangeMessage change)
        {
            this.LastChangeMessage = change;
            await Task.CompletedTask;
        }

        public override async Task OnMarketClose()
        {
            this.OnMarketCloseCalled = true;
            await Task.CompletedTask;
        }
    }
}
