﻿using System.Threading.Tasks;
using Betfair.Exchange.Interfaces;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class ExchangeStub : IExchangeService
    {
        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            return await Task.FromResult((T)default);
        }
    }
}
