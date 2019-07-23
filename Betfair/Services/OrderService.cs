namespace Betfair.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Betfair.Entities;
    using Betfair.Services.BetfairApi;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    /// <inheritdoc/>
    public class OrderService : IOrderService
    {
        /// <summary>
        /// The place order client.
        /// </summary>
        private readonly IBetfairApiClient betfairApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="betfairApiClient">
        /// The betfair api client.
        /// </param>
        internal OrderService(IBetfairApiClient betfairApiClient)
        {
            this.betfairApiClient = betfairApiClient;
        }

        /// <inheritdoc/>
        public async Task PlaceOrdersAsync(OrderBook orderBook)
        {
            var placeResponse = await this.betfairApiClient.PlaceOrders(orderBook.PlaceOrdersRequest());

            if (placeResponse.Result.Status != ExecutionReportStatus.SUCCESS)
            {
                var failed = placeResponse.Result.ErrorCode.ToString();
                return;
            }

            if (!orderBook.HasOrdersBelowMinimum)
            {
                foreach (var order in orderBook.Orders)
                {
                    order.Placed = true;
                }

                return;
            }

            var cancelRequest = GetCancelOrdersRequest(placeResponse, orderBook);
            var cancelResponse = await this.betfairApiClient.CancelOrders(cancelRequest);
            if (cancelResponse.Result.Status != ExecutionReportStatus.SUCCESS)
            {
                var cancelFail = cancelResponse.Result.ErrorCode.ToString();
            }

            var replaceRequest = GetReplaceOrdersRequest(placeResponse, orderBook);
            var replaceResponse = await this.betfairApiClient.ReplaceOrders(replaceRequest);
            if (replaceResponse.Result.Status != ExecutionReportStatus.SUCCESS)
            {
                var replaceFail = replaceResponse.Result.ErrorCode.ToString();
            }

            var result = GetPlaceInstructionReports(placeResponse, replaceResponse);

            return;
        }

        /// <summary>
        /// The cancel orders request.
        /// </summary>
        /// <param name="placeResponse">
        /// The place response.
        /// </param>
        /// <param name="orderBook">
        /// The orderBook.
        /// </param>
        /// <returns>
        /// The <see cref="CancelOrdersRequest"/>.
        /// </returns>
        private static CancelOrdersRequest GetCancelOrdersRequest(PlaceOrdersResponse placeResponse, OrderBook orderBook)
        {
            var belowOrders = orderBook.Orders.Where(w => w.IsStakeBelowMinimum).ToList();

            var cancelInstructions = new List<CancelInstruction>();
            foreach (var order in belowOrders)
            {
                var instruction = new CancelInstruction()
                                      {
                                          BetId = placeResponse.Result.InstructionReports.Where(w => w.Instruction.SelectionId == order.SelectionId && w.Instruction.Side == order.Side).Select(s => s.BetId).First(),
                                          SizeReduction = 2 - order.Size
                                      };
                cancelInstructions.Add(instruction);
            }

            var cancelOrders = new CancelOrders()
                                   {
                                       MarketId = placeResponse.Result.MarketId,
                                       Instructions = cancelInstructions.Where(w => w.SizeReduction > 0).ToList()
                                   };

            return new CancelOrdersRequest()
                       {
                           Params = cancelOrders
                       };
        }

        /// <summary>
        /// The get replace orders request.
        /// </summary>
        /// <param name="placeResponse">
        /// The place response.
        /// </param>
        /// <param name="orderBook">
        /// The orderBook.
        /// </param>
        /// <returns>
        /// The <see cref="ReplaceOrdersRequest"/>.
        /// </returns>
        private static ReplaceOrdersRequest GetReplaceOrdersRequest(
            PlaceOrdersResponse placeResponse,
            OrderBook orderBook)
        {
            var belowOrders = orderBook.Orders.Where(w => w.IsStakeBelowMinimum).ToList();

            var replaceInstructions = new List<ReplaceInstruction>();
            foreach (var order in belowOrders)
            {
                var instruction = new ReplaceInstruction()
                                      {
                                          BetId = placeResponse.Result.InstructionReports.Where(w => w.Instruction.SelectionId == order.SelectionId && w.Instruction.Side == order.Side).Select(s => s.BetId).First(),
                                          NewPrice = order.Price
                                      };
                replaceInstructions.Add(instruction);
            }

            var replaceOrders = new ReplaceOrders()
                                    {
                                        MarketId = placeResponse.Result.MarketId,
                                        Instructions = replaceInstructions.ToList()
                                    };

            return new ReplaceOrdersRequest()
                       {
                           Params = replaceOrders
                       };
        }

        /// <summary>
        /// The get place instruction reports.
        /// </summary>
        /// <param name="placeResponse">
        /// The place response.
        /// </param>
        /// <param name="replaceResponse">
        /// The replace response.
        /// </param>
        /// <returns>
        /// The <see cref="List{PlaceInstructionReport}"/>.
        /// </returns>
        private static List<PlaceInstructionReport> GetPlaceInstructionReports(
            PlaceOrdersResponse placeResponse,
            ReplaceOrdersResponse replaceResponse)
        {
            var replaceList = replaceResponse.Result.InstructionReports.Select(s => s.PlaceInstructionReport)
                .ToList();
            var placeList = placeResponse.Result.InstructionReports.ToList();
            var completeList = placeList.Where(w => replaceList.All(a => a.Instruction.SelectionId != w.Instruction.SelectionId && a.Instruction.Side != w.Instruction.Side)).ToList();
            completeList.AddRange(replaceList);

            return completeList;
        }
    }
}
