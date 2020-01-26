namespace Betfair.Account
{
    using System;

    public class StatementItem
    {
        public string RefId { get; internal set; }

        public DateTime ItemDate { get; internal set; }

        public double Amount { get; internal set; }

        public double Balance { get; internal set; }
    }
}
