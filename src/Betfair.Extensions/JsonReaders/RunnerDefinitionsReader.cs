using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class RunnerDefinitionsReader
{
    internal static void ReadRunners(this Market market, ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;

            market.ReadRunner(ref reader);
        }
    }
}
