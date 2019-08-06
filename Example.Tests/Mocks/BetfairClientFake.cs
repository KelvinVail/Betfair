namespace Example.Tests.Mocks
{
    using Betfair;
    using Betfair.Services;

    /// <summary>
    /// The betfair client fake.
    /// </summary>
    public class BetfairClientFake : IBetfairClient
    {
        public BetfairClientFake(IOrderService orderService)
        {
            this.OrderService = orderService;
        }

        /// <summary>
        /// Gets the order service.
        /// </summary>
        public IOrderService OrderService { get; }
    }
}
