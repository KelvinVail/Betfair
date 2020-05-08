namespace Betfair.Betting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;

    public sealed class Orders
    {
        private readonly IExchangeService service;

        private readonly List<LimitOrder> orders = new List<LimitOrder>();

        public Orders(IExchangeService service, string marketId)
        {
            this.service = service;
            this.MarketId = ValidateMarketId(marketId);
        }

        public Orders(IExchangeService service, string marketId, string strategyRef)
        {
            this.service = service;
            this.MarketId = ValidateMarketId(marketId);
            if (string.IsNullOrEmpty(strategyRef)) return;
            this.StrategyRef = ValidateStrategyReference(strategyRef);
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
            this.Validate();
            var results = await this.service.SendAsync<PlaceResult>("betting", "placeOrders", this.ToParams());
            this.Placed = true;
            this.UpdateOrders(results);
        }

        private static string ValidateStrategyReference(string strategyRef)
        {
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
            return strategyRef;
        }

        private static string ValidateMarketId(string marketId)
        {
            if (string.IsNullOrEmpty(marketId))
                throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
            return marketId;
        }

        private string ToParams()
        {
            return $"{{\"marketId\":\"{this.MarketId}\",{this.Reference()}\"instructions\":[{this.Instructions()}]}}";
        }

        private string Instructions()
        {
            var instructions = string.Empty;
            this.orders.ForEach(i => instructions += i.ToInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private string Reference()
        {
            return this.StrategyRef is null ? null : $"\"customerStrategyRef\":\"{this.StrategyRef}\",";
        }

        private void Validate()
        {
            if (!this.orders.Any()) throw new InvalidOperationException("Does not contain any orders.");
        }

        private void UpdateOrders(PlaceResult results)
        {
            if (results is null) return;
            this.orders.ForEach(o => o.AddReports(results.InstructionReports));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        [DataContract]
        private sealed class PlaceResult
        {
            [DataMember(Name = "instructionReports", EmitDefaultValue = false)]
            internal List<InstructionReport> InstructionReports { get; set; }
        }
    }
}
