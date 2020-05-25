namespace Betfair.Extensions.Tests
{
    using Xunit;

    public class TimeWindowTests
    {
        private readonly TimeWindow timeWindow = new TimeWindow(60);

        [Fact]
        public void IfArrivesAtSameTimeAddToExistingValue()
        {
            this.AssertMeanIsUpdated(20000, 2, 2);
            this.AssertMeanIsUpdated(20000, 3, 5);
        }

        [Fact]
        public void KeepsTrackOfTotal()
        {
            this.timeWindow.Update(1000, 1);
            this.timeWindow.Update(5000, 2);
            this.timeWindow.Update(62000, 6);

            Assert.Equal(8.00, this.timeWindow.Total(), 2);
        }

        [Fact]
        public void AddValuesToWindow()
        {
            this.AssertMeanIsUpdated(20000, 231.72, 231.72);
            this.AssertMeanIsUpdated(26410, 355.36, 293.54);
            this.AssertMeanIsUpdated(33724, 335.32, 307.466666666667);
            this.AssertMeanIsUpdated(42577, 550.38, 368.195);
            this.AssertMeanIsUpdated(42884, 509.15, 396.386);
            this.AssertMeanIsUpdated(47993, 351.76, 388.948333333333);
            this.AssertMeanIsUpdated(51162, 818.14, 450.261428571429);
            this.AssertMeanIsUpdated(55698, 989.56, 517.67375);
            this.AssertMeanIsUpdated(62693, 9.69, 461.231111111111);
            this.AssertMeanIsUpdated(63709, 908.97, 506.005);
            this.AssertMeanIsUpdated(69800, 407.36, 497.037272727273);
            this.AssertMeanIsUpdated(73891, 52.25, 459.971666666667);
            this.AssertMeanIsUpdated(75594, 66.38, 429.695384615385);
            this.AssertMeanIsUpdated(85317, 998.76, 488.698461538462);
            this.AssertMeanIsUpdated(95058, 328.36, 499.23);
            this.AssertMeanIsUpdated(102160, 596.95, 506.746923076923);
            this.AssertMeanIsUpdated(106591, 922, 537.515);
            this.AssertMeanIsUpdated(108232, 131.31, 519.144166666667);
            this.AssertMeanIsUpdated(113161, 87.19, 458.231666666667);
            this.AssertMeanIsUpdated(114901, 116.2, 431.921538461538);
            this.AssertMeanIsUpdated(118341, 237.45, 374.066923076923);
            this.AssertMeanIsUpdated(121081, 720.2, 398.790714285714);
            this.AssertMeanIsUpdated(122360, 889.75, 431.521333333333);
            this.AssertMeanIsUpdated(130706, 989.66, 472.035384615385);
            this.AssertMeanIsUpdated(131042, 980.37, 508.345);
            this.AssertMeanIsUpdated(131616, 497.49, 507.621333333333);
            this.AssertMeanIsUpdated(135938, 756.82, 589.465);
            this.AssertMeanIsUpdated(142129, 293.91, 569.761333333333);
            this.AssertMeanIsUpdated(151375, 953.55, 566.747333333333);
            this.AssertMeanIsUpdated(153593, 98.79, 537.5);
            this.AssertMeanIsUpdated(160257, 930.25, 575.118125);
            this.AssertMeanIsUpdated(169227, 826.03, 598.404285714286);
        }

        private void AssertMeanIsUpdated(long unixMilliseconds, double value, double mean)
        {
            this.timeWindow.Update(unixMilliseconds, value);
            Assert.Equal(mean, this.timeWindow.Mean(), 3);
        }
    }
}
