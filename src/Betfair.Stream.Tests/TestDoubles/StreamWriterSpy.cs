﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Betfair.Stream.Tests.TestDoubles
{
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
                    LastLineWritten = value;
                    AllLinesWritten.Add(value);
                });
        }

        public void ClearPreviousResults()
        {
            LastLineWritten = null;
            AllLinesWritten = new List<string>();
        }
    }
}
