using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;

namespace Betfair.Tests.Core.Enums;

public class OrderByTests
{
    [Theory]
    [InlineData(OrderBy.Market, "BY_MARKET")]
    [InlineData(OrderBy.MatchTime, "BY_MATCH_TIME")]
    [InlineData(OrderBy.PlaceTime, "BY_PLACE_TIME")]
    [InlineData(OrderBy.SettledTime, "BY_SETTLED_TIME")]
    [InlineData(OrderBy.VoidTime, "BY_VOID_TIME")]
    public void CanSerializeOrderByToJsonString(OrderBy orderBy, string expectedJson)
    {
        var json = JsonSerializer.Serialize(orderBy);
        json.Should().Be($"\"{expectedJson}\"");
    }

    [Theory]
    [InlineData("BY_MARKET", OrderBy.Market)]
    [InlineData("BY_MATCH_TIME", OrderBy.MatchTime)]
    [InlineData("BY_PLACE_TIME", OrderBy.PlaceTime)]
    [InlineData("BY_SETTLED_TIME", OrderBy.SettledTime)]
    [InlineData("BY_VOID_TIME", OrderBy.VoidTime)]
    public void CanDeserializeOrderByFromJsonString(string json, OrderBy expectedOrderBy)
    {
        var orderBy = JsonSerializer.Deserialize<OrderBy>($"\"{json}\"");
        orderBy.Should().Be(expectedOrderBy);
    }
}
