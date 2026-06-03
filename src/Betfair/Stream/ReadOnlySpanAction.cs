namespace Betfair.Stream;

/// <summary>
/// Encapsulates a method that receives a read-only span of objects of type T.
/// Used to pass span-based callbacks without allocating closures.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
/// <param name="span">The read-only span of elements.</param>
internal delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);
