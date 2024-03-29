namespace Betfair.Stream.Messages;

/// <summary>
/// Used to restrict data received in a Betfair market stream.
/// </summary>
public class DataFilter
{
    private HashSet<string>? _fields;

    [JsonPropertyName("ladderLevels")]
    public int LadderLevels { get; private set; } = 3;

    [JsonPropertyName("fields")]
    public IReadOnlySet<string>? Fields => _fields;

    /// <summary>
    /// Includes market definitions and updates in the stream data.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithMarketDefinition()
    {
        _fields ??=[];
        _fields.Add("EX_MARKET_DEF");
        return this;
    }

    /// <summary>
    /// Includes the best available back and lay prices including virtual prices. The number of ladder levels to receive can be set using .WithLadderLevels(),
    /// if undefined the default is 3 ladder levels.
    /// The virtual price stream is updated ~150 m/s after non-virtual prices.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithBestPricesIncludingVirtual()
    {
       _fields ??=[];
       _fields.Add("EX_BEST_OFFERS_DISP");
       return this;
    }

    /// <summary>
    /// Includes the best available back and lay prices not including virtual prices. The number of ladder levels to receive can be set using .WithLadderLevels(),
    /// if undefined the default is 3 ladder levels.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithBestPrices()
    {
        _fields ??=[];
        _fields.Add("EX_BEST_OFFERS");
        return this;
    }

    /// <summary>
    /// Includes the full available to BACK/LAY ladder.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithFullOffersLadder()
    {
        _fields ??=[];
        _fields.Add("EX_ALL_OFFERS");
        return this;
    }

    /// <summary>
    /// Include the full traded ladder.  This is the amount traded at any price on any selection in the market.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithFullTradedLadder()
    {
        _fields ??=[];
        _fields.Add("EX_TRADED");
        return this;
    }

    /// <summary>
    /// Includes market and runner level traded volume.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithTradedVolume()
    {
        _fields ??=[];
        _fields.Add("EX_TRADED_VOL");
        return this;
    }

    /// <summary>
    /// Includes the "Last Price Matched" on a selection.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithLastTradedPrice()
    {
        _fields ??=[];
        _fields.Add("EX_LTP");
        return this;
    }

    /// <summary>
    /// Include the starting price ladder.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithStartingPriceLadder()
    {
        _fields ??=[];
        _fields.Add("SP_TRADED");
        return this;
    }

    /// <summary>
    /// Includes starting price projection prices.
    /// </summary>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithStartingPriceProjection()
    {
        _fields ??=[];
        _fields.Add("SP_PROJECTED");
        return this;
    }

    /// <summary>
    /// For depth-based ladders the number of levels to send (1 to 10). 1 is best price to back or lay etc.
    /// </summary>
    /// <param name="levels">The number of ladder levels to return.</param>
    /// <returns>This DataFilter.</returns>
    public DataFilter WithLadderLevels(int levels)
    {
        LadderLevels = levels >= 11 ? 10 : levels;
        return this;
    }
}