namespace Betfair.Tests.TestDoubles
{
    using System.IO;

    public class StreamReaderSpy : StreamReader
    {
        public StreamReaderSpy(Stream stream)
            : base(stream)
        {
        }
    }
}
