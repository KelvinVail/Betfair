namespace Betfair.Betting
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

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

        public double Price { get; }

        public double SizeMatched { get; private set; }

        public string ToInstruction()
        {
            return $"{{\"selectionId\":\"{this.SelectionId}\"," +
                   $"\"side\":\"{this.Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"," +
                   "\"orderType\":\"LIMIT\"," +
                   "\"limitOrder\":{" +
                   $"\"size\":\"{Math.Round(this.Size, 2)}\"," +
                   $"\"price\":\"{NearestValidPrice(this.Price)}\"," +
                   "\"persistenceType\":\"LAPSE\"}}";
        }

        internal void AddReports(IEnumerable<InstructionReport> reports)
        {
            var report = reports.FirstOrDefault(r => r.Instruction.SelectionId == this.SelectionId && r.Instruction.Side == this.Side);
            if (report is null) return;
            this.SizeMatched = report.SizeMatched;
        }

        private static double NearestValidPrice(double price)
        {
            if (price <= 1.01) return 1.01;
            if (price >= 1000) return 1000;

            var increment = GetIncrement(price);
            return Math.Round((price * (1 / increment)) - 0.0001) / (1 / increment);
        }

        private static double GetIncrement(double price)
        {
            var increments = new Dictionary<int, double>
            {
                { 2, 0.01 },
                { 3, 0.02 },
                { 4, 0.05 },
                { 6, 0.1 },
                { 10, 0.2 },
                { 20, 0.5 },
                { 30, 1 },
                { 50, 2 },
                { 100, 5 },
                { 1000, 10 },
            };

            return increments.First(increment => price < increment.Key).Value;
        }
    }
}
