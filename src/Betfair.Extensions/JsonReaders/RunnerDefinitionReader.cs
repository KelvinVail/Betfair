using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.JsonReaders;

internal static class RunnerDefinitionReader
{
    private static readonly byte[] _selectionId = "id"u8.ToArray();
    private static readonly byte[] _adjustmentFactor = "adjustmentFactor"u8.ToArray();
    private static readonly byte[] _status = "status"u8.ToArray();
    private static readonly Dictionary<byte, RunnerStatus> _runnerStatusMap = new()
    {
        [(byte)'A'] = RunnerStatus.Active,
        [(byte)'W'] = RunnerStatus.Winner,
        [(byte)'L'] = RunnerStatus.Loser,
        [(byte)'R'] = RunnerStatus.Removed,
        [(byte)'H'] = RunnerStatus.Hidden,
    };

    internal static void ReadRunner(this Market market, ref Utf8JsonReader reader)
    {
        long id = 0;
        double adjustmentFactor = 0;
        RunnerStatus runnerStatus = RunnerStatus.Hidden;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            var propertyName = reader.NextProperty();
            switch (propertyName)
            {
                case var _ when propertyName.SequenceEqual(_selectionId):
                    reader.Read();
                    id = reader.GetInt64();
                    break;
                case var _ when propertyName.SequenceEqual(_adjustmentFactor):
                    reader.Read();
                    adjustmentFactor = reader.GetDouble();
                    break;
                case var _ when propertyName.SequenceEqual(_status):
                    reader.Read();
                    runnerStatus = _runnerStatusMap[reader.ValueSpan[0]];
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        market.AddOrUpdateRunnerDefinition(id, runnerStatus, adjustmentFactor);
    }
}
