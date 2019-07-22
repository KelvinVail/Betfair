namespace Betfair
{
    using Betfair.Services;

    /// <summary>
    /// The Betfair Client.
    /// </summary>
    public class BetfairClient : IBetfairClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BetfairClient"/> class.
        /// </summary>
        /// <param name="sessionToken">
        /// A valid Betfair session token.
        /// </param>
        /// <param name="appKey">
        /// Your Betfair app key.
        /// </param>
        public BetfairClient(string sessionToken, string appKey)
        {
            var betfairApiClient = new BetfairApiClient(appKey, sessionToken);
            this.OrderService = new OrderService(betfairApiClient);
        }

        /// <inheritdoc/>
        public IOrderService OrderService { get; }
    }
}
