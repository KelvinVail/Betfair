namespace Betfair.Betting
{
    using System.Threading.Tasks;

    public interface IExchangeClient
    {
        Task<T> SendAsync<T>(string endpoint, string betfairMethod, string parameters);
    }
}