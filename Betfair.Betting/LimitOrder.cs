namespace Betfair.Betting
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public sealed class LimitOrder
    {
        public LimitOrder(long selectionId, Side side, double price, double size)
        {
            this.SelectionId = selectionId;
            this.Side = side;
            this.Size = LowestPossibleSize(side, price, size);
            this.Price = price;
        }

        public long SelectionId { get; }

        public Side Side { get; }

        public double Size { get; }

        public double Price { get; }

        public double SizeMatched { get; private set; }

        public double AveragePriceMatched { get; private set; }

        public string Status { get; private set; }

        public string BetId { get; private set; }

        public string OrderStatus { get; private set; }

        public string ErrorCode { get; private set; }

        public DateTime PlacedDate { get; private set; }

        public bool BelowMinimumStake => this.Size < MinimumStake(this.Price);

        public bool Valid => IsProfitRatioValid(this.Price, this.Size);

        public string ToInstruction()
        {
            return this.BelowAbsoluteMinimum() ? null :
                   $"{{\"selectionId\":{this.SelectionId}," +
                   $"\"side\":\"{this.Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"," +
                   "\"orderType\":\"LIMIT\"," +
                   "\"limitOrder\":{" +
                   $"\"size\":{this.GetSize()}," +
                   $"\"price\":{this.GetPrice()}," +
                   "\"persistenceType\":\"LAPSE\"}}";
        }

        public string ToCancelInstruction()
        {
            return this.OrderStatus == "EXECUTION_COMPLETE" ? null : $"{{\"betId\":\"{this.BetId}\"}}";
        }

        public string ToBelowMinimumCancelInstruction()
        {
            if (!this.BelowMinimumStake) return null;
            var reduction = Math.Round(2 - this.Size, 2);
            return $"{{\"betId\":\"{this.BetId}\",\"sizeReduction\":{reduction}}}";
        }

        public string ToBelowMinimumReplaceInstruction()
        {
            return !this.BelowMinimumStake ? null : $"{{\"betId\":\"{this.BetId}\",\"newPrice\":{this.Price}}}";
        }

        internal void AddReports(IEnumerable<InstructionReport> reports)
        {
            var report = reports.FirstOrDefault(r => r.Instruction.SelectionId == this.SelectionId && r.Instruction.Side == this.Side);
            this.Update(report);
        }

        private static double LowestPossibleSize(Side side, double price, double size)
        {
            if (side == Side.Back) return size;
            var m = Math.Ceiling((0.01 / (price - 1)) * 100) / 100;
            return m > size ? m : size;
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

        private static double MinimumStake(double price)
        {
            var m = Math.Ceiling(10 / price * 100) / 100;
            return m < 2 ? m : 2;
        }

        private static double LowestLayPrice(double size)
        {
            return Math.Ceiling(((0.02 + size) / size) * 100) / 100;
        }

        private static bool IsProfitRatioValid(double price, double size)
        {
            var liability = Math.Round(size * (1 - price), 2, MidpointRounding.AwayFromZero);
            var actualProfitRatio = size / liability;
            var fairProfitRatio = 1 / (1 - price);
            var diff = Math.Round((actualProfitRatio / fairProfitRatio) - 1, 4);
            return diff >= -0.2 && diff < 0.25;
        }

        private double GetPrice()
        {
            var price = this.Size < MinimumStake(this.Price)
                ? (this.Side == Side.Back ? 1000 : LowestLayPrice(this.Size))
                : NearestValidPrice(this.Price);
            return price;
        }

        private double GetSize()
        {
            var size = this.Size < MinimumStake(this.Price) ? 2 : Math.Round(this.Size, 2);
            return size;
        }

        private bool BelowAbsoluteMinimum()
        {
            return LowestLayPrice(this.Size) > this.Price && this.Side == Side.Lay;
        }

        private void Update(InstructionReport report)
        {
            if (report is null) return;
            this.BetId = report.BetId;
            this.SizeMatched = report.SizeMatched;
            this.AveragePriceMatched = report.AveragePriceMatched;
            this.Status = report.Status;
            this.OrderStatus = report.OrderStatus;
            this.PlacedDate = report.PlacedDate;
            this.ErrorCode = report.ErrorCode;
        }
    }
}
