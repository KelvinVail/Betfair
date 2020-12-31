using System.Threading.Tasks;
using Betfair.Betting;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class OrderManagerSpy : OrderManagerBase
    {
        public OrderManagerSpy(IOrderService orderService)
            : base(orderService)
        {
        }

        public ChangeMessage LastChangeMessage { get; private set; }

        public bool OnMarketCloseCalled { get; private set; }

        public MarketCache MarketCache => Market;

        public override async Task OnChange(ChangeMessage change)
        {
            LastChangeMessage = change;
            await Task.CompletedTask;
        }

        public override async Task OnMarketClose()
        {
            await Task.Delay(100);
            OnMarketCloseCalled = true;
        }

        public bool CanAccessOrderService()
        {
            OrderService.CancelAll("Test");
            return true;
        }
    }
}
