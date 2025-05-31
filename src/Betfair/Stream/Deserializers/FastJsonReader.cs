using System.Buffers.Text;
using System.Text.Json;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-fast JSON reader optimized for Betfair stream data parsing.
/// Uses direct byte comparisons and minimal branching for maximum performance.
/// </summary>
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

        // Ultra-fast whitespace skipping using direct byte comparisons
        while (Position < length)
        {
            byte b = _buffer[Position];
            if (b != ' ' && b != '\t' && b != '\r' && b != '\n')
                break;
            Position++;
        }

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
                if (ValueSpan.Length > 0)
                {
                    byte firstByte = ValueSpan[0];
                    TokenType = firstByte switch
                    {
                        (byte)'t' => JsonTokenType.True,
                        (byte)'f' => JsonTokenType.False,
                        (byte)'n' => JsonTokenType.Null,
                        _ => JsonTokenType.Number
                    };
                }
                else
                {
                    TokenType = JsonTokenType.Number;
                }
                return true;
        }
    }

    public int GetInt32()
    {
        if (TokenType == JsonTokenType.Number)
        {
            Utf8Parser.TryParse(ValueSpan, out int value, out _);
            return value;
        }
        return 0;
    }

    public long GetInt64()
    {
        if (TokenType == JsonTokenType.Number)
        {
            Utf8Parser.TryParse(ValueSpan, out long value, out _);
            return value;
        }
        return 0;
    }

    public bool GetBoolean() => TokenType == JsonTokenType.True;

    public double GetDouble()
    {
        if (TokenType == JsonTokenType.Number)
        {
            // Try parsing as double first
            if (Utf8Parser.TryParse(ValueSpan, out double doubleValue, out _))
                return doubleValue;

            // If that fails, try parsing as int and convert to double
            if (Utf8Parser.TryParse(ValueSpan, out int intValue, out _))
                return intValue;
        }

        return 0.0;
    }

    public double? GetNullableDouble()
    {
        if (TokenType == JsonTokenType.Number)
        {
            // Try parsing as double first
            if (Utf8Parser.TryParse(ValueSpan, out double doubleValue, out _))
                return doubleValue;

            // If that fails, try parsing as int and convert to double
            if (Utf8Parser.TryParse(ValueSpan, out int intValue, out _))
                return intValue;
        }

        return null;
    }

    public string GetString()
    {
        if (TokenType == JsonTokenType.String)
            return System.Text.Encoding.UTF8.GetString(ValueSpan);
        if (TokenType == JsonTokenType.Null)
            return null;
        return string.Empty;
    }

    public DateTime GetDateTime()
    {
        if (TokenType == JsonTokenType.String)
        {
            var str = System.Text.Encoding.UTF8.GetString(ValueSpan);
            if (DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime value))
                return value;
        }
        return default;
    }

    public DateTime? GetNullableDateTime()
    {
        if (TokenType == JsonTokenType.String)
        {
            var str = System.Text.Encoding.UTF8.GetString(ValueSpan);
            if (DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime value))
                return value;
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
