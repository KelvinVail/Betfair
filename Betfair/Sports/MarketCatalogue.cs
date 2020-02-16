namespace Betfair.Sports
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    public sealed class MarketCatalogue : IDisposable
    {
        private readonly ExchangeService client;
        private readonly MarketCatalogueParams parameters = new MarketCatalogueParams();

        public MarketCatalogue(ISession session)
        {
            this.client = new ExchangeService(session, "betting");
            this.EventTypeIds = new List<string>();
            this.Countries = new List<string>();
        }

        public List<string> EventTypeIds { get; }

        public List<string> Countries { get; }

        public MarketCatalogue WithHandler(HttpClientHandler handler)
        {
            this.client.WithHandler(handler);
            return this;
        }

        public MarketCatalogue WithDateRange(DateTime from, DateTime to)
        {
            this.parameters.Filter.WithDataRange(from, to);
            return this;
        }

        public MarketCatalogue WithMarketTypeCode(string code)
        {
            this.parameters.Filter.WithMarketTypeCode(code);
            return this;
        }

        public MarketCatalogue WithMaxResults(int max)
        {
            this.parameters.MaxResults = max.ToString(CultureInfo.CurrentCulture);
            return this;
        }

        public MarketCatalogue WithSortByMinimumTraded()
        {
            this.parameters.Sort = "MINIMUM_TRADED";
            return this;
        }

        public MarketCatalogue WithSortByMaximumTraded()
        {
            this.parameters.Sort = "MAXIMUM_TRADED";
            return this;
        }

        public MarketCatalogue WithSortByMinimumAvailable()
        {
            this.parameters.Sort = "MINIMUM_AVAILABLE";
            return this;
        }

        public MarketCatalogue WithSortByMaximumAvailable()
        {
            this.parameters.Sort = "MAXIMUM_AVAILABLE";
            return this;
        }

        public MarketCatalogue WithSortByFirstToStart()
        {
            this.parameters.Sort = "FIRST_TO_START";
            return this;
        }

        public MarketCatalogue WithSortByLastToStart()
        {
            this.parameters.Sort = "LAST_TO_START";
            return this;
        }

        public MarketCatalogue WithCompetition()
        {
            this.parameters.WithMarketProjection("COMPETITION");
            return this;
        }

        public MarketCatalogue WithEvent()
        {
            this.parameters.WithMarketProjection("EVENT");
            return this;
        }

        public MarketCatalogue WithEventType()
        {
            this.parameters.WithMarketProjection("EVENT_TYPE");
            return this;
        }

        public MarketCatalogue WithMarketDescription()
        {
            this.parameters.WithMarketProjection("MARKET_DESCRIPTION");
            return this;
        }

        public MarketCatalogue WithMarketStartTime()
        {
            this.parameters.WithMarketProjection("MARKET_START_TIME");
            return this;
        }

        public MarketCatalogue WithRunnerDescription()
        {
            this.parameters.WithMarketProjection("RUNNER_DESCRIPTION");
            return this;
        }

        public MarketCatalogue WithRunnerMetadata()
        {
            this.parameters.WithMarketProjection("RUNNER_METADATA");
            return this;
        }

        public async Task RefreshAsync()
        {
            this.parameters.WithEventTypeIds(this.EventTypeIds);
            this.parameters.WithCountries(this.Countries);
            await this.client.SendParametersAsync<dynamic>("listMarketCatalogue", this.parameters);
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        [DataContract]
        private sealed class MarketCatalogueParams
        {
            internal MarketCatalogueParams()
            {
                this.Filter = new MarketCatalogueFilter();
            }

            [DataMember(Name = "filter", EmitDefaultValue = false)]
            internal MarketCatalogueFilter Filter { get; set; }

            [DataMember(Name = "marketProjection", EmitDefaultValue = false)]
            internal List<string> MarketProjection { get; set; }

            [DataMember(Name = "maxResults", EmitDefaultValue = false)]
            internal string MaxResults { get; set; }

            [DataMember(Name = "sort", EmitDefaultValue = false)]
            internal string Sort { get; set; }

            internal void WithEventTypeIds(List<string> eventTypeIds)
            {
                if (eventTypeIds.Count > 0) this.Filter.EventTypeIds = eventTypeIds;
            }

            internal void WithCountries(List<string> countries)
            {
                if (countries.Count > 0) this.Filter.MarketCountries = countries;
            }

            internal void WithMarketProjection(string projection)
            {
                if (this.MarketProjection == null)
                    this.MarketProjection = new List<string>();

                this.MarketProjection.Add(projection);
            }
        }

        [DataContract]
        private sealed class MarketCatalogueFilter
        {
            [DataMember(Name = "eventTypeIds", EmitDefaultValue = false)]
            internal List<string> EventTypeIds { get; set; }

            [DataMember(Name = "marketCountries", EmitDefaultValue = false)]
            internal List<string> MarketCountries { get; set; }

            [DataMember(Name = "marketStartTime", EmitDefaultValue = false)]
            internal DateRange MarketStartTime { get; set; }

            [DataMember(Name = "marketTypeCodes", EmitDefaultValue = false)]
            internal List<string> MarketTypeCodes { get; set; }

            internal void WithDataRange(DateTime from, DateTime to)
            {
                if (this.MarketStartTime == null)
                    this.MarketStartTime = new DateRange();

                this.MarketStartTime.WithFromDate(from);
                this.MarketStartTime.WithToDate(to);
            }

            internal void WithMarketTypeCode(string code)
            {
                if (this.MarketTypeCodes == null)
                    this.MarketTypeCodes = new List<string>();

                this.MarketTypeCodes.Add(code);
            }
        }

        [DataContract]
        private sealed class DateRange
        {
            [DataMember(Name = "from", EmitDefaultValue = false)]
            internal string From { get; set; }

            [DataMember(Name = "to", EmitDefaultValue = false)]
            internal string To { get; set; }

            internal void WithFromDate(DateTime from)
            {
                this.From = from.Date.ToString("s", new DateTimeFormatInfo()) + "Z";
            }

            internal void WithToDate(DateTime to)
            {
                this.To = to.Date.ToString("s", new DateTimeFormatInfo()) + "Z";
            }
        }
    }
}
