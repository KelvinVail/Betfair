using System.Collections.Generic;

namespace Betfair.Extensions
{
    public class LevelLadder
    {
        private readonly Dictionary<double, PriceSize> _ladder = new Dictionary<double, PriceSize>();

        public double Price(int level)
        {
            if (!_ladder.ContainsKey(level)) return 0;
            return _ladder[level].Price;
        }

        public double Size(int level)
        {
            if (!_ladder.ContainsKey(level)) return 0;
            return _ladder[level].Size;
        }

        internal void ProcessLevel(List<double> ladderValue)
        {
            var level = ladderValue[0];
            if (!_ladder.ContainsKey(level))
                _ladder.Add(level, new PriceSize(ladderValue));

            _ladder[level].Price = ladderValue[1];
            _ladder[level].Size = ladderValue[2];
        }

        private class PriceSize
        {
            public PriceSize(IReadOnlyList<double> levelPriceSize)
            {
                Price = levelPriceSize[1];
                Size = levelPriceSize[2];
            }

            public double Price { get; set; }

            public double Size { get; set; }
        }
    }
}