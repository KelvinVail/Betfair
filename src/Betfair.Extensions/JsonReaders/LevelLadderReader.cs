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
        int level = -1;
        double price = 0;
        double size = 0;

        reader.Read();
        level = reader.GetInt32();
        reader.Read();
        price = reader.GetDouble();
        reader.Read();
        size = reader.GetDouble();
        reader.Read();

        return (level, price, size);
    }
}
