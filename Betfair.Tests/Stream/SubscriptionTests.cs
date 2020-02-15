namespace Betfair.Tests.Stream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Tests.Stream.TestDoubles;
    using Betfair.Tests.TestDoubles;

    public abstract class SubscriptionTests : IDisposable
    {
        private bool disposed;

        protected SubscriptionTests()
        {
            this.Subscription = new Subscription(this.Session) { Writer = this.Writer };
        }

        protected SessionSpy Session { get; } = new SessionSpy();

        protected StreamWriterSpy Writer { get;  } = new StreamWriterSpy();

        protected Subscription Subscription { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing) this.Writer.Dispose();

            this.disposed = true;
        }

        protected async Task<ResponseMessage> SendLineAsync(string line)
        {
            var sendLines = new StringBuilder();
            sendLines.AppendLine(line);
            this.Subscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sendLines.ToString())));
            var messages = new List<ResponseMessage>();
            await foreach (var message in this.Subscription.GetChanges())
            {
                messages.Add(message);
            }

            return messages[0];
        }
    }
}
