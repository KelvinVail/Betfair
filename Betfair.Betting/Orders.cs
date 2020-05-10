namespace Betfair.Betting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;

    public sealed class Orders
    {
        private readonly IExchangeService service;

        private readonly List<LimitOrder> orders = new List<LimitOrder>();

        public Orders(IExchangeService service, string marketId, string strategyRef = null)
        {
            this.service = ValidateExchangeService(service);
            this.MarketId = ValidateMarketId(marketId);
            this.StrategyRef = ValidateStrategyRef(strategyRef);
        }

        public string StrategyRef { get; }

        public string MarketId { get; }

        public bool Placed { get; private set; }

        public void Add(LimitOrder limitOrder)
        {
            this.orders.Add(limitOrder);
        }

        public async Task PlaceAsync()
        {
            this.ValidateOrders();
            var report = await this.service.SendAsync<PlaceReport>("betting", "placeOrders", this.Params());
            this.UpdateOrders(report);
            if (this.orders.Any(o => o.BelowMinimumStake))
            {
                await this.service.SendAsync<PlaceReport>("betting", "cancelOrders", this.BelowMinimumCancelParams());
                var replaceReport = await this.service.SendAsync<ReplaceReport>("betting", "replaceOrders", this.BelowMinimumReplaceParams());
                this.UpdateOrders(replaceReport);
            }

            this.Placed = true;
        }

        public async Task CancelAsync()
        {
            if (this.orders.Any(o => o.OrderStatus == "EXECUTABLE"))
                await this.service.SendAsync<PlaceReport>("betting", "cancelOrders", this.CancelParams());
        }

        private static IExchangeService ValidateExchangeService(IExchangeService service)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service), "ExchangeService should not be null.");
            return service;
        }

        private static string ValidateMarketId(string marketId)
        {
            if (string.IsNullOrEmpty(marketId))
                throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
            return marketId;
        }

        private static string ValidateStrategyRef(string strategyRef)
        {
            if (string.IsNullOrEmpty(strategyRef)) return null;
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
            return strategyRef;
        }

        private string Params()
        {
            return $"{{\"marketId\":\"{this.MarketId}\",{this.Reference()}\"instructions\":[{this.Instructions()}]}}";
        }

        private string Instructions()
        {
            var instructions = string.Empty;
            this.orders.ForEach(i => instructions += i.ToInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private string CancelParams()
        {
            return $"{{\"marketId\":\"{this.MarketId}\",\"instructions\":[{this.CancelInstructions()}]}}";
        }

        private string CancelInstructions()
        {
            var cancelInstructions = string.Empty;
            this.orders.Where(o => o.ToCancelInstruction() != null).ToList().ForEach(i => cancelInstructions += i.ToCancelInstruction() + ",");
            return cancelInstructions.Remove(cancelInstructions.Length - 1, 1);
        }

        private string BelowMinimumCancelParams()
        {
            return $"{{\"marketId\":\"{this.MarketId}\",\"instructions\":[{this.BelowMinimumCancelInstructions()}]}}";
        }

        private string BelowMinimumCancelInstructions()
        {
            var instructions = string.Empty;
            this.orders.Where(o => o.BelowMinimumStake).ToList().ForEach(i => instructions += i.ToBelowMinimumCancelInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private string BelowMinimumReplaceParams()
        {
            return $"{{\"marketId\":\"{this.MarketId}\",{this.Reference()}\"instructions\":[{this.BelowMinimumReplaceInstructions()}]}}";
        }

        private string BelowMinimumReplaceInstructions()
        {
            var instructions = string.Empty;
            this.orders.Where(o => o.BelowMinimumStake).ToList().ForEach(i => instructions += i.ToBelowMinimumReplaceInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private string Reference()
        {
            return this.StrategyRef is null ? null : $"\"customerStrategyRef\":\"{this.StrategyRef}\",";
        }

        private void ValidateOrders()
        {
            if (!this.orders.Any()) throw new InvalidOperationException("Does not contain any orders.");
        }

        private void UpdateOrders(PlaceReport reports)
        {
            if (reports is null) return;
            this.orders.ForEach(o => o.AddReports(reports.InstructionReports));
        }

        private void UpdateOrders(ReplaceReport reports)
        {
            if (reports is null) return;
            var r = reports.InstructionReports.Select(i => i.PlaceInstructionReport).ToList();
            this.orders.ForEach(o => o.AddReports(r));
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
