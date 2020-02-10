namespace Betfair.Tests.Stream.TestDoubles
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class StreamWriterSpy : StreamWriter
    {
        public StreamWriterSpy()
#pragma warning disable CA2000 // Dispose objects before losing scope
            : base(new MemoryStream())
#pragma warning restore CA2000 // Dispose objects before losing scope
        {
        }

        public string LastLineWritten { get; private set; }

        public List<string> AllLinesWritten { get; private set; } = new List<string>();

        public override async Task WriteLineAsync(string value)
        {
            await Task.Run(() =>
                {
                    this.LastLineWritten = value;
                    this.AllLinesWritten.Add(value);
                });
        }

        public void ClearPreviousResults()
        {
            this.LastLineWritten = null;
            this.AllLinesWritten = new List<string>();
        }
    }
}
