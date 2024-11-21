using System.Buffers.Text;

namespace Betfair.Extensions.ByteReaders;

internal ref struct BetfairJsonReader
{
    private static readonly JsonTokenType[] _tokenMap = new JsonTokenType[256];
    private readonly ReadOnlySpan<byte> _buffer;
    private int _lastValueStart;
    private int _lastValueEnd;

    static BetfairJsonReader()
    {
        _tokenMap['{'] = JsonTokenType.StartObject;
        _tokenMap['}'] = JsonTokenType.EndObject;
        _tokenMap['['] = JsonTokenType.StartArray;
        _tokenMap[']'] = JsonTokenType.EndArray;
        _tokenMap['\"'] = JsonTokenType.String;
        _tokenMap[':'] = JsonTokenType.PropertyName;
    }

    public BetfairJsonReader(ReadOnlySpan<byte> data)
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
        while (Position < length)
        {
            byte currentByte = _buffer[Position];
            // var c = (char)currentByte;
            
            if (currentByte == ' ')
            {
                Position++;
                continue;
            }
            
            if (currentByte == ',')
            {
                Position++;
                return true;
            }
            
            TokenType = _tokenMap[currentByte];
            if (TokenType == JsonTokenType.None)
            {
                ReadToEndOfValue(length);
                return true;
            }
            
            if (TokenType == JsonTokenType.String)
            {
                ReadToEndOfString(length);
            }
            
            Position++;
            return true;
        }

        return false;
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

    public DateTimeOffset GetDateTimeOffset()
    {
        var trailingBytes = "0000"u8.ToArray();
        var newLength = (_lastValueEnd - _lastValueStart) + 3;
        byte[] result = new byte[newLength];
        var span = _buffer.Slice(_lastValueStart, _lastValueEnd - _lastValueStart - 1);
        span.CopyTo(result);
        trailingBytes.CopyTo(result.AsSpan(span.Length));

        Utf8Parser.TryParse(result, out DateTimeOffset value, out _, 'O');
        return value;
    }

    public bool GetBoolean() => ValueSpan[0] == 't';

    public double GetDouble()
    {
        Utf8Parser.TryParse(ValueSpan, out double value, out _);
        return value;
    }

    private void ReadToEndOfString(int length)
    {
        _lastValueStart = Position + 1;
        while (++Position < length)
        {
            if (_buffer[Position] != '\"') continue;

            _lastValueEnd = Position;
            return;
        }
    }

    private void ReadToEndOfValue(int length)
    {
        _lastValueStart = Position;
        while (++Position < length)
        {
            var b = _buffer[Position];
            if (b != ',' && _tokenMap[b] == JsonTokenType.None) continue;

            _lastValueEnd = Position;
            return;
        }
    }
}
