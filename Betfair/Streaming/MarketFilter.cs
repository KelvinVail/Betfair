namespace Betfair.Streaming
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarketFilter
    {
        public MarketFilter()
        {
            this.CountryCodes = new HashSet<string>();
            this.BettingTypes = new HashSet<string>();
            this.MarketTypes = new HashSet<string>();
            this.Venues = new HashSet<string>();
            this.MarketIds = new HashSet<string>();
            this.EventTypeIds = new HashSet<string>();
            this.EventIds = new HashSet<string>();
        }

        [DataMember(Name = "countryCodes", EmitDefaultValue = false)]
        public HashSet<string> CountryCodes { get; private set; }

        [DataMember(Name = "bettingTypes", EmitDefaultValue = false)]
        public HashSet<string> BettingTypes { get; private set; }

        [DataMember(Name = "turnInPlayEnabled", EmitDefaultValue = false)]
        public bool? TurnInPlayEnabled { get; private set; }

        [DataMember(Name = "marketTypes", EmitDefaultValue = false)]
        public HashSet<string> MarketTypes { get; private set; }

        [DataMember(Name = "venues", EmitDefaultValue = false)]
        public HashSet<string> Venues { get; private set; }

        [DataMember(Name = "marketIds", EmitDefaultValue = false)]
        public HashSet<string> MarketIds { get; private set; }

        [DataMember(Name = "eventTypeIds", EmitDefaultValue = false)]
        public HashSet<string> EventTypeIds { get; private set; }

        [DataMember(Name = "eventIds", EmitDefaultValue = false)]
        public HashSet<string> EventIds { get; private set; }

        [DataMember(Name = "bspMarket", EmitDefaultValue = false)]
        public bool? BspMarket { get; private set; }

        public MarketFilter WithMarketId(string marketId)
        {
            this.MarketIds.Add(marketId);
            return this;
        }

        public MarketFilter WithCountryCode(string countryCode)
        {
            this.CountryCodes.Add(countryCode);
            return this;
        }

        public MarketFilter WithInPlayMarketsOnly()
        {
            this.TurnInPlayEnabled = true;
            return this;
        }

        public MarketFilter WithMarketType(string marketType)
        {
            this.MarketTypes.Add(marketType);
            return this;
        }

        public MarketFilter WithVenue(string venue)
        {
            this.Venues.Add(venue);
            return this;
        }

        public MarketFilter WithEventTypeId(string eventTypeId)
        {
            this.EventTypeIds.Add(eventTypeId);
            return this;
        }

        public MarketFilter WithEventId(string eventId)
        {
            this.EventIds.Add(eventId);
            return this;
        }

        public MarketFilter WithBspMarketsOnly()
        {
            this.BspMarket = true;
            return this;
        }

        public MarketFilter WithBettingTypeOdds()
        {
            this.BettingTypes.Add("ODDS");
            return this;
        }

        public MarketFilter WithBettingTypeLine()
        {
            this.BettingTypes.Add("LINE");
            return this;
        }

        public MarketFilter WithBettingTypeRange()
        {
            this.BettingTypes.Add("RANGE");
            return this;
        }

        public MarketFilter WithBettingTypeAsianHandicapDoubleLine()
        {
            this.BettingTypes.Add("ASIAN_HANDICAP_DOUBLE_LINE");
            return this;
        }

        public MarketFilter WithBettingTypeAsianHandicapSingleLine()
        {
            this.BettingTypes.Add("ASIAN_HANDICAP_SINGLE_LINE");
            return this;
        }
    }
}
