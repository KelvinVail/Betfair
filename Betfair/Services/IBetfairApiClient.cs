namespace Betfair.Services
{
    using System.Threading.Tasks;

    using Betfair.Services.BetfairApi;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    /// <summary>
    /// The Betfair API BetfairClient interface.
    /// </summary>
    internal interface IBetfairApiClient
    {
        /// <summary>
        /// Place orders.
        /// </summary>
        /// <param name="placeOrdersRequest">
        /// The place orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<PlaceOrdersResponse> PlaceOrders(PlaceOrdersRequest placeOrdersRequest);

        /// <summary>
        /// Cancel orders.
        /// </summary>
        /// <param name="cancelOrdersRequest">
        /// The cancel orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<CancelOrdersResponse> CancelOrders(CancelOrdersRequest cancelOrdersRequest);

        /// <summary>
        /// Replace orders.
        /// </summary>
        /// <param name="replaceOrdersRequest">
        /// The replace orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<ReplaceOrdersResponse> ReplaceOrders(ReplaceOrdersRequest replaceOrdersRequest);
    }
}