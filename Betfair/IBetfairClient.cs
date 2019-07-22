namespace Betfair
{
    using Betfair.Services;

    /// <summary>
    /// The BetfairClient interface.
    /// </summary>
    public interface IBetfairClient
    {
        /// <summary>
        /// Gets the accounts service.
        /// </summary>
        IOrderService OrderService { get; }
    }
}