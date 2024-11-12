using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class RunnerChangeReader
{
    private static readonly byte[] _batb = "batb"u8.ToArray();
    private static readonly byte[] _batl = "batl"u8.ToArray();
    private static readonly byte[] _selectionId = "id"u8.ToArray();
    private static readonly (int, double, double)[] _batbLevels = new (int, double, double)[9];
    private static readonly (int, double, double)[] _batlLevels = new (int, double, double)[9];

    internal static void ReadRunnerChange(
        this Market market,
        ref Utf8JsonReader reader)
    {
        long selectionId = 0;
        Array.Clear(_batbLevels, 0, _batbLevels.Length);
        Array.Clear(_batlLevels, 0, _batlLevels.Length);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            var propertyName = reader.NextProperty();

            switch (propertyName)
            {
                case var _ when propertyName.SequenceEqual(_batb):
                    reader.ReadLevelLadder(_batbLevels);
                    break;
                case var _ when propertyName.SequenceEqual(_batl):
                    reader.ReadLevelLadder(_batlLevels);
                    break;
                case var _ when propertyName.SequenceEqual(_selectionId):
                    reader.Read();
                    selectionId = reader.GetInt64();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        for (int i = 0; i < _batbLevels.Length; i++)
        {
            var level = _batbLevels[i];
            if (level is { Item2: 0 }) break;
            market.UpdateBestAvailableToBack(selectionId, level.Item1, level.Item2, level.Item3);
        }

        for (int i = 0; i < _batlLevels.Length; i++)
        {
            var level = _batlLevels[i];
            if (level is { Item2: 0 }) break;
            market.UpdateBestAvailableToLay(selectionId, level.Item1, level.Item2, level.Item3);
        }
    }
}
