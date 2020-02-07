namespace Betfair.Streaming
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [DataContract]
    public class MarketDataFilter
    {
        public MarketDataFilter()
        {
            this.Fields = new HashSet<string>();
        }

        [DataMember(Name = "ladderLevels", EmitDefaultValue = false)]
        public int LadderLevels { get; private set; }

        [DataMember(Name = "fields", EmitDefaultValue = false)]
        public HashSet<string> Fields { get; private set; }

        public MarketDataFilter WithMarketDefinition()
        {
            this.Fields.Add("EX_MARKET_DEF");
            return this;
        }

        public MarketDataFilter WithBestPricesIncludingVirtual()
        {
            this.Fields.Add("EX_BEST_OFFERS_DISP");
            if (this.LadderLevelsIsEmpty()) this.LadderLevels = 3;
            return this;
        }

        public MarketDataFilter WithBestPrices()
        {
            this.Fields.Add("EX_BEST_OFFERS");
            if (this.LadderLevelsIsEmpty()) this.LadderLevels = 3;
            return this;
        }

        public MarketDataFilter WithFullOffersLadder()
        {
            this.Fields.Add("EX_ALL_OFFERS");
            return this;
        }

        public MarketDataFilter WithFullTradedLadder()
        {
            this.Fields.Add("EX_TRADED");
            return this;
        }

        public MarketDataFilter WithMarketAndRunnerTradedVolume()
        {
            this.Fields.Add("EX_TRADED_VOL");
            return this;
        }

        public MarketDataFilter WithLastTradedPrice()
        {
            this.Fields.Add("EX_LTP");
            return this;
        }

        public MarketDataFilter WithStartingPriceLadder()
        {
            this.Fields.Add("SP_TRADED");
            return this;
        }

        public MarketDataFilter WithStartingPriceProjection()
        {
            this.Fields.Add("SP_PROJECTED");
            return this;
        }

        public MarketDataFilter WithLadderLevels(int levels)
        {
            this.LadderLevels = levels > 10 ? 10 : levels;
            return this;
        }

        private bool LadderLevelsIsEmpty()
        {
            return this.LadderLevels == 0;
        }
    }
}
