namespace Betfair.Domain
{
    public static class Errors
    {
        public static DomainError InvalidPrice(decimal price) =>
            new ("invalid.price", $"'{price}' is an invalid price.");
    }
}
