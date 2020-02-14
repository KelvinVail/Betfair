namespace Betfair.Streaming
{
    using System;
    using System.Runtime.Serialization;
    using Utf8Json;
    using Utf8Json.Resolvers;

    [DataContract]
    public sealed class ResponseMessage
    {
        [DataMember(Name = "op", EmitDefaultValue = false)]
        public string Operation { get; set; }

        [DataMember(Name = "connectionId", EmitDefaultValue = false)]
        public string ConnectionId { get; set; }

        [DataMember(Name = "connectionClosed", EmitDefaultValue = false)]
        public bool? ConnectionClosed { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }

        [DataMember(Name = "initialClk", EmitDefaultValue = false)]
        public string InitialClock { get; set; }

        [DataMember(Name = "clk", EmitDefaultValue = false)]
        public string Clock { get; set; }

        [DataMember(Name = "conflateMs", EmitDefaultValue = false)]
        public int? ConflateMs { get; set; }

        [DataMember(Name = "heartbeatMs", EmitDefaultValue = false)]
        public int? HeartbeatMs { get; set; }

        [DataMember(Name = "pt", EmitDefaultValue = false)]
        public long? PublishTime { get; set; }

        [DataMember(Name = "ct", EmitDefaultValue = false)]
        public string ChangeType { get; set; }

        [DataMember(Name = "segmentType", EmitDefaultValue = false)]
        public string SegmentType { get; set; }

        public long? ArrivalTime { get; private set; }

        public void SetArrivalTime(DateTime arrivalTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            this.ArrivalTime = (long)(arrivalTime - epoch).TotalMilliseconds;
        }

        public string ToJson()
        {
            return JsonSerializer.ToJsonString(this, StandardResolver.ExcludeNull);
        }
    }
}
