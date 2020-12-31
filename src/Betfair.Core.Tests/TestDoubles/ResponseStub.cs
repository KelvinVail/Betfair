using System.Runtime.Serialization;

namespace Betfair.Core.Tests.TestDoubles
{
    [DataContract]
    public class ResponseStub<T>
    {
        public ResponseStub(T result)
        {
            Result = result;
        }

        [DataMember(Name = "jsonrpc", EmitDefaultValue = false)]
        public string Jsonrpc => "2.0";

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id => 1;

        [DataMember(Name = "result", EmitDefaultValue = false)]
        public T Result { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }
    }
}
