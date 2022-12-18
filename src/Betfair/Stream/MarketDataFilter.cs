#nullable enable
namespace Betfair.Stream;

public class MarketDataFilter
{
    public int LadderLevels { get; private set; } = 3;

    public HashSet<string>? Fields { get; private set; }

    public MarketDataFilter WithMarketDefinition()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_MARKET_DEF");
        return this;
    }

    public MarketDataFilter WithBestPricesIncludingVirtual()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_BEST_OFFERS_DISP");
        return this;
    }

    public MarketDataFilter WithBestPrices()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_BEST_OFFERS");
        return this;
    }

    public MarketDataFilter WithFullOffersLadder()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_ALL_OFFERS");
        return this;
    }

    public MarketDataFilter WithFullTradedLadder()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_TRADED");
        return this;
    }

    public MarketDataFilter WithTradedVolume()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_TRADED_VOL");
        return this;
    }

    public MarketDataFilter WithLastTradedPrice()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_LTP");
        return this;
    }

    public MarketDataFilter WithStartingPriceLadder()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("SP_TRADED");
        return this;
    }

    public MarketDataFilter WithStartingPriceProjection()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("SP_PROJECTED");
        return this;
    }

    public MarketDataFilter WithLadderLevels(int levels)
    {
        LadderLevels = levels > 10 ? 10 : levels;
        return this;
    }
}