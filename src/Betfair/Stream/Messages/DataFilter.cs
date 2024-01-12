namespace Betfair.Stream.Messages;

/// <summary>
/// Used to restrict data received in a Betfair stream.
/// </summary>
public class DataFilter
{
    public int LadderLevels { get; private set; } = 3;

    // TODO make internal or readonly
    public HashSet<string>? Fields { get; private set; }

    /// <summary>
    /// Include market definitions and updates in the stream data.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithMarketDefinition()
    {
        // TODO rename With... to Include...
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

    /// <summary>
    /// Include the best available back and lay prices not including virtual prices. The number of ladder levels to receive can be set using .WithLadderLevels(), if undefined the default is 3 ladder levels.
    /// </summary>
    /// <returns>This DataFilter.</returns>
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
        LadderLevels = levels >= 11 ? 10 : levels;
        return this;
    }
}