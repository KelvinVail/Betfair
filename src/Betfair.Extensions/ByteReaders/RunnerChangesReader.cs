using Betfair.Extensions.Markets;

namespace Betfair.Extensions.ByteReaders;

internal static class RunnerChangesReader
{
    internal static void ReadRunnerChanges(
        this Market market,
        ref BetfairJsonReader reader)
    {
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    break;
                case JsonTokenType.EndArray:
                    reader.Read();
                    return;
                case JsonTokenType.StartObject:
                    market.ReadRunnerChange(ref reader);
                    break;
            }
        }
    }
}
