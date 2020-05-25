namespace Betfair.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public class LevelLadder
    {
        private readonly List<LevelPriceSize> ladder;

        public LevelLadder()
        {
            this.ladder = new List<LevelPriceSize>();
        }

        public double Price(int ladderLevel)
        {
            return this.ladder
                .Where(w => w.Level == ladderLevel - 1)
                .Select(s => s.Price).FirstOrDefault();
        }

        public double Size(int ladderLevel)
        {
            return this.ladder
                .Where(w => w.Level == ladderLevel - 1)
                .Select(s => s.Size).FirstOrDefault();
        }

        public double TotalSizeAvailable()
        {
            return this.ladder.Sum(s => s.Size);
        }

        internal void ProcessLevel(List<double> ladderValue)
        {
            if (this.ladder.All(a => a.Level != (int)ladderValue[0]))
            {
                this.ladder.Add(new LevelPriceSize(ladderValue));
            }

            this.ladder.First(
                    w => w.Level == (int)ladderValue[0]).Update(ladderValue);
        }

        private class LevelPriceSize
        {
            public LevelPriceSize(IReadOnlyList<double> levelPriceSize)
            {
                this.Level = (int)levelPriceSize[0];
                this.Price = levelPriceSize[1];
                this.Size = levelPriceSize[2];
            }

            public int Level { get; }

            public double Price { get; private set; }

            public double Size { get; private set; }

            public void Update(IReadOnlyList<double> levelPriceSize)
            {
                this.Price = levelPriceSize[1];
                this.Size = levelPriceSize[2];
            }
        }
    }
}