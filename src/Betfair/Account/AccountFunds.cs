namespace Betfair.Account;

public class AccountFunds
{
    public double AvailableToBetBalance { get; set; }

    public double Exposure { get; set; }

    public double RetainedCommission { get; set; }

    public double ExposureLimit { get; set; }

    public double DiscountRate { get; set; }

    public double PointsBalance { get; set; }

    public string Wallet { get; set; } = string.Empty;
}