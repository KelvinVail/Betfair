using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for OrderChange objects.
/// </summary>
internal static class OrderChangeDeserializer
{
    /// <summary>
    /// Reads order changes array with optimized performance using FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<OrderChange>? ReadOrderChanges(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var orderChanges = ObjectPools.GetOrderChangeList();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var orderChange = ReadOrderChange(ref reader);
                if (orderChange != null)
                    orderChanges.Add(orderChange);
            }
        }

        return orderChanges.Count > 0 ? orderChanges : null;
    }

    /// <summary>
    /// Reads an OrderChange using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OrderChange? ReadOrderChange(ref FastJsonReader reader)
    {
        string? marketId = null;
        long? accountId = null;
        bool? closed = null;
        List<OrderRunnerChange>? orderRunnerChanges = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = PropertyLookup.GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Id:
                        marketId = reader.GetString();
                        break;
                    case PropertyType.AccountId:
                        accountId = reader.GetInt64();
                        break;
                    case PropertyType.Closed:
                        closed = reader.GetBoolean();
                        break;
                    case PropertyType.Orc:
                        orderRunnerChanges = OrderRunnerChangeDeserializer.ReadOrderRunnerChanges(ref reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return new OrderChange
        {
            MarketId = marketId,
            AccountId = accountId,
            Closed = closed,
            OrderRunnerChanges = orderRunnerChanges,
        };
    }
}
