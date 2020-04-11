﻿namespace Betfair.Betting.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public class ExchangeClientSpy : IExchangeClient
    {
        private string returnContent;

        public string Endpoint { get; private set; }

        public string Parameters { get; private set; }

        public string BetfairMethod { get; private set; }

        public ExchangeClientSpy WithReturnContent(string content)
        {
            this.returnContent = content;
            return this;
        }

        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            this.Endpoint = endpoint;
            this.BetfairMethod = betfairMethod;
            this.Parameters = parameters;
            return await Task.FromResult(this.returnContent != null ? JsonSerializer.Deserialize<T>(this.returnContent, StandardResolver.AllowPrivateExcludeNull) : default);
        }
    }
}
