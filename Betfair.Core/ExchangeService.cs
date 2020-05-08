namespace Betfair.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;
    using Betfair.Identity;

    public sealed class ExchangeService : IExchangeService, IDisposable
    {
        private readonly ISession session;

        private readonly ExchangeClient client = new ExchangeClient(new Uri("https://api.betfair.com/exchange/"));

        public ExchangeService(ISession session)
        {
            this.session = session;
        }

        public ExchangeService WithHandler(HttpClientHandler handler)
        {
            this.client.WithHandler(handler);
            return this;
        }

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            var response = await this.client.SendAsync<ExchangeResponse<T>>(await this.GetRequest(endpoint, betfairMethod, parameters));
            if (!string.IsNullOrEmpty(response.Error?.ToString())) throw new HttpRequestException(response.Error.Data.Exception.ErrorCode);
            return response.Result;
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        private static string GetBody(string method, string parameters)
        {
            return "{\"jsonrpc\":\"2.0\"," +
                   $"\"method\":\"SportsAPING/v1.0/{method}\"," +
                   $"\"id\":1,\"params\":{parameters}}}";
        }

        private async Task<HttpRequestMessage> GetRequest(string endpoint, string method, string parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/json-rpc/v1");
            request.Headers.Add("X-Authentication", await this.session.GetTokenAsync());
            request.Headers.Add("X-Application", this.session.AppKey);
            request.Content = new StringContent(GetBody(method, parameters));

            return request;
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize http response.")]
        [DataContract]
        private sealed class ExchangeResponse<T>
        {
            [DataMember(Name = "result", EmitDefaultValue = false)]
            public T Result { get; set; }

            [DataMember(Name = "error", EmitDefaultValue = false)]
            public ExchangeError Error { get; set; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize http response.")]
        [DataContract]
        private sealed class ExchangeError
        {
            [DataMember(Name = "data", EmitDefaultValue = false)]
            public ExceptionData Data { get; set; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize http response.")]
        [DataContract]
        private sealed class ExceptionData
        {
            [DataMember(Name = "APINGException", EmitDefaultValue = false)]
            public ExchangeException Exception { get; set; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize http response.")]
        [DataContract]
        private sealed class ExchangeException
        {
            [DataMember(Name = "errorCode", EmitDefaultValue = false)]
            public string ErrorCode { get; set; }
        }
    }
}
