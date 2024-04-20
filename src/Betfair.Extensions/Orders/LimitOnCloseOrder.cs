using Betfair.Api.Requests.Orders;
using Betfair.Core;

namespace Betfair.Extensions.Orders;

/// <summary>
/// Bets are matched if, and only if, the returned starting price is better
/// than a specified price.
/// In the case of back bets, LOC bets are matched if the calculated starting
/// price is greater than the specified price.
/// In the case of lay bets, LOC bets are matched if the starting price is
/// less than the specified price.
/// If the specified limit is equal to the starting price, then it may be
/// matched, partially matched, or may not be matched at all, depending on
/// how much is needed to balance all bets against each other
/// (MOC, LOC and normal exchange bets).
/// </summary>
/// <param name="selectionId">The selection id to place the bet on.</param>
/// <param name="side">Back or Lay.</param>
/// <param name="liability">The size of the bet.</param>
public class LimitOnCloseOrder(
    long selectionId,
    Side side,
    double liability)
    : OrderBase(selectionId, side)
{
    public double Liability { get; } = liability;

    internal override PlaceInstruction ToInstruction() => throw new NotImplementedException();
}