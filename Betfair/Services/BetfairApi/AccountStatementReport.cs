namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class AccountStatementReport
    {
        [JsonProperty(PropertyName = "accountStatement")]
        public IList<StatementItem> AccountStatement { get; set; }

        [JsonProperty(PropertyName = "moreAvailable")]
        public bool MoreAvailable { get; set; }
    }
}