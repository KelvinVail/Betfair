namespace Betfair.Account
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public abstract class BetfairAccountBase : HttpClientBase
    {
        private readonly ISession session;

        protected BetfairAccountBase(ISession session)
            : base(new Uri("https://api.betfair.com/exchange/account/json-rpc/v1"))
        {
            this.session = session;
        }

        protected async Task<T> SendAsync<T>(string method)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            request.Headers.Add("X-Application", this.session.AppKey);
            var token = await this.session.GetSessionTokenAsync();
            request.Headers.Add("X-Authentication", token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new AccountRequest(method)));
            var response = await this.SendAsync<AccountResponse<T>>(request);
            if (response.Error != null) throw new HttpRequestException(response.Error);
            return response.Result;
        }

        private sealed class AccountRequest
        {
            internal AccountRequest(string method)
            {
                this.Method = $"AccountAPING/v1.0/{method}";
                this.Id = 1;
                this.Jsonrpc = "2.0";
            }

            [JsonProperty(PropertyName = "jsonrpc")]
            internal string Jsonrpc { get; set; }

            [JsonProperty(PropertyName = "id")]
            internal int Id { get; set; }

            [JsonProperty(PropertyName = "method")]
            internal string Method { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        private sealed class AccountResponse<T>
        {
            [JsonProperty(PropertyName = "result")]
            internal T Result { get; set; }

            [JsonProperty(PropertyName = "error")]
            internal string Error { get; set; }
        }
    }
}
