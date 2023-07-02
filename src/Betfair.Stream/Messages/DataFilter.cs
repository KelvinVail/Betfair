namespace Betfair.Stream.Messages;

public class DataFilter
{
    public int LadderLevels { get; private set; } = 3;

    public HashSet<string>? Fields { get; private set; }

    public DataFilter WithMarketDefinition()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_MARKET_DEF");
        return this;
    }

    public DataFilter WithBestPricesIncludingVirtual()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_BEST_OFFERS_DISP");
        return this;
    }

    public DataFilter WithBestPrices()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_BEST_OFFERS");
        return this;
    }

    public DataFilter WithFullOffersLadder()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_ALL_OFFERS");
        return this;
    }

    public DataFilter WithFullTradedLadder()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_TRADED");
        return this;
    }

    public DataFilter WithTradedVolume()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_TRADED_VOL");
        return this;
    }

    public DataFilter WithLastTradedPrice()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("EX_LTP");
        return this;
    }

    public DataFilter WithStartingPriceLadder()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("SP_TRADED");
        return this;
    }

    public DataFilter WithStartingPriceProjection()
    {
        Fields ??= new HashSet<string>();
        Fields.Add("SP_PROJECTED");
        return this;
    }

    public DataFilter WithLadderLevels(int levels)
    {
        LadderLevels = levels > 10 ? 10 : levels;
        return this;
    }
}