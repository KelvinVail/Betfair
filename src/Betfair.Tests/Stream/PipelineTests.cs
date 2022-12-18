using System.Runtime.InteropServices.ComTypes;
using Betfair.Stream;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Stream;

public class PipelineTests
{
    [Fact]
    public async Task ObjectsAreSerializedAndWrittenToStream()
    {
        var ms = new MemoryStream();
        var pipe = new Pipeline(ms);

        var obj = new { Id = 1 };
        await pipe.Write(obj);

        ms.Position = 0;
        using var reader = new StreamReader(ms);
        var line = await reader.ReadLineAsync();
        var json = JsonSerializer.ToJsonString(obj, StandardResolver.ExcludeNullCamelCase);
        line.Should().Be(json);
    }

    [Fact]
    public async Task LineCanBeReadFromStream()
    {
        var ms = new MemoryStream();
        var cm = new ChangeMessage { Id = 1 };
        await JsonSerializer.SerializeAsync(ms, cm)
            .ContinueWith(_ => ms.WriteByte((byte)'\n'));

        ms.Position = 0;
        var pipe = new Pipeline(ms);
        var received = new List<ChangeMessage>();
        await foreach (var line in pipe.Read())
            received.Add(line);

        received.Should().Contain(x => x.Id == 1);
    }
}
