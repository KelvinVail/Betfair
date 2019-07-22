namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The market book request.
    /// </summary>
    public class MarketBookRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketBookRequest"/> class.
        /// </summary>
        /// <param name="marketId">
        /// The market id.
        /// </param>
        public MarketBookRequest(string marketId)
        {
            const MatchProjection MatchProjection = MatchProjection.ROLLED_UP_BY_AVG_PRICE;
            const OrderProjection OrderProjection = OrderProjection.EXECUTABLE;
            var marketIds = new List<string> { marketId };
            var priceProjection = new PriceProjection
            {
                PriceData = new HashSet<PriceData>() { PriceData.EX_BEST_OFFERS }
            };

            this.Params = new MarketBookParams()
            {
                MatchProjection = MatchProjection,
                OrderProjection = OrderProjection,
                MarketIds = marketIds,
                PriceProjection = priceProjection
            };
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; } = 1;

        /// <summary>
        /// Gets the JSONRPC.
        /// </summary>
        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc { get; } = "2.0";

        /// <summary>
        /// Gets the method.
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        public string Method { get; } = "SportsAPING/v1.0/listMarketBook";

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        [JsonProperty(PropertyName = "params")]
        public MarketBookParams Params { get; }
    }
}