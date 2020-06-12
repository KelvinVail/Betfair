namespace Betfair.Betting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;

    public sealed class OrderService
    {
        private readonly IExchangeService exchange;

        public OrderService(IExchangeService exchange)
        {
            this.exchange = ValidateExchangeService(exchange);
        }

        public async Task Place(string marketId, List<LimitOrder> orders, string strategyRef = null)
        {
            ValidateMarketId(marketId);
            ValidateStrategyRef(strategyRef);
            ValidateOrders(orders);
            await this.PlaceOrders(marketId, orders, strategyRef);
            await this.PlaceBelowMinimumOrders(marketId, orders, strategyRef);
        }

        public async Task Cancel(string marketId, List<LimitOrder> orders)
        {
            if (orders.Any(o => o.OrderStatus == "EXECUTABLE"))
                await this.exchange.SendAsync<PlaceReport>("betting", "cancelOrders", CancelParams(marketId, orders));
        }

        private static IExchangeService ValidateExchangeService(IExchangeService exchange)
        {
            if (exchange is null)
                throw new ArgumentNullException(nameof(exchange), "ExchangeService should not be null.");
            return exchange;
        }

        private static void ValidateMarketId(string marketId)
        {
            if (string.IsNullOrEmpty(marketId))
                throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
        }

        private static void ValidateStrategyRef(string strategyRef)
        {
            if (string.IsNullOrEmpty(strategyRef)) return;
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
        }

        private static void ValidateOrders(List<LimitOrder> orders)
        {
            if (!orders.Any()) throw new InvalidOperationException("Does not contain any orders.");
        }

        private static string Instructions(List<LimitOrder> orders)
        {
            var instructions = string.Empty;
            orders.ForEach(i => instructions += i.ToInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private static void UpdateOrders(PlaceReport reports, List<LimitOrder> orders)
        {
            if (reports is null) return;
            orders.ForEach(o => o.AddReports(reports.InstructionReports));
        }

        private static string Params(string marketId, List<LimitOrder> orders, string strategyRef = null)
        {
            return $"{{\"marketId\":\"{marketId}\",{Reference(strategyRef)}\"instructions\":[{Instructions(orders)}]}}";
        }

        private static string BelowMinimumCancelParams(string marketId, IEnumerable<LimitOrder> orders)
        {
            return $"{{\"marketId\":\"{marketId}\",\"instructions\":[{BelowMinimumCancelInstructions(orders)}]}}";
        }

        private static string BelowMinimumCancelInstructions(IEnumerable<LimitOrder> orders)
        {
            var instructions = string.Empty;
            orders.Where(o => o.BelowMinimumStake).ToList().ForEach(i => instructions += i.ToBelowMinimumCancelInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private static string BelowMinimumReplaceParams(string marketId, IEnumerable<LimitOrder> orders, string strategyRef)
        {
            return $"{{\"marketId\":\"{marketId}\",{Reference(strategyRef)}\"instructions\":[{BelowMinimumReplaceInstructions(orders)}]}}";
        }

        private static string BelowMinimumReplaceInstructions(IEnumerable<LimitOrder> orders)
        {
            var instructions = string.Empty;
            orders.Where(o => o.BelowMinimumStake).ToList().ForEach(i => instructions += i.ToBelowMinimumReplaceInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private static void UpdateOrders(ReplaceReport reports, List<LimitOrder> orders)
        {
            if (reports is null) return;
            var r = reports.InstructionReports.Select(i => i.PlaceInstructionReport).ToList();
            orders.ForEach(o => o.AddReports(r));
        }

        private static string Reference(string strategyRef)
        {
            return strategyRef is null ? null : $"\"customerStrategyRef\":\"{strategyRef}\",";
        }

        private static string CancelParams(string marketId, IEnumerable<LimitOrder> orders)
        {
            return $"{{\"marketId\":\"{marketId}\",\"instructions\":[{CancelInstructions(orders)}]}}";
        }

        private static string CancelInstructions(IEnumerable<LimitOrder> orders)
        {
            var cancelInstructions = string.Empty;
            orders.Where(o => o.ToCancelInstruction() != null).ToList().ForEach(i => cancelInstructions += i.ToCancelInstruction() + ",");
            return cancelInstructions.Remove(cancelInstructions.Length - 1, 1);
        }

        private async Task PlaceOrders(string marketId, List<LimitOrder> orders, string strategyRef)
        {
            var report = await this.exchange.SendAsync<PlaceReport>("betting", "placeOrders", Params(marketId, orders, strategyRef));
            UpdateOrders(report, orders);
        }

        private async Task PlaceBelowMinimumOrders(string marketId, List<LimitOrder> orders, string strategyRef)
        {
            if (orders.Any(o => o.BelowMinimumStake))
            {
                await this.exchange.SendAsync<PlaceReport>("betting", "cancelOrders", BelowMinimumCancelParams(marketId, orders));
                var replaceReport =
                    await this.exchange.SendAsync<ReplaceReport>("betting", "replaceOrders", BelowMinimumReplaceParams(marketId, orders, strategyRef));
                UpdateOrders(replaceReport, orders);
            }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        [DataContract]
        private sealed class PlaceReport
        {
            [DataMember(Name = "instructionReports", EmitDefaultValue = false)]
            internal List<InstructionReport> InstructionReports { get; set; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        [DataContract]
        private sealed class ReplaceReport
        {
            [DataMember(Name = "instructionReports", EmitDefaultValue = false)]
            public List<ReplaceInstructionReport> InstructionReports { get; set; }
        }
    }
}
