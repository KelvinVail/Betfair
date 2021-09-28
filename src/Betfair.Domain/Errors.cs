namespace Betfair.Domain
{
    public static class Errors
    {
        public static Error InvalidPrice(decimal price) =>
            new ("invalid.price", $"'{price}' is an invalid price.");
    }
}
