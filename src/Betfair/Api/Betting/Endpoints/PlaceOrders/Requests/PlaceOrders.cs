namespace Betfair.Api.Betting.Endpoints.PlaceOrders.Requests;

/// <summary>
/// Represents a request to place orders on a market.
/// </summary>
public class PlaceOrders
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceOrders"/> class.
    /// </summary>
    /// <param name="marketId">The market id these orders are to be placed on.</param>
    public PlaceOrders(string marketId) =>
        MarketId = marketId ?? throw new ArgumentNullException(nameof(marketId));

    /// <summary>
    /// Gets the market ID.
    /// The market id these orders are to be placed on.
    /// </summary>
    public string MarketId { get; }

    /// <summary>
    /// Gets the list of instructions for placing orders.
    /// The number of place instructions.
    /// The limit of place instructions per request is 200 for the Global Exchange and 50 for the Italian Exchange.
    /// </summary>
    public List<PlaceInstruction> Instructions { get; } = [];

    /// <summary>
    /// Gets or sets the customer reference.
    /// Optional parameter allowing the client to pass a unique string (up to 32 chars) that is used to de-dupe mistaken re-submissions.
    /// CustomerRef can contain: upper/lower chars, digits, chars : - . _ + * : ; ~ only.
    /// Please note: There is a time window associated with the de-duplication of duplicate submissions which is 60 seconds.
    /// NB: This field does not persist into the placeOrders response/Order Stream API and should not be confused with customerOrderRef, which is separate field that can be sent in the PlaceInstruction.
    /// </summary>
    public string? CustomerRef { get; set; }

    /// <summary>
    /// Gets or sets the version of the market.
    /// Optional parameter allowing the client to specify which version of the market the orders should be placed on.
    /// If the current market version is higher than that sent on an order, the bet will be lapsed.
    /// </summary>
    public long? MarketVersion { get; set; }

    /// <summary>
    /// Gets or sets the customer strategy reference.
    /// An optional reference customers can use to specify which strategy has sent the order.
    /// The reference will be returned on order change messages through the stream API.
    /// The string is limited to 15 characters.
    /// If an empty string is provided it will be treated as null.
    /// </summary>
    public string? CustomerStrategyRef { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request is asynchronous.
    /// An optional flag (not setting equates to false) which specifies if the orders should be placed asynchronously.
    /// Orders can be tracked via the Exchange Stream API or the API-NG by providing a customerOrderRef for each place order.
    /// An order's status will be PENDING and no bet ID will be returned.
    /// This functionality is available for all bet types - including Market on Close and Limit on Close.
    /// </summary>
    public bool? Async { get; set; }
}

