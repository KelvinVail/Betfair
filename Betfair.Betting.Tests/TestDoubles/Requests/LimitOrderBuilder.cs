namespace Betfair.Betting.Tests.TestDoubles.Requests
{
    using System.Runtime.Serialization;
    using Betfair.Betting.Tests.TestDoubles.Responses;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public class LimitOrderBuilder
    {
        public LimitOrderBuilder(long selectionId, Side side, double price, double size)
        {
            this.SelectionId = selectionId;
            this.Side = side;
            this.LimitOrder = new LimitOrderStub(price, size);
            this.OrderType = "LIMIT";
            this.Object = new LimitOrder(
                this.SelectionId,
                this.Side,
                this.LimitOrder.Price,
                this.LimitOrder.Size);
        }

        [DataMember(Name = "selectionId", EmitDefaultValue = false)]
        public long SelectionId { get; }

        [DataMember(Name = "side", EmitDefaultValue = false)]
        public Side Side { get; }

        [DataMember(Name = "orderType", EmitDefaultValue = false)]
        public string OrderType { get; }

        [DataMember(Name = "limitOrder", EmitDefaultValue = false)]
        public LimitOrderStub LimitOrder { get; }

        internal LimitOrder Object { get; }

        public string ExpectedInstructionJson()
        {
            return JsonSerializer.ToJsonString(this, StandardResolver.ExcludeNull);
        }

        public string PlaceInstructionReportJson(string betId, string status, string orderStatus)
        {
            return new PlaceInstructionReportStub(this, betId, status, orderStatus).ToJson();
        }

        public PlaceInstructionReportStub PlaceInstructionReportObject(string betId, string status, string orderStatus)
        {
            return new PlaceInstructionReportStub(this, betId, status, orderStatus);
        }
    }
}
