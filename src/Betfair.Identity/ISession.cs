using System.Threading.Tasks;

namespace Betfair.Identity
{
    public interface ISession
    {
        string AppKey { get; }

        Task<string> GetTokenAsync();
    }
}