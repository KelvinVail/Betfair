using System.Runtime.Serialization;

namespace Betfair.Core.Tests.TestDoubles
{
    [DataContract]
    public class RequestBuilder
    {
        public RequestBuilder()
        {
            Id = 1;
            Jsonrpc = "2.0";
        }

        [DataMember(Name = "jsonrpc", EmitDefaultValue = false)]
        public string Jsonrpc { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(Name = "method", EmitDefaultValue = false)]
        public string Method { get; set; }

        [DataMember(Name = "params", EmitDefaultValue = false)]
        public dynamic Params { get; set; }
    }
}
