using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for MarketDefinition objects.
/// </summary>
internal static class MarketDefinitionDeserializer
{
    /// <summary>
    /// Reads a MarketDefinition using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static MarketDefinition? ReadMarketDefinition(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            return null;

        bool? bspMarket = null;
        bool? turnInPlayEnabled = null;
        bool? persistenceEnabled = null;
        double? marketBaseRate = null;
        string? bettingType = null;
        string? status = null;
        string? venue = null;
        DateTime? settledTime = null;
        string? timezone = null;
        double? eachWayDivisor = null;
        List<string>? regulators = null;
        string? marketType = null;
        int? numberOfWinners = null;
        string? countryCode = null;
        bool? inPlay = null;
        int? betDelay = null;
        int? numberOfActiveRunners = null;
        string? eventId = null;
        bool? crossMatching = null;
        bool? runnersVoidable = null;
        DateTime? suspendTime = null;
        bool? discountAllowed = null;
        List<RunnerDefinition>? runners = null;
        long? version = null;
        string? eventTypeId = null;
        bool? complete = null;
        DateTime? openDate = null;
        DateTime? marketTime = null;
        bool? bspReconciled = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyName = reader.ValueSpan;
                var propertyType = PropertyLookup.GetPropertyType(propertyName);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.BspMarket:
                        bspMarket = reader.GetBoolean();
                        break;
                    case PropertyType.TurnInPlayEnabled:
                        turnInPlayEnabled = reader.GetBoolean();
                        break;
                    case PropertyType.PersistenceEnabled:
                        persistenceEnabled = reader.GetBoolean();
                        break;
                    case PropertyType.MarketBaseRate:
                        marketBaseRate = reader.GetNullableDouble();
                        break;
                    case PropertyType.BettingType:
                        bettingType = reader.GetString();
                        break;
                    case PropertyType.Status:
                        status = reader.GetString();
                        break;
                    case PropertyType.Venue:
                        venue = reader.GetString();
                        break;
                    case PropertyType.Timezone:
                        timezone = reader.GetString();
                        break;
                    case PropertyType.SettledTime:
                        settledTime = reader.GetNullableDateTime();
                        break;
                    case PropertyType.SuspendTime:
                        suspendTime = reader.GetNullableDateTime();
                        break;
                    case PropertyType.OpenDate:
                        openDate = reader.GetNullableDateTime();
                        break;
                    case PropertyType.MarketTime:
                        marketTime = reader.GetNullableDateTime();
                        break;
                    case PropertyType.EachWayDivisor:
                        eachWayDivisor = reader.GetNullableDouble();
                        break;
                    case PropertyType.Regulators:
                        regulators = StringArrayDeserializer.ReadStringArray(ref reader);
                        break;
                    case PropertyType.MarketType:
                        marketType = reader.GetString();
                        break;
                    case PropertyType.NumberOfWinners:
                        numberOfWinners = reader.GetInt32();
                        break;
                    case PropertyType.CountryCode:
                        countryCode = reader.GetString();
                        break;
                    case PropertyType.InPlay:
                        inPlay = reader.GetBoolean();
                        break;
                    case PropertyType.BetDelay:
                        betDelay = reader.GetInt32();
                        break;
                    case PropertyType.NumberOfActiveRunners:
                        numberOfActiveRunners = reader.GetInt32();
                        break;
                    case PropertyType.EventId:
                        eventId = reader.GetString();
                        break;
                    case PropertyType.CrossMatching:
                        crossMatching = reader.GetBoolean();
                        break;
                    case PropertyType.RunnersVoidable:
                        runnersVoidable = reader.GetBoolean();
                        break;
                    case PropertyType.DiscountAllowed:
                        discountAllowed = reader.GetBoolean();
                        break;
                    case PropertyType.Runners:
                        runners = RunnerDefinitionDeserializer.ReadRunnerDefinitions(ref reader);
                        break;
                    case PropertyType.Version:
                        version = reader.GetInt64();
                        break;
                    case PropertyType.EventTypeId:
                        eventTypeId = reader.GetString();
                        break;
                    case PropertyType.Complete:
                        complete = reader.GetBoolean();
                        break;
                    case PropertyType.BspReconciled:
                        bspReconciled = reader.GetBoolean();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return new MarketDefinition
        {
            BspMarket = bspMarket,
            TurnInPlayEnabled = turnInPlayEnabled,
            PersistenceEnabled = persistenceEnabled,
            MarketBaseRate = marketBaseRate,
            BettingType = bettingType,
            Status = status,
            Venue = venue,
            SettledTime = settledTime,
            Timezone = timezone,
            EachWayDivisor = eachWayDivisor,
            Regulators = regulators,
            MarketType = marketType,
            NumberOfWinners = numberOfWinners,
            CountryCode = countryCode,
            InPlay = inPlay,
            BetDelay = betDelay,
            NumberOfActiveRunners = numberOfActiveRunners,
            EventId = eventId,
            CrossMatching = crossMatching,
            RunnersVoidable = runnersVoidable,
            SuspendTime = suspendTime,
            DiscountAllowed = discountAllowed,
            Runners = runners,
            Version = version,
            EventTypeId = eventTypeId,
            Complete = complete,
            OpenDate = openDate,
            MarketTime = marketTime,
            BspReconciled = bspReconciled,
        };
    }
}
