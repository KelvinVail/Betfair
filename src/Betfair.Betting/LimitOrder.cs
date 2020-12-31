using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Betfair.Betting
{
    public sealed class LimitOrder
    {
        private readonly Dictionary<double, double> _lowestValidPrices
            = new Dictionary<double, double>
            {
                { 0.01, 1.8 }, { 0.02, 1.4 }, { 0.03, 1.27 }, { 0.04, 1.2 },
                { 0.05, 1.16 }, { 0.06, 1.14 }, { 0.07, 1.12 }, { 0.08, 1.1 },
                { 0.09, 1.09 }, { 0.1, 1.08 }, { 0.11, 1.08 }, { 0.12, 1.07 },
                { 0.13, 1.07 }, { 0.14, 1.06 }, { 0.15, 1.06 }, { 0.16, 1.05 },
                { 0.17, 1.05 }, { 0.18, 1.05 }, { 0.19, 1.05 }, { 0.2, 1.04 },
                { 0.21, 1.04 }, { 0.22, 1.04 }, { 0.23, 1.04 }, { 0.24, 1.04 },
                { 0.25, 1.04 }, { 0.26, 1.04 }, { 0.27, 1.03 }, { 0.28, 1.03 },
                { 0.29, 1.03 }, { 0.3, 1.03 }, { 0.31, 1.03 }, { 0.32, 1.03 },
                { 0.33, 1.03 }, { 0.34, 1.03 }, { 0.35, 1.03 }, { 0.36, 1.03 },
                { 0.37, 1.03 }, { 0.38, 1.03 }, { 0.39, 1.03 }, { 0.4, 1.02 },
                { 0.41, 1.02 }, { 0.42, 1.02 }, { 0.43, 1.02 }, { 0.44, 1.02 },
                { 0.45, 1.02 }, { 0.46, 1.02 }, { 0.47, 1.02 }, { 0.48, 1.02 },
                { 0.49, 1.02 }, { 0.5, 1.02 }, { 0.51, 1.02 }, { 0.52, 1.02 },
                { 0.53, 1.02 }, { 0.54, 1.02 }, { 0.55, 1.02 }, { 0.56, 1.02 },
                { 0.57, 1.02 }, { 0.58, 1.02 }, { 0.59, 1.02 }, { 0.6, 1.02 },
                { 0.61, 1.02 }, { 0.62, 1.02 }, { 0.63, 1.03 }, { 0.64, 1.03 },
                { 0.65, 1.03 }, { 0.66, 1.03 }, { 0.67, 1.03 }, { 0.68, 1.03 },
                { 0.69, 1.03 }, { 0.7, 1.03 }, { 0.71, 1.03 }, { 0.72, 1.03 },
                { 0.73, 1.03 }, { 0.74, 1.03 }, { 0.75, 1.03 }, { 0.76, 1.03 },
                { 0.77, 1.03 }, { 0.78, 1.03 }, { 0.79, 1.03 }, { 0.8, 1.01 },
                { 0.81, 1.01 }, { 0.82, 1.01 }, { 0.83, 1.01 }, { 0.84, 1.01 },
                { 0.85, 1.01 }, { 0.86, 1.01 }, { 0.87, 1.01 }, { 0.88, 1.01 },
                { 0.89, 1.01 }, { 0.9, 1.01 }, { 0.91, 1.01 }, { 0.92, 1.01 },
                { 0.93, 1.01 }, { 0.94, 1.01 }, { 0.95, 1.01 }, { 0.96, 1.01 },
                { 0.97, 1.01 }, { 0.98, 1.01 }, { 0.99, 1.01 }, { 1, 1.01 },
                { 1.01, 1.01 }, { 1.02, 1.01 }, { 1.03, 1.01 }, { 1.04, 1.01 },
                { 1.05, 1.01 }, { 1.06, 1.01 }, { 1.07, 1.01 }, { 1.08, 1.01 },
                { 1.09, 1.01 }, { 1.1, 1.01 }, { 1.11, 1.01 }, { 1.12, 1.01 },
                { 1.13, 1.01 }, { 1.14, 1.01 }, { 1.15, 1.01 }, { 1.16, 1.01 },
                { 1.17, 1.01 }, { 1.18, 1.01 }, { 1.19, 1.01 }, { 1.2, 1.01 },
                { 1.21, 1.01 }, { 1.22, 1.01 }, { 1.23, 1.01 }, { 1.24, 1.01 },
                { 1.25, 1.02 }, { 1.26, 1.02 }, { 1.27, 1.02 }, { 1.28, 1.02 },
                { 1.29, 1.02 }, { 1.3, 1.02 }, { 1.31, 1.02 }, { 1.32, 1.02 },
                { 1.33, 1.02 }, { 1.34, 1.02 }, { 1.35, 1.02 }, { 1.36, 1.02 },
                { 1.37, 1.02 }, { 1.38, 1.02 }, { 1.39, 1.02 }, { 1.4, 1.02 },
                { 1.41, 1.02 }, { 1.42, 1.02 }, { 1.43, 1.02 }, { 1.44, 1.02 },
                { 1.45, 1.02 }, { 1.46, 1.02 }, { 1.47, 1.02 }, { 1.48, 1.02 },
                { 1.49, 1.02 }, { 1.5, 1.02 }, { 1.51, 1.02 }, { 1.52, 1.02 },
                { 1.53, 1.02 }, { 1.54, 1.02 }, { 1.55, 1.02 }, { 1.56, 1.02 },
                { 1.57, 1.02 }, { 1.58, 1.02 }, { 1.59, 1.02 },
            };

        public LimitOrder(long selectionId, Side side, double price, double size)
        {
            SelectionId = selectionId;
            Side = side;
            Size = size;
            Price = price;
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

        public bool BelowMinimumStake => Size < MinimumStake(Price);

        public bool Valid => IsProfitRatioValid(NearestValidPrice(Price), Size);

        public string ToInstruction()
        {
            if (Side == Side.Lay && ValidPrice() > Price) return null;
            if (!Valid) return null;

            return $"{{\"selectionId\":{SelectionId}," +
                   $"\"side\":\"{Side.ToString().ToUpper(CultureInfo.CurrentCulture)}\"," +
                   "\"orderType\":\"LIMIT\"," +
                   "\"limitOrder\":{" +
                   $"\"size\":{GetSize()}," +
                   $"\"price\":{ValidPrice()}," +
                   "\"persistenceType\":\"LAPSE\"}}";
        }

        public string ToCancelInstruction()
        {
            return OrderStatus == "EXECUTION_COMPLETE" ? null
                : $"{{\"betId\":\"{BetId}\"}}";
        }

        public string ToBelowMinimumCancelInstruction()
        {
            if (!BelowMinimumStake) return null;
            var reduction = Math.Round(2 - Size, 2);
            return $"{{\"betId\":\"{BetId}\",\"sizeReduction\":{reduction}}}";
        }

        public string ToBelowMinimumReplaceInstruction()
        {
            return !BelowMinimumStake ? null : $"{{\"betId\":\"{BetId}\",\"newPrice\":{Price}}}";
        }

        internal void AddReports(IEnumerable<InstructionReport> reports)
        {
            var report = reports.FirstOrDefault(r => r.Instruction.SelectionId == SelectionId && r.Instruction.Side == Side);
            Update(report);
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

        private static bool IsProfitRatioValid(double price, double size)
        {
            var liability = Math.Round(size * (1 - price), 2, MidpointRounding.AwayFromZero);
            var actualProfitRatio = size / liability;
            var fairProfitRatio = 1 / (1 - price);
            var diff = Math.Round((actualProfitRatio / fairProfitRatio) - 1, 4);
            return diff >= -0.2 && diff < 0.25;
        }

        private double ValidPrice()
        {
            var price = Size < MinimumStake(Price)
                ? LowestValidPriceOrDefault()
                : NearestValidPrice(Price);
            return price;
        }

        private double LowestValidPriceOrDefault()
        {
            return Side == Side.Back ? 1000 : LowestValidPrice(Size);
        }

        private double LowestValidPrice(double size)
        {
            return !_lowestValidPrices.ContainsKey(size) ? 1.01
                : _lowestValidPrices[size];
        }

        private double GetSize()
        {
            var size = Size < MinimumStake(Price) ? 2 : Math.Round(Size, 2);
            return size;
        }

        private void Update(InstructionReport report)
        {
            if (report is null) return;
            BetId = report.BetId;
            SizeMatched = report.SizeMatched;
            AveragePriceMatched = report.AveragePriceMatched;
            Status = report.Status;
            OrderStatus = report.OrderStatus;
            PlacedDate = report.PlacedDate;
            ErrorCode = report.ErrorCode;
        }
    }
}
