namespace Betfair.Streaming
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class ResponseMessage
    {
        [DataMember(Name = "op", EmitDefaultValue = false)]
        public string Operation { get; set; }

        [DataMember(Name = "connectionId", EmitDefaultValue = false)]
        public string ConnectionId { get; set; }

        [DataMember(Name = "connectionClosed", EmitDefaultValue = false)]
        public bool ConnectionClosed { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(Name = "initialClk", EmitDefaultValue = false)]
        public string InitialClock { get; set; }
    }
}
