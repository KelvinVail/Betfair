using System.Buffers.Text;
using System.Text.Json;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Fast JSON reader optimized for Betfair stream data parsing.
/// </summary>
internal ref struct FastJsonReader
{
    private static readonly JsonTokenType[] _tokenMap = new JsonTokenType[256];
    private readonly ReadOnlySpan<byte> _buffer;
    private int _lastValueStart;
    private int _lastValueEnd;

    static FastJsonReader()
    {
        _tokenMap['{'] = JsonTokenType.StartObject;
        _tokenMap['}'] = JsonTokenType.EndObject;
        _tokenMap['['] = JsonTokenType.StartArray;
        _tokenMap[']'] = JsonTokenType.EndArray;
        _tokenMap['"'] = JsonTokenType.String;
    }

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

        // Skip whitespace
        while (Position < length && (_buffer[Position] == ' ' || _buffer[Position] == '\t' || _buffer[Position] == '\r' || _buffer[Position] == '\n'))
        {
            Position++;
        }

        if (Position >= length)
            return false;

        byte currentByte = _buffer[Position];

        // Skip commas and colons
        if (currentByte == ',' || currentByte == ':')
        {
            Position++;
            return Read(); // Recursively read the next token
        }

        TokenType = _tokenMap[currentByte];

        if (TokenType == JsonTokenType.String)
        {
            ReadToEndOfString(length);
            Position++; // Move past the closing quote
            return true;
        }
        else if (TokenType != JsonTokenType.None)
        {
            Position++;
            return true;
        }
        else
        {
            // This is a value (number, boolean, null)
            ReadToEndOfValue(length);
            return true;
        }
    }

    public int GetInt32()
    {
        Utf8Parser.TryParse(ValueSpan, out int value, out _);
        return value;
    }

    public long GetInt64()
    {
        Utf8Parser.TryParse(ValueSpan, out long value, out _);
        return value;
    }

    public bool GetBoolean() => ValueSpan[0] == 't';

    public double GetDouble()
    {
        Utf8Parser.TryParse(ValueSpan, out double value, out _);
        return value;
    }

    public string GetString()
    {
        return System.Text.Encoding.UTF8.GetString(ValueSpan);
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
        
        while (Position < length)
        {
            byte currentByte = _buffer[Position];
            if (currentByte == ',' || currentByte == '}' || currentByte == ']' || currentByte == ' ')
            {
                _lastValueEnd = Position;
                return;
            }
            Position++;
        }
        
        _lastValueEnd = Position;
    }
}
