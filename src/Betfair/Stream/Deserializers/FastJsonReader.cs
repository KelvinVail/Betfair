using System.Buffers.Text;

namespace Betfair.Stream.Deserializers;

internal ref struct FastJsonReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private int _lastValueStart;
    private int _lastValueEnd;

    public FastJsonReader(ReadOnlySpan<byte> data)
    {
        _buffer = data;
        Position = 0;
        TokenType = JsonTokenType.None;
    }

    public int Position { get; private set; }

    public JsonTokenType TokenType { get; private set; }

    public ReadOnlySpan<byte> ValueSpan => _buffer[_lastValueStart.._lastValueEnd];

    public bool Read()
    {
        int length = _buffer.Length;
        TokenType = JsonTokenType.None;

        if (Position >= length)
            return false;

        byte currentByte = _buffer[Position];

        // Skip commas and colons with minimal branching
        if (currentByte == ',' || currentByte == ':')
        {
            Position++;
            return Read(); // Recursively read the next token
        }

        // Ultra-fast token type detection using direct byte comparisons
        switch (currentByte)
        {
            case (byte)'{':
                TokenType = JsonTokenType.StartObject;
                Position++;
                return true;
            case (byte)'}':
                TokenType = JsonTokenType.EndObject;
                Position++;
                return true;
            case (byte)'[':
                TokenType = JsonTokenType.StartArray;
                Position++;
                return true;
            case (byte)']':
                TokenType = JsonTokenType.EndArray;
                Position++;
                return true;
            case (byte)'"':
                TokenType = JsonTokenType.String;
                ReadToEndOfString(length);
                Position++; // Move past the closing quote
                return true;
            default:
                // This is a value (number, boolean, null)
                ReadToEndOfValue(length);

                // Ultra-fast token type determination
                byte firstByte = ValueSpan[0];
                TokenType = firstByte switch
                {
                    (byte)'t' => JsonTokenType.True,
                    (byte)'f' => JsonTokenType.False,
                    (byte)'n' => JsonTokenType.Null,
                    _ => JsonTokenType.Number
                };

                return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInt32()
    {
        if (TokenType == JsonTokenType.Number && ValueSpan.Length > 0)
        {
            // Ultra-fast integer parsing for common cases
            var span = ValueSpan;
            int result = 0;
            int sign = 1;
            int i = 0;

            // Handle negative numbers
            if (span[0] == (byte)'-')
            {
                sign = -1;
                i = 1;
            }

            // Parse digits directly for maximum speed
            for (; i < span.Length; i++)
            {
                byte b = span[i];
                if (b >= (byte)'0' && b <= (byte)'9')
                {
                    result = result * 10 + (b - (byte)'0');
                }
                else
                {
                    break; // Non-digit character
                }
            }

            return result * sign;
        }
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetInt64()
    {
        if (TokenType == JsonTokenType.Number && ValueSpan.Length > 0)
        {
            // Ultra-fast long parsing for timestamps and IDs
            var span = ValueSpan;
            long result = 0;
            long sign = 1;
            int i = 0;

            // Handle negative numbers
            if (span[0] == (byte)'-')
            {
                sign = -1;
                i = 1;
            }

            // Parse digits directly for maximum speed
            for (; i < span.Length; i++)
            {
                byte b = span[i];
                if (b >= (byte)'0' && b <= (byte)'9')
                {
                    result = result * 10 + (b - (byte)'0');
                }
                else
                {
                    break; // Non-digit character
                }
            }

            return result * sign;
        }
        return 0;
    }

    public bool GetBoolean() => TokenType == JsonTokenType.True;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetDouble()
    {
        if (TokenType == JsonTokenType.Number && ValueSpan.Length > 0)
        {
            // Ultra-fast double parsing optimized for Betfair data patterns
            var span = ValueSpan;

            // Check for simple integer case first (most common)
            bool hasDecimal = false;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == (byte)'.')
                {
                    hasDecimal = true;
                    break;
                }
            }

            if (!hasDecimal)
            {
                // Simple integer case - use fast int parsing
                return GetInt64();
            }

            // Fall back to standard parsing for decimals
            if (Utf8Parser.TryParse(ValueSpan, out double doubleValue, out _))
                return doubleValue;
        }

        return 0.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? GetNullableDouble()
    {
        if (TokenType == JsonTokenType.Null)
            return null;

        if (TokenType == JsonTokenType.Number && ValueSpan.Length > 0)
        {
            // Ultra-fast double parsing optimized for Betfair data patterns
            var span = ValueSpan;

            // Check for simple integer case first (most common)
            bool hasDecimal = false;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == (byte)'.')
                {
                    hasDecimal = true;
                    break;
                }
            }

            if (!hasDecimal)
            {
                // Simple integer case - use fast int parsing
                return GetInt64();
            }

            // Fall back to standard parsing for decimals
            if (Utf8Parser.TryParse(ValueSpan, out double doubleValue, out _))
                return doubleValue;
        }

        return null;
    }

    public string? GetString()
    {
        if (TokenType == JsonTokenType.String)
            return System.Text.Encoding.UTF8.GetString(ValueSpan);
        if (TokenType == JsonTokenType.Null)
            return null;
        return string.Empty;
    }

    public DateTime? GetNullableDateTime()
    {
        if (TokenType == JsonTokenType.String)
        {
            var trailingBytes = "0000"u8.ToArray();
            var newLength = (_lastValueEnd - _lastValueStart) + 3;
            byte[] result = new byte[newLength];
            var span = _buffer.Slice(_lastValueStart, _lastValueEnd - _lastValueStart - 1);
            span.CopyTo(result);
            trailingBytes.CopyTo(result.AsSpan(span.Length));

            if (Utf8Parser.TryParse(result, out DateTimeOffset value, out _, 'O'))
                return value.DateTime;
        }

        return null;
    }

    public void SkipValue()
    {
        if (TokenType == JsonTokenType.StartObject)
        {
            int depth = 1;
            while (depth > 0 && Read())
            {
                if (TokenType == JsonTokenType.StartObject)
                    depth++;
                else if (TokenType == JsonTokenType.EndObject)
                    depth--;
            }
        }
        else if (TokenType == JsonTokenType.StartArray)
        {
            int depth = 1;
            while (depth > 0 && Read())
            {
                if (TokenType == JsonTokenType.StartArray)
                    depth++;
                else if (TokenType == JsonTokenType.EndArray)
                    depth--;
            }
        }
    }

    private void ReadToEndOfString(int length)
    {
        _lastValueStart = Position + 1;
        Position++;

        while (Position < length)
        {
            if (_buffer[Position] == '"')
            {
                _lastValueEnd = Position;
                return;
            }

            Position++;
        }

        _lastValueEnd = Position;
    }

    private void ReadToEndOfValue(int length)
    {
        _lastValueStart = Position;

        // Ultra-fast value scanning with minimal branching
        while (Position < length)
        {
            byte currentByte = _buffer[Position];

            // Check for value terminators in order of likelihood
            if (currentByte == ',' || currentByte == '}' || currentByte == ']' ||
                currentByte == ' ' || currentByte == '\t' || currentByte == '\r' || currentByte == '\n')
            {
                _lastValueEnd = Position;
                return;
            }

            Position++;
        }

        _lastValueEnd = Position;
    }
}
