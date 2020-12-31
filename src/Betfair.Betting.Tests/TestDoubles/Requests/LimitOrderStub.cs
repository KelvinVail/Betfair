using System.Runtime.Serialization;

namespace Betfair.Betting.Tests.TestDoubles.Requests
{
    public class LimitOrderStub
    {
        public LimitOrderStub(double price, double size)
        {
            Price = price;
            Size = size;
            PersistenceType = "LAPSE";
        }

        [DataMember(Name = "size", EmitDefaultValue = false)]
        public double Size { get; }

        [DataMember(Name = "price", EmitDefaultValue = false)]
        public double Price { get; }

        [DataMember(Name = "persistenceType", EmitDefaultValue = false)]
        public string PersistenceType { get; }
    }
}
