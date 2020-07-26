namespace Betfair.Betting.Tests.TestDoubles.Responses
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Betfair.Betting.Tests.TestDoubles.Requests;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public class PlaceInstructionReportStub
    {
        public PlaceInstructionReportStub(
            LimitOrderBuilder instruction,
            string betId,
            string status,
            string orderStatus)
        {
            this.Instruction = instruction;
            this.BetId = betId;
            this.Status = status;
            this.OrderStatus = orderStatus;
            this.PlacedDate = DateTime.Parse("2013-10-30T14:22:47.000Z", new CultureInfo(1));
            this.AveragePriceMatched = instruction?.LimitOrder.Price;
            this.SizeMatched = instruction?.LimitOrder.Size;
            this.ErrorCode = "TEST_ERROR";
        }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; }

        [DataMember(Name = "orderStatus", EmitDefaultValue = false)]
        public string OrderStatus { get; }

        [DataMember(Name = "errorCode", EmitDefaultValue = false)]
        public string ErrorCode { get; }

        [DataMember(Name = "instruction", EmitDefaultValue = false)]
        public LimitOrderBuilder Instruction { get; }

        [DataMember(Name = "betId", EmitDefaultValue = false)]
        public string BetId { get; }

        [DataMember(Name = "placedDate", EmitDefaultValue = false)]
        public DateTime? PlacedDate { get; }

        [DataMember(Name = "averagePriceMatched", EmitDefaultValue = false)]
        public double? AveragePriceMatched { get; }

        [DataMember(Name = "sizeMatched", EmitDefaultValue = false)]
        public double? SizeMatched { get; }

        public string ToJson()
        {
            return JsonSerializer.ToJsonString(this, StandardResolver.AllowPrivateExcludeNull);
        }
    }
}
