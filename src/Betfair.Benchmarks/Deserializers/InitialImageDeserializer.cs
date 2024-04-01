using System.Text.Json;
using Betfair.Benchmarks.Responses;

namespace Betfair.Benchmarks.Deserializers;

public static class InitialImageDeserializer
{
    private static readonly byte[] _id = "id"u8.ToArray();
    private static readonly byte[] _initialClk = "initialClk"u8.ToArray();
    private static readonly byte[] _mc = "mc"u8.ToArray();

    public static MarketChange Deserialize(byte[] data)
    {
        var result = new MarketChange();
        var reader = new Utf8JsonReader(data);
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            if (reader.ValueTextEquals(_mc))
            {
                while (reader.Read())
                {
                }

                return result;
            }

            if (reader.ValueTextEquals(_id))
            {
                reader.Read();
                result.Id = reader.GetInt32();
                continue;
            }

            if (reader.ValueTextEquals(_initialClk))
            {
                reader.Read();
                result.SetInitialClock(reader.ValueSpan.ToArray());
                return result;
            }
        }

        return result;
    }
}
