namespace Betfair.Tests.Stream.TestDoubles
{
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

        public string LineWritten { get; private set; }

        public override async Task WriteLineAsync(string value)
        {
            await Task.Run(() => this.LineWritten = value);
        }
    }
}
