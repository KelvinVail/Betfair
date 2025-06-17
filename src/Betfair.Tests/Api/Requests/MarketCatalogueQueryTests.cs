using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;

namespace Betfair.Tests.Api.Requests;

public class MarketCatalogueQueryTests
{
    private readonly MarketCatalogueQuery _query = new ();

    [Fact]
    public void IsCreatedWithNullMarketProjection() =>
        _query.MarketProjection.Should().BeNull();

    [Fact]
    public void CanAddAMarketProjection()
    {
        _query.Include(MarketProjection.MarketStartTime);

        _query.MarketProjection.Should()
            .Contain(MarketProjection.MarketStartTime);
    }

    [Fact]
    public void IsCreatedWithNullSortOrder() =>
        _query.Sort.Should().BeNull();

    [Fact]
    public void CanAddASortOrder()
    {
        _query.OrderBy(MarketSort.FirstToStart);

        _query.Sort.Should()
            .Be(MarketSort.FirstToStart);
    }

    [Fact]
    public void IsCreatedWithMaxResultsOf1000() =>
        _query.MaxResults.Should().Be(1000);

    [Theory]
    [InlineData(10)]
    [InlineData(99)]
    [InlineData(392)]
    public void MaxResultsCanBeSet(int value)
    {
        _query.Take(value);

        _query.MaxResults.Should().Be(value);
    }
}
