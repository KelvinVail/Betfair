namespace Betfair.Extensions
{
    public class PriceSizeTimeWindow
    {
        private readonly int secondsToLive;

        private readonly TimeWindow sizeWindow;

        private readonly TimeWindow weightedPriceWindow;

        public PriceSizeTimeWindow(int secondsToLive)
        {
            this.secondsToLive = secondsToLive;
            this.sizeWindow = new TimeWindow(secondsToLive);
            this.weightedPriceWindow = new TimeWindow(secondsToLive);
        }

        public double MeanSize => this.sizeWindow.Mean();

        public double Vwap => this.weightedPriceWindow.Total() / this.sizeWindow.Total();

        public double SizePerSecond => this.sizeWindow.Total() / this.secondsToLive;

        public void Update(double price, double size, long unixMilliseconds)
        {
            this.sizeWindow.Update(unixMilliseconds, size);
            this.weightedPriceWindow.Update(unixMilliseconds, price * size);
        }
    }
}
