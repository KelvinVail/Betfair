using System.Text;
using Betfair.Stream;
using Betfair.Stream.Responses;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public abstract class SubscriptionTests : IDisposable
{
    private bool _disposed;

    protected SubscriptionTests()
    {
        //Subscription = new Subscription { Writer = Writer };
    }

    protected StreamWriterSpy Writer { get;  } = new ();

    protected Subscription Subscription { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) Writer.Dispose();

        _disposed = true;
    }

    protected async Task<ChangeMessage> SendLineAsync(string line)
    {
        var sendLines = new StringBuilder();
        sendLines.AppendLine(line);
        //Subscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sendLines.ToString())));
        var messages = new List<ChangeMessage>();
        await foreach (var message in Subscription.GetChanges())
        {
            messages.Add(message);
        }

        return messages[0];
    }
}