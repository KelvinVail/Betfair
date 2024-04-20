using Betfair.Api.Requests.OrderDtos;
using Betfair.Core;

namespace Betfair.Extensions.Orders;

/// <summary>
/// Fill or Kill bets are killed if they cannot be matched when placed.
/// Optionally passing a minFillSize value,
/// the Exchange will only match the order if at least the specified minFillSize can be
/// matched (if passed) or the whole order matched (if not).
/// Any order which cannot be so matched, and any remaining unmatched part of the order
/// (if minFillSize is specified) will be immediately cancelled.
/// Please note: the matching algorithm for Fill or Kill orders behaves slightly
/// differently to that for standard limit orders.
/// Whereas the price on a limit order represents the lowest price at which any fragment
/// should be matched, the price on a Fill or Kill order represents the lower limit of
/// the Volume Weighted Average Price (“VWAP”) for the entire volume matched.
/// So, for instance, a Fill or Kill order with price = 5.4 and size = 10 might be
/// matched as £2 @ 5.5, £6 @ 5.4 and £2 @ 5.3.
/// </summary>
/// <param name="selectionId">The selection id to place the bet on.</param>
/// <param name="side">Back or Lay.</param>
/// <param name="price">The price at which to place the bet.</param>
/// <param name="size">The size of the bet.</param>
/// <param name="minFillSize">The bet is killed unless at least the minFillSize can be matched.</param>
public class FillOrKillOrder(
    long selectionId,
    Side side,
    double price,
    double size,
    double? minFillSize = null)
    : LimitOrder(selectionId, side, price, size)
{
    public double? MinFillSize { get; } = minFillSize;

    internal override PlaceInstruction ToInstruction()
    {
        var instruction = base.ToInstruction();
        instruction.LimitOrder!.TimeInForce = "FILL_OR_KILL";
        instruction.LimitOrder!.MinFillSize = MinFillSize;

        return instruction;
    }
}