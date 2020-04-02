namespace Betfair.Betting
{
    using System;
    using System.Globalization;

    public class LimitOrder
    {
        public LimitOrder(long selectionId, Side side, double size, double price)
        {
            this.SelectionId = selectionId;
            this.Side = side;
            this.Size = size;
            this.Price = price;
        }

        public long SelectionId { get; }

        public Side Side { get; }

        public double Size { get; }

        public double Price { get; set; }

        public string ToInstruction()
        {
            return $"{{\"selectionId\":\"{this.SelectionId}\"," +
                   $"\"side\":\"{this.Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"," +
                   $"\"orderType\":\"LIMIT\"," +
                   $"\"limitOrder\":{{" +
                   $"\"size\":\"{Math.Round(this.Size, 2)}\"," +
                   $"\"price\":\"{NearestValidPrice(this.Price)}\"," +
                   $"\"persistenceType\":\"LAPSE\"}}}}";
        }

        private static double NearestValidPrice(double price)
        {
            if (price == 1) return 1.01;
            return price;
        }
    }
}
