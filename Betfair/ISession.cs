namespace Betfair
{
    using System.Threading.Tasks;

    public interface ISession
    {
        string AppKey { get; }

        Task<string> GetTokenAsync();
    }
}