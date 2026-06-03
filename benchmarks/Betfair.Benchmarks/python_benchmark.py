"""
Benchmark: betfairlightweight's own StreamListener processing stream messages.
This uses the library's MarketBookCache directly — the real implementation
with orjson as the JSON backend.

Usage:
    pip install betfairlightweight orjson
    py python_benchmark.py

Compare with .NET:
    dotnet run -c Release -- quick
"""

import os
import time
import statistics
import gc

import orjson
from betfairlightweight.streaming.listener import StreamListener


DATA_FILE = os.path.join(
    os.path.dirname(__file__),
    "Betfair.Benchmarks",
    "Data",
    "MarketStream.txt",
)

WARMUP_ITERATIONS = 5
BENCHMARK_ITERATIONS = 20


def load_lines():
    """Load all lines from the MarketStream.txt file as strings."""
    with open(DATA_FILE, "r", encoding="utf-8") as f:
        return [line.strip() for line in f.readlines() if line.strip()]


def benchmark_betfairlightweight(lines):
    """Full pipeline using betfairlightweight's own StreamListener + MarketBookCache."""
    listener = StreamListener()
    listener.register_stream(0, "marketSubscription")
    for line in lines:
        listener.on_data(line)
    return listener


def run_benchmark(name, func, data, warmup=WARMUP_ITERATIONS, iterations=BENCHMARK_ITERATIONS):
    """Run a benchmark function and collect timing statistics."""
    for _ in range(warmup):
        func(data)

    gc.collect()
    gc.disable()
    times = []
    for _ in range(iterations):
        start = time.perf_counter_ns()
        func(data)
        end = time.perf_counter_ns()
        times.append((end - start) / 1_000_000)
    gc.enable()

    mean = statistics.mean(times)
    stdev = statistics.stdev(times) if len(times) > 1 else 0
    median = statistics.median(times)

    return {
        "name": name,
        "mean_ms": mean,
        "stdev_ms": stdev,
        "median_ms": median,
        "min_ms": min(times),
        "max_ms": max(times),
    }


def main():
    if not os.path.exists(DATA_FILE):
        print(f"ERROR: Data file not found: {DATA_FILE}")
        return

    lines = load_lines()
    print(f"Loaded {len(lines)} lines from MarketStream.txt")
    print()

    # Run once to show what the cache contains after processing
    listener = benchmark_betfairlightweight(lines)
    stream = listener.stream
    for market_id, cache in stream._caches.items():
        print(f"Market: {market_id}")
        print(f"  Runners: {len(cache.runners)}")
        print(f"  Total matched: {cache.total_matched}")
        for runner in cache.runners[:3]:
            print(f"  Runner {runner.selection_id}: LTP={runner.last_price_traded} "
                  f"TV={runner.total_matched} "
                  f"ATB levels={len(runner.available_to_back.order_book)} "
                  f"ATL levels={len(runner.available_to_lay.order_book)}")
        if len(cache.runners) > 3:
            print(f"  ... and {len(cache.runners) - 3} more runners")
    print()

    # Benchmark
    print("Running: betfairlightweight StreamListener (orjson + MarketBookCache)...")
    result = run_benchmark(
        "betfairlightweight (orjson + cache)",
        benchmark_betfairlightweight,
        lines,
    )

    print()
    print("=" * 80)
    print("Result: betfairlightweight StreamListener (orjson + MarketBookCache)")
    print("=" * 80)
    print()
    print(f"| {'Method':<40} | {'Mean':>10} | {'Median':>10} | {'Min':>10} | {'StdDev':>10} |")
    print(f"|{'-'*42}|{'-'*12}|{'-'*12}|{'-'*12}|{'-'*12}|")
    print(
        f"| {result['name']:<40} | {result['mean_ms']:>8.3f}ms | "
        f"{result['median_ms']:>8.3f}ms | {result['min_ms']:>8.3f}ms | "
        f"{result['stdev_ms']:>8.3f}ms |"
    )
    print()
    print("This is the target to beat in .NET.")
    print()


if __name__ == "__main__":
    main()
