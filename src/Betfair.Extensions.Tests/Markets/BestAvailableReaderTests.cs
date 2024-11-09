using System.Text;
using Betfair.Core.Login;
using Betfair.Extensions.JsonReaders;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Tests.TestDoubles;

namespace Betfair.Extensions.Tests.Markets;

public class BestAvailableReaderTests
{
    private readonly byte[] _byteLine;
    private readonly Market _market;

    public BestAvailableReaderTests()
    {
        var path = Path.Combine("Data", "BestAvailable", "initialImage.json");

        var line = File.ReadAllText(path);
        var cleanLine = line.Replace("\r", string.Empty, StringComparison.OrdinalIgnoreCase).Replace("\n", string.Empty, StringComparison.OrdinalIgnoreCase);
        _byteLine = Encoding.UTF8.GetBytes(cleanLine);

        _market = Market.Create(new Credentials("u", "p", "k"), "1.235123059", new SubscriptionStub()).Value;

        var options = new JsonReaderOptions
        {
            CommentHandling = JsonCommentHandling.Disallow,
        };

        var reader = new Utf8JsonReader(_byteLine, options);
        _market.ReadChangeMessage(ref reader);
    }

    [Theory(Skip = "x")]
    [InlineData(9517643, 30)]
    public void LevelZeroBackPriceShouldBe(long id, double value)
    {
        var price = Price.Of(value);

        var runner = _market.Runners.Single(x => x.Id == id);

        // runner.BestAvailable.Back(0).Price.Should().Be(price);
    }
}
