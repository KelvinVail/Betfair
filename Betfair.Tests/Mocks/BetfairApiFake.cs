namespace Betfair.Tests.Mocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Betfair.Services;
    using Betfair.Services.BetfairApi;
    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    /// <summary>
    /// The betfair API fake.
    /// </summary>
    internal class BetfairApiFake : IBetfairApiClient
    {
        /// <summary>
        /// Gets or sets the place order count.
        /// </summary>
        public int PlaceOrderCount { get; set; }

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
            this.PlaceOrderCount++;
            return await Task.Run(() => this.FullMatchedOrderResponse(placeOrdersRequest));
        }

        public async Task<CancelOrdersResponse> CancelOrders(CancelOrdersRequest cancelOrdersRequest)
        {
            return await Task.Run(() => new CancelOrdersResponse());
        }

        public async Task<ReplaceOrdersResponse> ReplaceOrders(ReplaceOrdersRequest replaceOrdersRequest)
        {
            return await Task.Run(() => new ReplaceOrdersResponse());
        }

        /// <summary>
        /// The full matched order response.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PlaceOrdersResponse"/>.
        /// </returns>
        private PlaceOrdersResponse FullMatchedOrderResponse(PlaceOrdersRequest request)
        {
            var betCount = 0;
            var instructionReports = request.Params.Instructions
                .Select(
                    order => new PlaceInstructionReport
                                 {
                                     AveragePriceMatched = order.LimitOrder.Price,
                                     BetId = (betCount++).ToString(),
                                     ErrorCode = InstructionReportErrorCode.SUCCESS,
                                     PlacedDate = DateTime.UtcNow,
                                     Instruction = order,
                                     OrderStatus = OrderStatus.EXECUTION_COMPLETE,
                                     SizeMatched = order.LimitOrder.Size,
                                     Status = InstructionReportStatus.SUCCESS
                                 })
                .ToList();

            return new PlaceOrdersResponse
                       {
                           Id = 1,
                           Jsonrpc = "1",
                           Result = new PlaceExecutionReport
                                        {
                                            CustomerRef = null,
                                            ErrorCode = ExecutionReportErrorCode.SUCCESS,
                                            MarketId = request.Params.MarketId,
                                            Status = ExecutionReportStatus.SUCCESS,
                                            InstructionReports = instructionReports
                                        }
                       };
        }
    }
}
