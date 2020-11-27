namespace Betfair.Core.Tests.TestDoubles
{
    using System.Runtime.Serialization;
    using Utf8Json;

    [DataContract]
    public class ExchangeResponseStub
    {
        [DataMember(Name = "testString", EmitDefaultValue = false)]
        public string TestString { get; set; }

        [DataMember(Name = "testDouble", EmitDefaultValue = false)]
        public double TestDouble { get; set; }

        public string ToJson()
        {
            return JsonSerializer.ToJsonString(this);
        }
    }
}
