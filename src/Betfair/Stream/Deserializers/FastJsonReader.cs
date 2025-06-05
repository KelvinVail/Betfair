using System.Buffers.Text;
using System.Text;

namespace Betfair.Stream.Deserializers;

public ref struct FastJsonReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private readonly int _length;
    private int _lastValueStart;
    private int _lastValueEnd;

    public FastJsonReader(ReadOnlySpan<byte> data)
    {
        _buffer = data;
        _length = _buffer.Length;
        Position = 0;
        TokenType = JsonTokenType.None;
    }

    public int Position { get; private set; }

    public JsonTokenType TokenType { get; private set; }

    public ReadOnlySpan<byte> ValueSpan => _buffer[_lastValueStart.._lastValueEnd];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Read()
    {
        TokenType = JsonTokenType.None;

        while (Position < _length)
        {
            byte currentByte = _buffer[Position];

            switch (currentByte)
            {
                case (byte)':':
                case (byte)',':
                    Position++;
                    continue;
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
                    ReadToEndOfString();
                    Position++; // Move past the closing quote
                    return true;
                default:
                    ReadToEndOfValue();
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
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInt32()
    {
        if (TokenType == JsonTokenType.Number && ValueSpan.Length > 0)
        {
            var span = ValueSpan;
            int result = 0;
            int sign = 1;
            int i = 0;
            if (span[0] == (byte)'-')
            {
                sign = -1;
                i = 1;
            }
            for (; i < span.Length; i++)
            {
                byte b = span[i];
                if ((uint)(b - (byte)'0') <= 9)
                    result = result * 10 + (b - (byte)'0');
                else
                    break;
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
            var span = ValueSpan;
            long result = 0;
            long sign = 1;
            int i = 0;
            if (span[0] == (byte)'-')
            {
                sign = -1;
                i = 1;
            }
            for (; i < span.Length; i++)
            {
                byte b = span[i];
                if ((uint)(b - (byte)'0') <= 9)
                    result = result * 10 + (b - (byte)'0');
                else
                    break;
            }
            return result * sign;
        }
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBoolean() => TokenType == JsonTokenType.True;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetDouble()
    {
        if (TokenType == JsonTokenType.Number && ValueSpan.Length > 0)
        {
            var span = ValueSpan;
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
                return GetInt64();
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
            var span = ValueSpan;
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
                return GetInt64();
            if (Utf8Parser.TryParse(ValueSpan, out double doubleValue, out _))
                return doubleValue;
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetString()
    {
        if (TokenType == JsonTokenType.String)
            return Encoding.UTF8.GetString(ValueSpan);
        if (TokenType == JsonTokenType.Null)
            return null;
        return string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadToEndOfString()
    {
        _lastValueStart = Position + 1;
        Position++;
        while (Position < _length)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadToEndOfValue()
    {
        _lastValueStart = Position;
        while (Position < _length)
        {
            byte currentByte = _buffer[Position];
            if (currentByte is (byte)',' or (byte)'}' or (byte)']' or (byte)' ' or (byte)'\t' or (byte)'\r' or (byte)'\n')
            {
                _lastValueEnd = Position;
                return;
            }

            Position++;
        }

        _lastValueEnd = Position;
    }
}
