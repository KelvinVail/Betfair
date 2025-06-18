using Betfair.Api.Accounts.Endpoints.GetAccountFunds.Responses;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountFunds.Responses;

public class AccountFundsResponseTests
{
    [Fact]
    public void CanSerializeResponseWithAllProperties()
    {
        var response = new AccountFundsResponse
        {
            AvailableToBetBalance = 1000.50,
            Exposure = 250.75,
            RetainedCommission = 15.25,
            ExposureLimit = 5000.00,
            DiscountRate = 0.05,
            PointsBalance = 100,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountFundsResponse);

        json.Should().Contain("\"availableToBetBalance\":1000.5");
        json.Should().Contain("\"exposure\":250.75");
        json.Should().Contain("\"retainedCommission\":15.25");
        json.Should().Contain("\"exposureLimit\":5000");
        json.Should().Contain("\"discountRate\":0.05");
        json.Should().Contain("\"pointsBalance\":100");
    }

    [Fact]
    public void CanDeserializeResponseWithAllProperties()
    {
        const string json = """
            {
                "availableToBetBalance": 2500.75,
                "exposure": 500.25,
                "retainedCommission": 25.50,
                "exposureLimit": 10000.00,
                "discountRate": 0.03,
                "pointsBalance": 250
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        response.Should().NotBeNull();
        response!.AvailableToBetBalance.Should().Be(2500.75);
        response.Exposure.Should().Be(500.25);
        response.RetainedCommission.Should().Be(25.50);
        response.ExposureLimit.Should().Be(10000.00);
        response.DiscountRate.Should().Be(0.03);
        response.PointsBalance.Should().Be(250);
    }

    [Fact]
    public void CanDeserializeResponseWithZeroValues()
    {
        const string json = """
            {
                "availableToBetBalance": 0.0,
                "exposure": 0.0,
                "retainedCommission": 0.0,
                "exposureLimit": 0.0,
                "discountRate": 0.0,
                "pointsBalance": 0
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        response.Should().NotBeNull();
        response!.AvailableToBetBalance.Should().Be(0.0);
        response.Exposure.Should().Be(0.0);
        response.RetainedCommission.Should().Be(0.0);
        response.ExposureLimit.Should().Be(0.0);
        response.DiscountRate.Should().Be(0.0);
        response.PointsBalance.Should().Be(0);
    }

    [Fact]
    public void CanDeserializeResponseWithNegativeValues()
    {
        const string json = """
            {
                "availableToBetBalance": -500.25,
                "exposure": -100.50,
                "retainedCommission": -25.75,
                "exposureLimit": -1000.00,
                "discountRate": -0.02,
                "pointsBalance": -50
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        response.Should().NotBeNull();
        response!.AvailableToBetBalance.Should().Be(-500.25);
        response.Exposure.Should().Be(-100.50);
        response.RetainedCommission.Should().Be(-25.75);
        response.ExposureLimit.Should().Be(-1000.00);
        response.DiscountRate.Should().Be(-0.02);
        response.PointsBalance.Should().Be(-50);
    }

    [Fact]
    public void CanDeserializeResponseWithLargeValues()
    {
        const string json = """
            {
                "availableToBetBalance": 999999.99,
                "exposure": 888888.88,
                "retainedCommission": 777777.77,
                "exposureLimit": 1000000.00,
                "discountRate": 0.99,
                "pointsBalance": 999999
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        response.Should().NotBeNull();
        response!.AvailableToBetBalance.Should().Be(999999.99);
        response.Exposure.Should().Be(888888.88);
        response.RetainedCommission.Should().Be(777777.77);
        response.ExposureLimit.Should().Be(1000000.00);
        response.DiscountRate.Should().Be(0.99);
        response.PointsBalance.Should().Be(999999);
    }

    [Fact]
    public void CanDeserializeResponseWithMissingProperties()
    {
        const string json = """
            {
                "discountRate": 0.02,
                "pointsBalance": 50
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        response.Should().NotBeNull();
        response!.AvailableToBetBalance.Should().Be(0.0);
        response.Exposure.Should().Be(0.0);
        response.RetainedCommission.Should().Be(0.0);
        response.ExposureLimit.Should().Be(0.0);
        response.DiscountRate.Should().Be(0.02);
        response.PointsBalance.Should().Be(50);
    }

    [Fact]
    public void CanSerializeAndDeserializeRoundTrip()
    {
        var originalResponse = new AccountFundsResponse
        {
            AvailableToBetBalance = 1500.25,
            Exposure = 300.50,
            RetainedCommission = 45.75,
            ExposureLimit = 7500.00,
            DiscountRate = 0.04,
            PointsBalance = 175,
        };

        var json = JsonSerializer.Serialize(originalResponse, SerializerContext.Default.AccountFundsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        deserializedResponse.Should().BeEquivalentTo(originalResponse);
    }

    [Fact]
    public void CanDeserializeResponseWithExtraProperties()
    {
        const string json = """
            {
                "availableToBetBalance": 1000.00,
                "exposure": 200.00,
                "retainedCommission": 10.00,
                "exposureLimit": 5000.00,
                "discountRate": 0.05,
                "pointsBalance": 100,
                "extraProperty": "extraValue",
                "anotherProperty": 123
            }
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        response.Should().NotBeNull();
        response!.AvailableToBetBalance.Should().Be(1000.00);
        response.Exposure.Should().Be(200.00);
        response.RetainedCommission.Should().Be(10.00);
        response.ExposureLimit.Should().Be(5000.00);
        response.DiscountRate.Should().Be(0.05);
        response.PointsBalance.Should().Be(100);

        // Extra properties should be ignored during deserialization
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(100.50)]
    [InlineData(-50.25)]
    [InlineData(999999.99)]
    [InlineData(0.01)]
    [InlineData(1000000.00)]
    public void CanHandleDifferentAvailableToBetBalanceValues(double balance)
    {
        var response = new AccountFundsResponse
        {
            AvailableToBetBalance = balance,
            DiscountRate = 0.05,
            PointsBalance = 100,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountFundsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        deserializedResponse!.AvailableToBetBalance.Should().Be(balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(-50)]
    [InlineData(999999)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void CanHandleDifferentPointsBalanceValues(int points)
    {
        var response = new AccountFundsResponse
        {
            AvailableToBetBalance = 1000.00,
            DiscountRate = 0.05,
            PointsBalance = points,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountFundsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        deserializedResponse!.PointsBalance.Should().Be(points);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.05)]
    [InlineData(-0.02)]
    [InlineData(0.99)]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    public void CanHandleDifferentDiscountRateValues(double discountRate)
    {
        var response = new AccountFundsResponse
        {
            AvailableToBetBalance = 1000.00,
            DiscountRate = discountRate,
            PointsBalance = 100,
        };

        var json = JsonSerializer.Serialize(response, SerializerContext.Default.AccountFundsResponse);
        var deserializedResponse = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountFundsResponse);

        deserializedResponse!.DiscountRate.Should().Be(discountRate);
    }

    [Fact]
    public void ResponseShouldBePublicClass()
    {
        var responseType = typeof(AccountFundsResponse);

        responseType.IsPublic.Should().BeTrue();
    }

    [Fact]
    public void AllPropertiesShouldHaveInitAccessors()
    {
        var properties = typeof(AccountFundsResponse).GetProperties();

        foreach (var property in properties)
        {
            property.CanRead.Should().BeTrue($"Property {property.Name} should be readable");
            property.SetMethod.Should().NotBeNull($"Property {property.Name} should have a setter");
        }
    }

    [Fact]
    public void AllPropertiesShouldHaveCorrectTypes()
    {
        var response = new AccountFundsResponse();

        response.AvailableToBetBalance.Should().BeOfType(typeof(double));
        response.Exposure.Should().BeOfType(typeof(double));
        response.RetainedCommission.Should().BeOfType(typeof(double));
        response.ExposureLimit.Should().BeOfType(typeof(double));
        response.DiscountRate.Should().BeOfType(typeof(double));
        response.PointsBalance.Should().BeOfType(typeof(int));
    }
}
