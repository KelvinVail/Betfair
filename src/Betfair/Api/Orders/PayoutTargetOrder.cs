using Betfair.Api.Requests.OrderDtos;
using Betfair.Core;

namespace Betfair.Api.Orders;

/// <summary>
/// Payout target bets automatically calculate the size of bet needed to
/// achieve the specified payout target.
/// </summary>
/// <param name="selectionId">The selection id to place the bet on.</param>
/// <param name="side">Back or Lay.</param>
/// <param name="payoutTarget">The total payout requested on a LimitOrder.</param>
/// <param name="persistenceType">What to do with the bet at turn-in-play.</param>
public class PayoutTargetOrder(
    long selectionId,
    Side side,
    double payoutTarget,
    PersistenceType? persistenceType = null)
    : OrderBase(selectionId, side)
{
    public double PayoutTarget { get; } = payoutTarget;

    public string PersistenceType { get; } = persistenceType?.Value ?? Core.PersistenceType.Lapse.Value;

    internal override PlaceInstruction ToInstruction() => throw new NotImplementedException();
}