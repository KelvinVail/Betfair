using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;
using Betfair.Api.Betting.Enums;

namespace Betfair.Tests.Core.Enums;

public class EnumJsonConverterTests
{
    [Fact]
#pragma warning disable CA1502 // Avoid excessive complexity
    public void AllEnumsHaveJsonConverter()
#pragma warning restore CA1502
    {
        var betfairAssembly = typeof(Side).Assembly;
        var enumTypes = betfairAssembly.GetTypes()
            .Where(t => t.IsEnum)
            .ToList();

        enumTypes.Should().NotBeEmpty("There should be enum types in the Betfair assembly");

        var enumsWithoutConverter = new List<string>();

        foreach (var enumType in enumTypes)
        {
            var jsonConverterAttribute = enumType.GetCustomAttribute<JsonConverterAttribute>();

            if (jsonConverterAttribute == null)
            {
                enumsWithoutConverter.Add(enumType.FullName ?? enumType.Name);
                continue;
            }

            var converterType = jsonConverterAttribute.ConverterType;

            // Check if it's either SnakeCaseEnumJsonConverter<T> or a custom converter
            var isSnakeCaseConverter = converterType?.IsGenericType == true &&
                                     converterType.GetGenericTypeDefinition() == typeof(SnakeCaseEnumJsonConverter<>);

            var isCustomConverter = converterType != null && typeof(JsonConverter).IsAssignableFrom(converterType) &&
                                  !isSnakeCaseConverter;

            if (!isSnakeCaseConverter && !isCustomConverter)
            {
                enumsWithoutConverter.Add($"{enumType.FullName ?? enumType.Name} (has invalid converter: {converterType?.Name ?? "null"})");
            }
        }

        enumsWithoutConverter.Should().BeEmpty(
            $"All enums should have either SnakeCaseEnumJsonConverter<T> or a custom JsonConverter. " +
            $"Enums without proper converters: {string.Join(", ", enumsWithoutConverter)}");
    }

    [Fact]
    public void AllEnumsWithSnakeCaseConverterCanSerializeAndDeserialize()
    {
        var betfairAssembly = typeof(Side).Assembly;
        var enumTypes = betfairAssembly.GetTypes()
            .Where(t => t.IsEnum)
            .Where(t => HasSnakeCaseConverter(t))
            .ToList();

        foreach (var enumType in enumTypes)
        {
            var enumValues = Enum.GetValues(enumType);

            foreach (var enumValue in enumValues)
            {
                // Test serialization
                var serialized = JsonSerializer.Serialize(enumValue);
                serialized.Should().NotBeNullOrEmpty($"Enum {enumType.Name}.{enumValue} should serialize to a non-empty string");

                // Test deserialization
                var deserialized = JsonSerializer.Deserialize(serialized, enumType);
                deserialized.Should().Be(enumValue, $"Enum {enumType.Name}.{enumValue} should deserialize back to the same value");

                // Verify it's in UPPER_SNAKE_CASE format
                var jsonValue = serialized.Trim('"');
                jsonValue.Should().MatchRegex(@"^[A-Z_]+$", $"Enum {enumType.Name}.{enumValue} should serialize to UPPER_SNAKE_CASE format, but got: {jsonValue}");
            }
        }
    }

    [Fact]
    public void AllEnumsWithCustomConverterCanSerializeAndDeserialize()
    {
        var betfairAssembly = typeof(Side).Assembly;
        var enumTypes = betfairAssembly.GetTypes()
            .Where(t => t.IsEnum)
            .Where(t => HasCustomConverter(t))
            .ToList();

        foreach (var enumType in enumTypes)
        {
            var enumValues = Enum.GetValues(enumType);

            foreach (var enumValue in enumValues)
            {
                // Test serialization
                var serialized = JsonSerializer.Serialize(enumValue);
                serialized.Should().NotBeNullOrEmpty($"Enum {enumType.Name}.{enumValue} should serialize to a non-empty string");

                // Test deserialization
                var deserialized = JsonSerializer.Deserialize(serialized, enumType);
                deserialized.Should().Be(enumValue, $"Enum {enumType.Name}.{enumValue} should deserialize back to the same value");
            }
        }
    }

    private static bool HasSnakeCaseConverter(Type enumType)
    {
        var jsonConverterAttribute = enumType.GetCustomAttribute<JsonConverterAttribute>();
        if (jsonConverterAttribute == null) return false;

        var converterType = jsonConverterAttribute.ConverterType;
        return converterType?.IsGenericType == true &&
               converterType.GetGenericTypeDefinition() == typeof(SnakeCaseEnumJsonConverter<>);
    }

#pragma warning disable CA1502 // Avoid excessive complexity
    private static bool HasCustomConverter(Type enumType)
#pragma warning restore CA1502
    {
        var jsonConverterAttribute = enumType.GetCustomAttribute<JsonConverterAttribute>();
        if (jsonConverterAttribute == null) return false;

        var converterType = jsonConverterAttribute.ConverterType;
        var isSnakeCaseConverter = converterType?.IsGenericType == true &&
                                 converterType.GetGenericTypeDefinition() == typeof(SnakeCaseEnumJsonConverter<>);

        return converterType != null && typeof(JsonConverter).IsAssignableFrom(converterType) && !isSnakeCaseConverter;
    }
}
