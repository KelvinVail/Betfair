using Betfair.Api.Requests.OrderDtos;
using Betfair.Core;

namespace Betfair.Api.Orders;

/// <summary>
/// Market On Close bets remain unmatched until the market is reconciled.
/// They are matched and settled at a price that is representative of the market
/// at the point the market is turned in-play.
/// The market is reconciled to find a starting price and MOC bets are settled
/// at whatever starting price is returned.
/// MOC bets are always matched and settled, unless a starting price is not
/// available for the selection.
/// Market on Close bets can only be placed before the starting price is determined.
/// </summary>
/// <param name="selectionId">The selection id to place the bet on.</param>
/// <param name="side">Back or Lay.</param>
/// <param name="liability">The size of the bet.</param>
public class MarketOnCloseOrder(
    long selectionId,
    Side side,
    double liability)
    : OrderBase(selectionId, side)
{
    public double Liability { get; } = liability;

    internal override PlaceInstruction ToInstruction() => throw new NotImplementedException();
}