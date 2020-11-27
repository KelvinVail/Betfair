namespace Betfair.Extensions
{
    using System.Collections.Generic;

    public class LevelLadder
    {
        private readonly Dictionary<double, PriceSize> ladder = new Dictionary<double, PriceSize>();

        public double Price(int level)
        {
            if (!this.ladder.ContainsKey(level)) return 0;
            return this.ladder[level].Price;
        }

        public double Size(int level)
        {
            if (!this.ladder.ContainsKey(level)) return 0;
            return this.ladder[level].Size;
        }

        internal void ProcessLevel(List<double> ladderValue)
        {
            var level = ladderValue[0];
            if (!this.ladder.ContainsKey(level))
                this.ladder.Add(level, new PriceSize(ladderValue));

            this.ladder[level].Price = ladderValue[1];
            this.ladder[level].Size = ladderValue[2];
        }

        private class PriceSize
        {
            public PriceSize(IReadOnlyList<double> levelPriceSize)
            {
                this.Price = levelPriceSize[1];
                this.Size = levelPriceSize[2];
            }

            public double Price { get; set; }

            public double Size { get; set; }
        }
    }
}