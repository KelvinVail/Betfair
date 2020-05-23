namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;

    public class ExchangeStub : IExchangeService
    {
        public async Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters)
        {
            return await Task.FromResult((T)default);
        }
    }
}
