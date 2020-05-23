namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Threading.Tasks;
    using Betfair.Stream.Responses;

    public class StrategySpy : IStrategy
    {
        public string ClocksProcessed { get; private set; }

        public async Task OnChange(ChangeMessage change)
        {
            this.ClocksProcessed += change?.Clock;
            await Task.CompletedTask;
        }
    }
}
