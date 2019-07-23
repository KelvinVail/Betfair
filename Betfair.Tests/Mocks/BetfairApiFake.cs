namespace Betfair.Tests.Mocks
{
    using System.Threading.Tasks;

    using Betfair.Services;
    using Betfair.Services.BetfairApi;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    /// <summary>
    /// The betfair API fake.
    /// </summary>
    internal class BetfairApiFake : IBetfairApiClient
    {
        /// <summary>
        /// The place orders.
        /// </summary>
        /// <param name="placeOrdersRequest">
        /// The place orders request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<PlaceOrdersResponse> PlaceOrders(PlaceOrdersRequest placeOrdersRequest)
        {
            return await Task.Run(() => new PlaceOrdersResponse());
        }

        public async Task<CancelOrdersResponse> CancelOrders(CancelOrdersRequest cancelOrdersRequest)
        {
            return await Task.Run(() => new CancelOrdersResponse());
        }

        public async Task<ReplaceOrdersResponse> ReplaceOrders(ReplaceOrdersRequest replaceOrdersRequest)
        {
            return await Task.Run(() => new ReplaceOrdersResponse());
        }
    }
}
