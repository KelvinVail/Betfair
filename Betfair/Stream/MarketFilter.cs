namespace Betfair.Stream
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarketFilter
    {
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
            if (this.MarketIds == null) this.MarketIds = new HashSet<string>();
            this.MarketIds.Add(marketId);
            return this;
        }

        public MarketFilter WithCountryCode(string countryCode)
        {
            if (this.CountryCodes == null) this.CountryCodes = new HashSet<string>();
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
            if (this.MarketTypes == null) this.MarketTypes = new HashSet<string>();
            this.MarketTypes.Add(marketType);
            return this;
        }

        public MarketFilter WithVenue(string venue)
        {
            if (this.Venues == null) this.Venues = new HashSet<string>();
            this.Venues.Add(venue);
            return this;
        }

        public MarketFilter WithEventTypeId(string eventTypeId)
        {
            if (this.EventTypeIds == null) this.EventTypeIds = new HashSet<string>();
            this.EventTypeIds.Add(eventTypeId);
            return this;
        }

        public MarketFilter WithEventId(string eventId)
        {
            if (this.EventIds == null) this.EventIds = new HashSet<string>();
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
            if (this.BettingTypes == null) this.BettingTypes = new HashSet<string>();
            this.BettingTypes.Add("ODDS");
            return this;
        }

        public MarketFilter WithBettingTypeLine()
        {
            if (this.BettingTypes == null) this.BettingTypes = new HashSet<string>();
            this.BettingTypes.Add("LINE");
            return this;
        }

        public MarketFilter WithBettingTypeRange()
        {
            if (this.BettingTypes == null) this.BettingTypes = new HashSet<string>();
            this.BettingTypes.Add("RANGE");
            return this;
        }

        public MarketFilter WithBettingTypeAsianHandicapDoubleLine()
        {
            if (this.BettingTypes == null) this.BettingTypes = new HashSet<string>();
            this.BettingTypes.Add("ASIAN_HANDICAP_DOUBLE_LINE");
            return this;
        }

        public MarketFilter WithBettingTypeAsianHandicapSingleLine()
        {
            if (this.BettingTypes == null) this.BettingTypes = new HashSet<string>();
            this.BettingTypes.Add("ASIAN_HANDICAP_SINGLE_LINE");
            return this;
        }
    }
}
