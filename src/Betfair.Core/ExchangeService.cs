using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Betfair.Exchange.Interfaces;
using Betfair.Identity;
using Microsoft.Extensions.Logging;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Core
{
    public sealed class ExchangeService : IExchangeService, IDisposable
    {
        private readonly ISession _session;
        private readonly ILogger _log;
        private readonly ExchangeClient _client = new ExchangeClient("api");

        public ExchangeService(ISession session, ILogger log)
        {
            _session = session;
            _log = log;
        }

        public ExchangeService WithHandler(HttpClientHandler handler)
        {
            _client.WithHandler(handler);
            return this;
        }

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            var request = await GetRequest(endpoint, betfairMethod, parameters);
            var response = await _client.SendAsync<ExchangeResponse<T>>(request);
            LogResponse(response.ToJson());
            if (!string.IsNullOrEmpty(response.Error?.ToString())) throw new HttpRequestException(response.Error.Data.Exception.ErrorCode);
            return response.Result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static string GetBody(string method, string parameters)
        {
            return "{\"jsonrpc\":\"2.0\"," +
                   $"\"method\":\"SportsAPING/v1.0/{method}\"," +
                   $"\"id\":1,\"params\":{parameters}}}";
        }

        private async Task<HttpRequestMessage> GetRequest(string endpoint, string method, string parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"exchange/{endpoint}/json-rpc/v1");
            request.Headers.Add("X-Authentication", await _session.GetTokenAsync());
            request.Headers.Add("X-Application", _session.AppKey);

            var body = GetBody(method, parameters);
            request.Content = new StringContent(body);
            LogRequest(body);

            return request;
        }

        private void LogRequest(string requestString)
        {
            _log.LogInformation($"Betfair API called: {requestString}.");
        }

        private void LogResponse(string responseString)
        {
            _log.LogInformation($"Betfair API responded: {responseString}.");
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

            public string ToJson()
            {
                return JsonSerializer.ToJsonString(this, StandardResolver.AllowPrivateExcludeNull);
            }
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
