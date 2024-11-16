namespace Betfair.Extensions.ByteReaders;

internal static class LevelLadderReader
{
    internal static void ReadLevelLadder(this ref BetfairJsonReader reader, (int, double, double)[] levels)
    {
        reader.Read();

        var count = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            levels[count] = reader.ReadLevel();
        }
    }

    private static (int, double, double) ReadLevel(this ref BetfairJsonReader reader)
    {
        reader.Read();

        var level = reader.GetInt32();
        reader.Read();
        reader.Read();

        var price = reader.GetDouble();
        reader.Read();
        reader.Read();

        var size = reader.GetDouble();
        reader.Read();

        return (level, price, size);
    }
}
