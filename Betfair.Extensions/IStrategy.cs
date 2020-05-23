namespace Betfair.Extensions
{
    using System.Threading.Tasks;
    using Betfair.Stream.Responses;

    public interface IStrategy
    {
        public Task OnChange(ChangeMessage change);
    }
}