using System.Text;
using Betfair.Extensions.Contracts;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.Markets;

public class SubscriptionMock : ISubscription
{
    private readonly List<byte[]> _byteLines = [];
    private readonly List<ChangeMessage> _changeMessages = [];

    public SubscriptionMock(string? dataPath = null)
    {
        if (dataPath is null) return;

        // Read the first line of the file.
        var isMultiLine = File.ReadLines(dataPath).First().Length != 1;

        if (!isMultiLine)
        {
            var line = File.ReadAllText(dataPath);
            var cleanLine = line.Replace("\r", string.Empty, StringComparison.OrdinalIgnoreCase).Replace("\n", string.Empty, StringComparison.OrdinalIgnoreCase);
            _byteLines.Add(Encoding.UTF8.GetBytes(cleanLine));

            var changeMessage = JsonSerializer.Deserialize<ChangeMessage>(line);
            if (changeMessage is null) return;
            _changeMessages.Add(changeMessage);

            return;
        }

        foreach (var line in File.ReadAllLines(dataPath))
        {
            _byteLines.Add(Encoding.UTF8.GetBytes(line));

            var changeMessage = JsonSerializer.Deserialize<ChangeMessage>(line);
            if (changeMessage is null) continue;
            _changeMessages.Add(changeMessage);
        }
    }

    public bool SubscribedCalled { get; private set; }

    public bool SubscribedToOrdersCalled { get; private set; }

    public StreamMarketFilter? MarketFilter { get; private set; }

    public DataFilter? DataFilter { get; private set; }

    public Task Subscribe(
        StreamMarketFilter marketFilter,
        DataFilter? dataFilter = null,
        TimeSpan? conflate = null,
        CancellationToken cancellationToken = default)
    {
        SubscribedCalled = true;
        MarketFilter = marketFilter;
        DataFilter = dataFilter;

        return Task.CompletedTask;
    }

    public Task SubscribeToOrders(
        OrderFilter? orderFilter = null,
        TimeSpan? conflate = null,
        CancellationToken cancellationToken = default)
    {
        SubscribedToOrdersCalled = true;

        return Task.CompletedTask;
    }

    public IAsyncEnumerable<ChangeMessage> ReadLines(CancellationToken cancellationToken)
    {
        return _changeMessages.ToAsyncEnumerable();
    }

    public IAsyncEnumerable<byte[]> ReadBytes(CancellationToken cancellationToken)
    {
        return _byteLines.ToAsyncEnumerable();
    }
}
