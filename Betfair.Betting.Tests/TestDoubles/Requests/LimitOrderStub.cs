namespace Betfair.Betting.Tests.TestDoubles.Requests
{
    using System.Runtime.Serialization;

    public class LimitOrderStub
    {
        public LimitOrderStub(double price, double size)
        {
            this.Price = price;
            this.Size = size;
            this.PersistenceType = "LAPSE";
        }

        [DataMember(Name = "size", EmitDefaultValue = false)]
        public double Size { get; }

        [DataMember(Name = "price", EmitDefaultValue = false)]
        public double Price { get; }

        [DataMember(Name = "persistenceType", EmitDefaultValue = false)]
        public string PersistenceType { get; }
    }
}
