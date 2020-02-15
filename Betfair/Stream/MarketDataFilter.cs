namespace Betfair.Stream
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarketDataFilter
    {
        [DataMember(Name = "ladderLevels", EmitDefaultValue = false)]
        public int? LadderLevels { get; private set; }

        [DataMember(Name = "fields", EmitDefaultValue = false)]
        public HashSet<string> Fields { get; private set; }

        public MarketDataFilter WithMarketDefinition()
        {
            this.InitializedFields();
            this.Fields.Add("EX_MARKET_DEF");
            return this;
        }

        public MarketDataFilter WithBestPricesIncludingVirtual()
        {
            this.InitializedFields();
            this.Fields.Add("EX_BEST_OFFERS_DISP");
            if (this.LadderLevelsEmpty()) this.LadderLevels = 3;
            return this;
        }

        public MarketDataFilter WithBestPrices()
        {
            this.InitializedFields();
            this.Fields.Add("EX_BEST_OFFERS");
            if (this.LadderLevelsEmpty()) this.LadderLevels = 3;
            return this;
        }

        public MarketDataFilter WithFullOffersLadder()
        {
            this.InitializedFields();
            this.Fields.Add("EX_ALL_OFFERS");
            return this;
        }

        public MarketDataFilter WithFullTradedLadder()
        {
            this.InitializedFields();
            this.Fields.Add("EX_TRADED");
            return this;
        }

        public MarketDataFilter WithMarketAndRunnerTradedVolume()
        {
            this.InitializedFields();
            this.Fields.Add("EX_TRADED_VOL");
            return this;
        }

        public MarketDataFilter WithLastTradedPrice()
        {
            this.InitializedFields();
            this.Fields.Add("EX_LTP");
            return this;
        }

        public MarketDataFilter WithStartingPriceLadder()
        {
            this.InitializedFields();
            this.Fields.Add("SP_TRADED");
            return this;
        }

        public MarketDataFilter WithStartingPriceProjection()
        {
            this.InitializedFields();
            this.Fields.Add("SP_PROJECTED");
            return this;
        }

        public MarketDataFilter WithLadderLevels(int levels)
        {
            this.LadderLevels = levels > 10 ? 10 : levels;
            return this;
        }

        private bool LadderLevelsEmpty()
        {
            return this.LadderLevels == null;
        }

        private void InitializedFields()
        {
            if (this.Fields == null) this.Fields = new HashSet<string>();
        }
    }
}
