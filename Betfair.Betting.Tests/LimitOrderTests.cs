namespace Betfair.Betting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Betfair.Betting.Tests.TestDoubles;
    using Betfair.Betting.Tests.TestDoubles.Requests;
    using Betfair.Betting.Tests.TestDoubles.Responses;
    using Xunit;

    public class LimitOrderTests
    {
        private readonly ExchangeServiceSpy service = new ExchangeServiceSpy();

        private readonly OrderService orderService;

        public LimitOrderTests()
        {
            this.orderService = new OrderService(this.service);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2147483648)]
        public void SelectionIdIsSetInConstructor(long selectionId)
        {
            var order = new LimitOrder(selectionId, Side.Back, 1.01, 1);

            Assert.Equal(selectionId, order.SelectionId);
        }

        [Theory]
        [InlineData(Side.Back)]
        [InlineData(Side.Lay)]
        public void SideCanBeSetInConstructor(Side side)
        {
            var order = new LimitOrder(1, side, 1.01, 1);

            Assert.Equal(side, order.Side);
        }

        [Theory]
        [InlineData(1.99)]
        [InlineData(10.77)]
        [InlineData(3.3333333)]
        public void SizeCanBeSetInConstructor(double size)
        {
            var order = new LimitOrder(1, Side.Back, 1.01, size);

            Assert.Equal(size, order.Size);
        }

        [Theory]
        [InlineData(1.01)]
        [InlineData(1000)]
        [InlineData(3.4567)]
        public void PriceCanBeSetInConstructor(double price)
        {
            var order = new LimitOrder(1, Side.Back, price, 1);

            Assert.Equal(price, order.Price);
        }

        [Theory]
        [InlineData(12345, Side.Back, 2.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 2.99, 1.01)]
        public void ToInstructionReturnCorrectJsonString(long selectionId, Side side, double size, double price)
        {
            var order = new LimitOrderBuilder(selectionId, side, price, size);
            var expected = order.ExpectedInstructionJson();

            Assert.Equal(expected, order.Object.ToInstruction());
        }

        [Theory]
        [InlineData(2.999, 3)]
        [InlineData(3.3333333, 3.33)]
        [InlineData(2.555, 2.56)]
        [InlineData(10.6666, 10.67)]
        public void ToInstructionRoundsSizeToTwoDecimalsPlaces(double size, double rounded)
        {
            var order = new LimitOrder(1, Side.Back, 1.01, size);
            var expected = new LimitOrderBuilder(1, Side.Back, 1.01, rounded).ExpectedInstructionJson();

            Assert.Equal(expected, order.ToInstruction());
        }

        [Theory]
        [InlineData(1, 1.01)]
        [InlineData(0.01, 1.01)]
        [InlineData(1.009, 1.01)]
        [InlineData(1.015, 1.01)]
        [InlineData(1.016, 1.02)]
        [InlineData(2.01, 2)]
        [InlineData(2.011, 2.02)]
        [InlineData(3.025, 3)]
        [InlineData(3.026, 3.05)]
        [InlineData(4.05, 4)]
        [InlineData(4.06, 4.1)]
        [InlineData(6.1, 6)]
        [InlineData(6.11, 6.2)]
        [InlineData(10.25, 10)]
        [InlineData(10.26, 10.5)]
        [InlineData(20.5, 20)]
        [InlineData(20.6, 21)]
        [InlineData(31, 30)]
        [InlineData(31.1, 32)]
        [InlineData(52.5, 50)]
        [InlineData(52.6, 55)]
        [InlineData(105, 100)]
        [InlineData(105.1, 110)]
        [InlineData(1001, 1000)]
        [InlineData(1000.1, 1000)]
        [InlineData(1000.00001, 1000)]
        [InlineData(9999, 1000)]
        public void PriceIsRoundedToNearestValidPrice(double price, double expectedPrice)
        {
            var order = new LimitOrder(1, Side.Back, price, 2);
            var expected = new LimitOrderBuilder(1, Side.Back, expectedPrice, 2).ExpectedInstructionJson();

            Assert.Equal(expected, order.ToInstruction());
        }

        [Theory]
        [InlineData(2.00, 1.01, "SUCCESS")]
        [InlineData(9.99, 1000, "FAILURE")]
        public async Task ResultsAreSet(double size, double price, string status)
        {
            var orders = new PlaceExecutionReportStub("MarketId", "SUCCESS");
            orders.AddReport(new LimitOrderBuilder(98765, Side.Lay, -1, -1), "1", "SUCCESS", "EXECUTION_COMPLETE");

            var order = new LimitOrderBuilder(12345, Side.Back, price, size);
            orders.AddReport(order, "2", status, "EXECUTION_COMPLETE");

            orders.SetReturnContent(this.service);

            await this.orderService.Place("MarketId", orders.LimitOrders);

            Assert.NotEqual("1", order.Object.BetId);
            Assert.Equal("2", order.Object.BetId);
            Assert.Equal(size, order.Object.SizeMatched);
            Assert.Equal(price, order.Object.AveragePriceMatched);
            Assert.Equal(status, order.Object.Status);
            Assert.Equal("EXECUTION_COMPLETE", order.Object.OrderStatus);
            Assert.Equal(DateTime.Parse("2013-10-30T14:22:47.000Z", new DateTimeFormatInfo()), order.Object.PlacedDate);
        }

        [Fact]
        public async Task SetResultsShouldHandleMissingReport()
        {
            var orders = new PlaceExecutionReportStub("MarketId", "SUCCESS");
            orders.AddReport(new LimitOrderBuilder(98765, Side.Lay, 1.01, 2.00), "1", "SUCCESS", "EXECUTION_COMPLETE");

            var order = new LimitOrderBuilder(12345, Side.Back, 1.01, 2.00);
            orders.AddNullReport(order);

            orders.SetReturnContent(this.service);

            await this.orderService.Place("MarketId", orders.LimitOrders);

            Assert.Equal(0, order.Object.SizeMatched);
        }

        [Fact]
        public async Task SetResultsShouldHandleNullReport()
        {
            var limitOrders = new List<LimitOrder> { new LimitOrder(98765, Side.Lay, 1.01, 2.00) };
            await this.orderService.Place("MarketId", limitOrders);
        }

        [Theory]
        [InlineData(2.00)]
        [InlineData(9.99)]
        public async Task SetResultsShouldUseCorrectResult(double size)
        {
            var order = new LimitOrderBuilder(12345, Side.Back, 1.01, size);
            var limitOrders = new List<LimitOrderBuilder>
            {
                new LimitOrderBuilder(98765, Side.Lay, -1, -1),
                new LimitOrderBuilder(12345, Side.Lay, -1, -1),
                order,
            };
            await this.SetResults(limitOrders, "SUCCESS");
            Assert.Equal(size, order.Object.SizeMatched);
        }

        [Theory]
        [InlineData(12345, Side.Back, 1.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 1.99, 3)]
        public async Task ToCancelInstructionReturnCorrectJsonString(long selectionId, Side side, double size, double price)
        {
            var order = new LimitOrderBuilder(selectionId, side, price, size);
            await this.SetResults(new List<LimitOrderBuilder> { order }, "SUCCESS", "EXECUTABLE");
            var expected = $"{{\"betId\":\"{order.Object.BetId}\"}}";
            Assert.Equal(expected, order.Object.ToCancelInstruction());
        }

        [Theory]
        [InlineData(12345, Side.Back, 1.99, 1.01)]
        [InlineData(98765, Side.Lay, 10.99, 1000)]
        [InlineData(2147483648, Side.Back, 7.24, 3.5)]
        [InlineData(12345, Side.Lay, 1.99, 3)]
        public async Task ToCancelInstructionShouldReturnNullIfOrderIsComplete(long selectionId, Side side, double size, double price)
        {
            var order = new LimitOrderBuilder(selectionId, side, price, size);
            await this.SetResults(new List<LimitOrderBuilder> { order }, "SUCCESS");
            Assert.Null(order.Object.ToCancelInstruction());
        }

        [Theory]
        [InlineData(1.99, 1.01, 2, 1000)]
        [InlineData(2, 1.01, 2, 1.01)]
        [InlineData(1.50, 2.2, 2, 1000)]
        [InlineData(2, 2.2, 2, 2.2)]
        [InlineData(1.99, 5, 2, 1000)]
        [InlineData(2, 5, 2, 5)]
        [InlineData(1.97, 5.1, 1.97, 5.1)]
        [InlineData(1.96, 5.1, 2, 1000)]
        [InlineData(1, 10, 1, 10)]
        [InlineData(0.99, 10, 2, 1000)]
        [InlineData(0.10, 100, 0.10, 100)]
        [InlineData(0.09, 100, 2, 1000)]
        [InlineData(0.01, 1000, 0.01, 1000)]
        public void HandleBelowMinimumStakeForBackOrders(double size, double price, double expectedSize, double expectedPrice)
        {
            var order = new LimitOrder(12345, Side.Back, price, size);
            var expected = new LimitOrderBuilder(12345, Side.Back, expectedPrice, expectedSize).ExpectedInstructionJson();
            Assert.Equal(expected, order.ToInstruction());
        }

        [Theory]
        [InlineData(1.99, 1.02, 2, 1.02)]
        [InlineData(2, 1.01, 2, 1.01)]
        [InlineData(1.50, 2.2, 2, 1.02)]
        [InlineData(2, 2.2, 2, 2.2)]
        [InlineData(1.99, 5, 2, 1.02)]
        [InlineData(2, 5, 2, 5)]
        [InlineData(1.97, 5.1, 1.97, 5.1)]
        [InlineData(1.96, 5.1, 2, 1.02)]
        [InlineData(1, 10, 1, 10)]
        [InlineData(0.10, 100, 0.10, 100)]
        [InlineData(0.01, 1000, 0.01, 1000)]
        public void HandleBelowMinimumStakeForLayOrders(double size, double price, double expectedSize, double expectedPrice)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            var expected = new LimitOrderBuilder(12345, Side.Lay, expectedPrice, expectedSize).ExpectedInstructionJson();
            Assert.Equal(expected, order.ToInstruction());
        }

        [Theory]
        [InlineData(0.01, 3.2, 3)]
        [InlineData(0.02, 2.8, 2)]
        [InlineData(0.03, 1.7, 1.67)]
        [InlineData(0.11, 1.2, 1.19)]
        [InlineData(0.7, 1.05, 1.03)]
        [InlineData(1, 1.05, 1.02)]
        [InlineData(1.01, 1.05, 1.02)]
        [InlineData(1.99, 3, 1.02)]
        public void InitialPriceUsedForVerySmallLaysShouldReturnAtLeastOnePenceProfit(double size, double price, double initialPrice)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            var expected = new LimitOrderBuilder(12345, Side.Lay, initialPrice, 2).ExpectedInstructionJson();
            Assert.Equal(expected, order.ToInstruction());
        }

        [Theory]
        [InlineData(0.01, 2.2)]
        [InlineData(0.02, 1.8)]
        [InlineData(0.03, 1.5)]
        [InlineData(0.11, 1.15)]
        [InlineData(0.7, 1.02)]
        [InlineData(1, 1.01)]
        [InlineData(1.01, 1.01)]
        [InlineData(1.99, 1.01)]
        public void ReturnNullIfOrderIsBelowAbsoluteMinimum(double size, double price)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            Assert.Null(order.ToInstruction());
        }

        [Theory]
        [InlineData(1.1, 0.05, 0.1)]
        [InlineData(1.06, 0.08, 0.17)]
        [InlineData(1.05, 0.1, 0.2)]
        [InlineData(1.5, 0.01, 0.02)]
        [InlineData(1.01, 0.01, 1)]
        public void LayOrderPlacedAtBelowMinimumShouldBeAdjustedToReturnAtLeastOnePenceProfit(double price, double size, double expectedSize)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            Assert.Equal(expectedSize, order.Size);

            var backOrder = new LimitOrder(12345, Side.Back, price, size);
            Assert.Equal(size, backOrder.Size);
        }

        [Theory]
        [InlineData(1.99, 2, true)]
        [InlineData(1.99, 1.01, true)]
        [InlineData(2, 1.01, false)]
        [InlineData(1.50, 2.2, true)]
        [InlineData(2, 2.2, false)]
        [InlineData(1.99, 5, true)]
        [InlineData(2, 5, false)]
        [InlineData(1.97, 5.1, false)]
        [InlineData(1.96, 5.1, true)]
        [InlineData(1, 10, false)]
        [InlineData(0.99, 10, true)]
        [InlineData(0.10, 100, false)]
        [InlineData(0.09, 100, true)]
        [InlineData(0.01, 1000, false)]
        public void BelowMinimumStakeFlagIsSet(double size, double price, bool expected)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            Assert.Equal(expected, order.BelowMinimumStake);
        }

        [Theory]
        [InlineData(1.99, 2, 0.01)]
        [InlineData(1.90, 2, 0.10)]
        [InlineData(1.75, 2, 0.25)]
        [InlineData(1.50, 2, 0.50)]
        [InlineData(1, 2, 1)]
        [InlineData(0.5, 2, 1.5)]
        [InlineData(0.01, 2, 1.99)]
        [InlineData(0.01, 6, 1.99)]
        [InlineData(0.01, 7.2, 1.99)]
        public void IfBelowMinimumStakeThenToBelowMinimumCancelInstructionShouldBeSet(double size, double price, double reduction)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            var expected = $"{{\"betId\":\"{order.BetId}\",\"sizeReduction\":{reduction}}}";
            Assert.Equal(expected, order.ToBelowMinimumCancelInstruction());
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 5)]
        [InlineData(1.43, 7)]
        [InlineData(1, 10)]
        [InlineData(0.1, 100)]
        public void IfAboveMinimumStakeToBelowMinimumInstructionsShouldBeNull(double size, double price)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            Assert.Null(order.ToBelowMinimumCancelInstruction());
            Assert.Null(order.ToBelowMinimumReplaceInstruction());
        }

        [Theory]
        [InlineData(1.99, 2)]
        [InlineData(1.90, 2)]
        [InlineData(1.75, 2)]
        [InlineData(1.50, 2)]
        [InlineData(1, 2)]
        [InlineData(0.5, 2)]
        [InlineData(0.01, 2)]
        public void IfBelowMinimumStakeThenToBelowMinimumReplaceInstructionShouldBeSet(double size, double price)
        {
            var order = new LimitOrder(12345, Side.Lay, price, size);
            var expected = $"{{\"betId\":\"{order.BetId}\",\"newPrice\":{price}}}";
            Assert.Equal(expected, order.ToBelowMinimumReplaceInstruction());
        }

        [Fact]
        public async Task BetIdIsUpdatedWhenOrderIsReplaced()
        {
            var order = new LimitOrderBuilder(12345, Side.Lay, 2, 0.5);
            await this.SetResults(new List<LimitOrderBuilder> { order }, "SUCCESS");
            Assert.Equal("2", order.Object.BetId);
        }

        [Fact]
        public async Task SetResultsShouldUpdateCorrectBetId()
        {
            var order1 = new LimitOrderBuilder(12345, Side.Back, 1.01, 0.5); // betId = 1, newId = 4
            var order2 = new LimitOrderBuilder(98765, Side.Lay, 2, 0.5); // betId = 2, newId = 5
            var order3 = new LimitOrderBuilder(12345, Side.Lay, 3, 0.5); // betId = 3, newId = 6
            var limitOrders = new List<LimitOrderBuilder>
            {
                order1,
                order2,
                order3,
            };
            await this.SetResults(limitOrders, "SUCCESS");
            Assert.Equal("4", order1.Object.BetId);
            Assert.Equal("5", order2.Object.BetId);
            Assert.Equal("6", order3.Object.BetId);
        }

        [Theory]
        [InlineData(12345, Side.Back, 2, 1.01)]
        [InlineData(11111, Side.Lay, 2, 2)]
        [InlineData(98765, Side.Back, 2, 3.5)]
        public async Task CancelSendsCancelInstruction(long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrderBuilder(selectionId, side, price, size);
            var limitOrder2 = new LimitOrderBuilder(1, Side.Back, 2, 10);
            var limitOrders = new List<LimitOrderBuilder> { limitOrder, limitOrder2 };
            await this.SetResults(limitOrders, "SUCCESS", "EXECUTABLE");
            var cancelInstruction = $"{{\"marketId\":\"MarketId\",\"instructions\":[{limitOrder.Object.ToCancelInstruction()},{limitOrder2.Object.ToCancelInstruction()}]}}";
            await this.orderService.Cancel("MarketId", limitOrders.Select(o => o.Object.BetId).ToList());
            Assert.Equal(cancelInstruction, this.service.SentParameters["cancelOrders"]);
        }

        [Fact]
        public async Task AddReportShouldHandleNullReport()
        {
            var limitOrder = new LimitOrderBuilder(123, Side.Lay, 2.5, 9.99);
            var instruction = new LimitOrderBuilder(987, Side.Back, 5.5, 11.99).PlaceInstructionReportJson("1", "SUCCESS", "SUCCESS");
            this.SetPlaceReturnContent(instruction);
            await this.orderService.Place("MarketId", new List<LimitOrder> { limitOrder.Object });
        }

        [Fact]
        public async Task ErrorCodeShouldBeRecordedIfOrderFails()
        {
            var order = new LimitOrderBuilder(12345, Side.Back, 2, 9.99);
            var limitOrders = new List<LimitOrderBuilder> { order };
            await this.SetResults(limitOrders, "FAILURE");

            Assert.Equal("TEST_ERROR", order.Object.ErrorCode);
        }

        private static string GetReplaceReport(LimitOrder limitOrder, string status, long newBetId, long originalBetId, string orderStatus)
        {
            return $"{{\"status\":\"{status}\"," +
                   "\"cancelInstructionReport\":" +
                   $"{{\"status\":\"{status}\"," +
                   "\"instruction\":" +
                   $"{{\"betId\":\"{originalBetId}\"}}," +
                   $"\"sizeCancelled\":{limitOrder.Size}," +
                   "\"cancelledDate\":\"2020-05-09T19:53:50.000Z\"}," +
                   "\"placeInstructionReport\":" +
                   $"{{\"status\":\"{status}\"," +
                   "\"instruction\":" +
                   $"{{\"selectionId\":{limitOrder.SelectionId}," +
                   "\"limitOrder\":" +
                   $"{{\"size\":{limitOrder.Size}," +
                   $"\"price\":{limitOrder.Price}," +
                   "\"persistenceType\":\"LAPSE\"}," +
                   "\"orderType\":\"LIMIT\"," +
                   $"\"side\":\"{limitOrder.Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"}}," +
                   $"\"betId\":\"{newBetId}\"," +
                   "\"placedDate\":\"2020-05-09T19:53:50.000Z\"," +
                   "\"averagePriceMatched\":0.0," +
                   "\"sizeMatched\":0.0," +
                   $"\"orderStatus\":\"{orderStatus}\"}}}}";
        }

        private async Task SetResults(List<LimitOrderBuilder> placeInstructions, string status, string orderStatus = "EXECUTION_COMPLETE")
        {
            var instructions = string.Empty;
            var betId = 0;
            foreach (var instruction in placeInstructions)
            {
                betId++;
                instructions += instruction.PlaceInstructionReportJson(betId.ToString(new CultureInfo(1)), status, orderStatus) + ",";
            }

            instructions = instructions.Remove(instructions.Length - 1, 1);

            this.SetPlaceReturnContent(instructions);

            var replaceInstructions = string.Empty;
            var originalBetId = 0;
            foreach (var limitOrder in placeInstructions.Select(p => p.Object).Where(o => o.BelowMinimumStake))
            {
                betId++;
                originalBetId++;
                replaceInstructions += GetReplaceReport(limitOrder, orderStatus, betId, originalBetId, orderStatus) + ",";
            }

            if (placeInstructions.Select(p => p.Object).Any(o => o.BelowMinimumStake))
            {
                replaceInstructions = replaceInstructions.Remove(replaceInstructions.Length - 1, 1);

                this.SetReplaceReturnContent(replaceInstructions);
            }

            await this.orderService.Place("MarketId", placeInstructions.Select(p => p.Object).ToList());
        }

        private void SetPlaceReturnContent(string instructions)
        {
            this.service.WithReturnContent(
                "placeOrders",
                $"{{\"marketId\":\"MarketId\",\"instructionReports\":[{instructions}], \"status\":\"SUCCESS\"}}");
        }

        private void SetReplaceReturnContent(string replaceInstructions)
        {
            this.service.WithReturnContent(
                "replaceOrders",
                $"{{\"marketId\":\"MarketId\",\"instructionReports\":[{replaceInstructions}], \"status\":\"SUCCESS\"}}");
        }
    }
}
