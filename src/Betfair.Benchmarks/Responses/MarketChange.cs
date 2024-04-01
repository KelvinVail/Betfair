using System.Text;

namespace Betfair.Benchmarks.Responses;

public class MarketChange
{
    private byte[] ? _initialClk;

    public int Id { get; set; }

    public string? InitialClock =>
        _initialClk is null ? null : Encoding.UTF8.GetString(_initialClk);

    public void SetInitialClock(byte[] data) => _initialClk = data;
}
