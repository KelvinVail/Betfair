using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Betfair.Stream.Responses;
using Betfair.Stream.Tests.TestDoubles;

namespace Betfair.Stream.Tests
{
    public abstract class SubscriptionTests : IDisposable
    {
        private bool _disposed;

        protected SubscriptionTests()
        {
            Subscription = new Subscription(Session) { Writer = Writer };
        }

        protected SessionSpy Session { get; } = new SessionSpy();

        protected StreamWriterSpy Writer { get;  } = new StreamWriterSpy();

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
            Subscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sendLines.ToString())));
            var messages = new List<ChangeMessage>();
            await foreach (var message in Subscription.GetChanges())
            {
                messages.Add(message);
            }

            return messages[0];
        }
    }
}
