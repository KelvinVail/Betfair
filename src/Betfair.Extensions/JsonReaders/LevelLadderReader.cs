namespace Betfair.Extensions.JsonReaders;

internal static class LevelLadderReader
{
    internal static void ReadLevelLadder(this ref Utf8JsonReader reader, (int, double, double)[] levels)
    {
        reader.Read();

        var count = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            levels[count] = reader.ReadLevel();
            count++;
        }
    }

    private static (int, double, double) ReadLevel(this ref Utf8JsonReader reader)
    {
        reader.Read();

        var level = reader.GetInt32();
        reader.Read();

        var price = reader.GetDouble();
        reader.Read();

        var size = reader.GetDouble();
        reader.Read();

        return (level, price, size);
    }
}
