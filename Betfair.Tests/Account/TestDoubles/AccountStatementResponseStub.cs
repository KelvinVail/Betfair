namespace Betfair.Tests.Account.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AccountStatementResponseStub
    {
        [JsonProperty(PropertyName = "accountStatement")]
        public List<StatementItemStub> StatementItems { get; } = new List<StatementItemStub>();

        [JsonProperty(PropertyName = "moreAvailable")]
        public bool MoreAvailable { get; set; }

        public void AddItem(string refId, DateTime date, double amount, double balance)
        {
            var item = new StatementItemStub
            {
                RefId = refId,
                ItemDate = date,
                Amount = amount,
                Balance = balance,
            };

            this.StatementItems.Add(item);
        }
    }
}
