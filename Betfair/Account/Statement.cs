namespace Betfair.Account
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public sealed class Statement : IDisposable
    {
        private readonly AccountService client;

        public Statement(ISession session)
        {
            this.client = new AccountService(session);
        }

        public IReadOnlyList<StatementItem> Items { get; private set; }

        public Statement WithHandler(HttpClientHandler handler)
        {
            this.client.WithHandler(handler);
            return this;
        }

        public async Task RefreshAsync()
        {
            var statement = await this.client.SendAsync<AccountStatementResponse>("getAccountStatement");
            var items = new List<StatementItem>();
            statement.StatementItems.ForEach(item =>
                items.Add(new StatementItem
                {
                    RefId = item.RefId,
                    ItemDate = item.ItemDate,
                    Amount = item.Amount,
                    Balance = item.Balance,
                }));
            this.Items = items;
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        private sealed class AccountStatementResponse
        {
            [JsonProperty(PropertyName = "accountStatement")]
            internal List<StatementItemResponse> StatementItems { get; private set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        private sealed class StatementItemResponse
        {
            [JsonProperty(PropertyName = "refId")]
            internal string RefId { get; private set; }

            [JsonProperty(PropertyName = "itemDate")]
            internal DateTime ItemDate { get; private set; }

            [JsonProperty(PropertyName = "amount")]
            internal double Amount { get; private set; }

            [JsonProperty(PropertyName = "balance")]
            internal double Balance { get; private set; }
        }
    }
}
