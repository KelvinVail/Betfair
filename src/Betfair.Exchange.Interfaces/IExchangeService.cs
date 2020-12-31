using System.Threading.Tasks;

namespace Betfair.Exchange.Interfaces
{
    public interface IExchangeService
    {
        Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters);
    }
}