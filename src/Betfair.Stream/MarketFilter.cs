using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Betfair.Stream
{
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
            MarketIds ??= new HashSet<string>();
            MarketIds.Add(marketId);
            return this;
        }

        public MarketFilter WithCountryCode(string countryCode)
        {
            CountryCodes ??= new HashSet<string>();
            CountryCodes.Add(countryCode);
            return this;
        }

        public MarketFilter WithInPlayMarketsOnly()
        {
            TurnInPlayEnabled = true;
            return this;
        }

        public MarketFilter WithMarketType(string marketType)
        {
            MarketTypes ??= new HashSet<string>();
            MarketTypes.Add(marketType);
            return this;
        }

        public MarketFilter WithVenue(string venue)
        {
            Venues ??= new HashSet<string>();
            Venues.Add(venue);
            return this;
        }

        public MarketFilter WithEventTypeId(string eventTypeId)
        {
            EventTypeIds ??= new HashSet<string>();
            EventTypeIds.Add(eventTypeId);
            return this;
        }

        public MarketFilter WithEventId(string eventId)
        {
            EventIds ??= new HashSet<string>();
            EventIds.Add(eventId);
            return this;
        }

        public MarketFilter WithBspMarketsOnly()
        {
            BspMarket = true;
            return this;
        }

        public MarketFilter WithBettingTypeOdds()
        {
            BettingTypes ??= new HashSet<string>();
            BettingTypes.Add("ODDS");
            return this;
        }

        public MarketFilter WithBettingTypeLine()
        {
            BettingTypes ??= new HashSet<string>();
            BettingTypes.Add("LINE");
            return this;
        }

        public MarketFilter WithBettingTypeRange()
        {
            BettingTypes ??= new HashSet<string>();
            BettingTypes.Add("RANGE");
            return this;
        }

        public MarketFilter WithBettingTypeAsianHandicapDoubleLine()
        {
            BettingTypes ??= new HashSet<string>();
            BettingTypes.Add("ASIAN_HANDICAP_DOUBLE_LINE");
            return this;
        }

        public MarketFilter WithBettingTypeAsianHandicapSingleLine()
        {
            BettingTypes ??= new HashSet<string>();
            BettingTypes.Add("ASIAN_HANDICAP_SINGLE_LINE");
            return this;
        }
    }
}
