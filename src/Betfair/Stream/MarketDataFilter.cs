using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Betfair.Stream
{
    [DataContract]
    public class MarketDataFilter
    {
        [DataMember(Name = "ladderLevels", EmitDefaultValue = false)]
        public int? LadderLevels { get; private set; }

        [DataMember(Name = "fields", EmitDefaultValue = false)]
        public HashSet<string> Fields { get; private set; }

        public MarketDataFilter WithMarketDefinition()
        {
            InitializedFields();
            Fields.Add("EX_MARKET_DEF");
            return this;
        }

        public MarketDataFilter WithBestPricesIncludingVirtual()
        {
            InitializedFields();
            Fields.Add("EX_BEST_OFFERS_DISP");
            if (LadderLevelsEmpty()) LadderLevels = 3;
            return this;
        }

        public MarketDataFilter WithBestPrices()
        {
            InitializedFields();
            Fields.Add("EX_BEST_OFFERS");
            if (LadderLevelsEmpty()) LadderLevels = 3;
            return this;
        }

        public MarketDataFilter WithFullOffersLadder()
        {
            InitializedFields();
            Fields.Add("EX_ALL_OFFERS");
            return this;
        }

        public MarketDataFilter WithFullTradedLadder()
        {
            InitializedFields();
            Fields.Add("EX_TRADED");
            return this;
        }

        public MarketDataFilter WithMarketAndRunnerTradedVolume()
        {
            InitializedFields();
            Fields.Add("EX_TRADED_VOL");
            return this;
        }

        public MarketDataFilter WithLastTradedPrice()
        {
            InitializedFields();
            Fields.Add("EX_LTP");
            return this;
        }

        public MarketDataFilter WithStartingPriceLadder()
        {
            InitializedFields();
            Fields.Add("SP_TRADED");
            return this;
        }

        public MarketDataFilter WithStartingPriceProjection()
        {
            InitializedFields();
            Fields.Add("SP_PROJECTED");
            return this;
        }

        public MarketDataFilter WithLadderLevels(int levels)
        {
            LadderLevels = levels > 10 ? 10 : levels;
            return this;
        }

        public void Merge(MarketDataFilter other)
        {
            MergeFields(other);
            MergeLadderLevels(other);
        }

        private void MergeFields(MarketDataFilter other)
        {
            InitializedFields();
            if (other?.Fields != null) Fields.UnionWith(other.Fields);
        }

        private void MergeLadderLevels(MarketDataFilter other)
        {
            if (other?.LadderLevels == null) return;
            if (LadderLevelsEmpty()) LadderLevels = other.LadderLevels;
            if (LadderLevels != null) LadderLevels = Math.Max((int)LadderLevels, (int)other.LadderLevels);
        }

        private bool LadderLevelsEmpty()
        {
            return LadderLevels == null;
        }

        private void InitializedFields()
        {
            Fields ??= new HashSet<string>();
        }
    }
}
