namespace Betfair.Exchange.Interfaces
{
    using System.Threading.Tasks;

    public interface IExchangeService
    {
        Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters);
    }
}