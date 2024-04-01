using System.Text.Json;

namespace Betfair.Benchmarks.Deserializers;

public static class StatusMessageDeserializer
{
    public static (int, bool) Deserialize(byte[] data)
    {
        var reader = new Utf8JsonReader(data);
        int count = 0;
        bool isClosed = true;
        while (reader.Read())
        {
            count++;
            if (count == 5) isClosed = reader.GetBoolean();
            if (count == 7) return (reader.GetInt32(), isClosed);
        }

        return (0, true);
    }
}
