namespace Betfair.Core.Tests.TestDoubles
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ResponseStub<T>
    {
        public ResponseStub(T result)
        {
            this.Result = result;
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
