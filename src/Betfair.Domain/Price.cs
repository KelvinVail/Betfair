using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Betfair.Domain
{
    public class Price : ValueObject
    {
        private static readonly decimal[] _validPrices =
        {
            1.01m, 1.02m, 1.03m, 1.04m, 1.05m, 1.06m, 1.07m, 1.08m, 1.09m, 1.1m, 1.11m,
            1.12m, 1.13m, 1.14m, 1.15m, 1.16m, 1.17m, 1.18m, 1.19m, 1.2m, 1.21m, 1.22m,
            1.23m, 1.24m, 1.25m, 1.26m, 1.27m, 1.28m, 1.29m, 1.3m, 1.31m, 1.32m, 1.33m,
            1.34m, 1.35m, 1.36m, 1.37m, 1.38m, 1.39m, 1.4m, 1.41m, 1.42m, 1.43m, 1.44m,
            1.45m, 1.46m, 1.47m, 1.48m, 1.49m, 1.5m, 1.51m, 1.52m, 1.53m, 1.54m, 1.55m,
            1.56m, 1.57m, 1.58m, 1.59m, 1.6m, 1.61m, 1.62m, 1.63m, 1.64m, 1.65m, 1.66m,
            1.67m, 1.68m, 1.69m, 1.7m, 1.71m, 1.72m, 1.73m, 1.74m, 1.75m, 1.76m, 1.77m,
            1.78m, 1.79m, 1.8m, 1.81m, 1.82m, 1.83m, 1.84m, 1.85m, 1.86m, 1.87m, 1.88m,
            1.89m, 1.9m, 1.91m, 1.92m, 1.93m, 1.94m, 1.95m, 1.96m, 1.97m, 1.98m, 1.99m,
            2.0m,

            2.02m, 2.04m, 2.06m, 2.08m, 2.1m, 2.12m, 2.14m, 2.16m, 2.18m, 2.2m, 2.22m,
            2.24m, 2.26m, 2.28m, 2.3m, 2.32m, 2.34m, 2.36m, 2.38m, 2.4m, 2.42m, 2.44m,
            2.46m, 2.48m, 2.5m, 2.52m, 2.54m, 2.56m, 2.58m, 2.6m, 2.62m, 2.64m, 2.66m,
            2.68m, 2.7m, 2.72m, 2.74m, 2.76m, 2.78m, 2.8m, 2.82m, 2.84m, 2.86m, 2.88m,
            2.9m, 2.92m, 2.94m, 2.96m, 2.98m, 3.0m,

            3.05m, 3.1m, 3.15m, 3.2m, 3.25m, 3.3m, 3.35m, 3.4m, 3.45m, 3.5m, 3.55m, 3.6m,
            3.65m, 3.7m, 3.75m, 3.8m, 3.85m, 3.9m, 3.95m, 4.0m,

            4.1m, 4.2m, 4.3m, 4.4m, 4.5m, 4.6m, 4.7m, 4.8m, 4.9m, 5.0m, 5.1m, 5.2m, 5.3m,
            5.4m, 5.5m, 5.6m, 5.7m, 5.8m, 5.9m, 6.0m,

            6.2m, 6.4m, 6.6m, 6.8m, 7.0m, 7.2m, 7.4m, 7.6m, 7.8m, 8.0m, 8.2m, 8.4m, 8.6m,
            8.8m, 9.0m, 9.2m, 9.4m, 9.6m, 9.8m, 10.0m,

            10.5m, 11.0m, 11.5m, 12.0m, 12.5m, 13.0m, 13.5m, 14.0m, 14.5m, 15.0m, 15.5m,
            16.0m, 16.5m, 17.0m, 17.5m, 18.0m, 18.5m, 19.0m, 19.5m,

            20.0m, 21.0m, 22.0m, 23.0m, 24.0m, 25.0m, 26.0m, 27.0m, 28.0m, 29.0m, 30.0m,

            32.0m, 34.0m, 36.0m, 38.0m, 40.0m, 42.0m, 44.0m, 46.0m, 48.0m, 50.0m,

            55.0m, 60.0m, 65.0m, 70.0m, 75.0m, 80.0m, 85.0m, 90.0m, 95.0m, 100.0m,

            110.0m, 120.0m, 130.0m, 140.0m, 150.0m, 160.0m, 170.0m, 180.0m, 190.0m, 200.0m,
            210.0m, 220.0m, 230.0m, 240.0m, 250.0m, 260.0m, 270.0m, 280.0m, 290.0m, 300.0m,
            310.0m, 320.0m, 330.0m, 340.0m, 350.0m, 360.0m, 370.0m, 380.0m, 390.0m, 400.0m,
            410.0m, 420.0m, 430.0m, 440.0m, 450.0m, 460.0m, 470.0m, 480.0m, 490.0m, 500.0m,
            510.0m, 520.0m, 530.0m, 540.0m, 550.0m, 560.0m, 570.0m, 580.0m, 590.0m, 600.0m,
            610.0m, 620.0m, 630.0m, 640.0m, 650.0m, 660.0m, 670.0m, 680.0m, 690.0m, 700.0m,
            710.0m, 720.0m, 730.0m, 740.0m, 750.0m, 760.0m, 770.0m, 780.0m, 790.0m, 800.0m,
            810.0m, 820.0m, 830.0m, 840.0m, 850.0m, 860.0m, 870.0m, 880.0m, 890.0m, 900.0m,
            910.0m, 920.0m, 930.0m, 940.0m, 950.0m, 960.0m, 970.0m, 980.0m, 990.0m, 1000.0m,
        };

        private Price(decimal decimalOdds) => DecimalOdds = decimalOdds;

        public decimal DecimalOdds { get; }

        public decimal Chance => 1 / DecimalOdds;

        public static Price Of(decimal decimalOdds)
        {
            if (!_validPrices.Contains(decimalOdds))
                throw new ArgumentException(Errors.InvalidPrice(decimalOdds).Message);

            return new Price(decimalOdds);
        }

        public Price AddTicks(int ticks)
        {
            var index = Array.IndexOf(_validPrices, DecimalOdds);

            if (index + ticks >= _validPrices.Length)
                return new Price(1000);

            if (index + ticks < 0)
                return new Price(1.01m);

            return new Price(_validPrices[index + ticks]);
        }

        public int TicksBetween(Price endPrice)
        {
            if (endPrice == null) return 0;

            var indexFrom = Array.IndexOf(_validPrices, DecimalOdds);
            var indexTo = Array.IndexOf(_validPrices, endPrice.DecimalOdds);
            return indexTo - indexFrom;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DecimalOdds;
        }
    }
}
