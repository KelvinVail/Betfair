using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class RunnerChangesReader
{
    internal static void ReadRunnerChanges(
        this Market market,
        ref Utf8JsonReader reader)
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
                default:
                    reader.Skip();
                    break;
            }
        }
    }
}
