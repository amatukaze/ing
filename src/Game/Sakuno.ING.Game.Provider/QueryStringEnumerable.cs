using System.Diagnostics;
using System.Text;

namespace Sakuno.ING.Game.Provider;

internal readonly ref struct QueryStringEnumerable(ReadOnlySpan<byte> queryString)
{
    private readonly ReadOnlySpan<byte> _queryString = queryString;

    public Enumerator GetEnumerator() => new(_queryString);

    public readonly ref struct NameValuePair(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        public ReadOnlySpan<byte> Name { get; } = name;
        public ReadOnlySpan<byte> Value { get; } = value;

        public int DecodeValueAsInt() => int.Parse(Value);
        public TId DecodeValueAsIdentifier<TId>() where TId : struct, IIdentifier<TId, int> =>
            TId.From(DecodeValueAsInt());

        public string DecodeValueAsString() => Decode(Value).ToString();
        private static ReadOnlyMemory<char> Decode(ReadOnlySpan<byte> bytes)
        {
            if (!bytes.ContainsAny((byte)'%', (byte)'+'))
                return Encoding.ASCII.GetString(bytes).AsMemory();

            var buffer = new char[bytes.Length];
            Encoding.ASCII.GetChars(bytes, buffer);

            buffer.AsSpan().Replace('+', ' ');

            var success = Uri.TryUnescapeDataString(buffer, buffer, out var unescapedLength);
            Debug.Assert(success);

            return buffer.AsMemory(0, unescapedLength);
        }
    }

    public ref struct Enumerator
    {
        private ReadOnlySpan<byte> _remaining;

        public NameValuePair Current { get; private set; }

        internal Enumerator(ReadOnlySpan<byte> queryString)
        {
            _remaining = queryString.IsEmpty || queryString[0] is not (byte)'?' ? queryString : queryString[1..];
        }

        public bool MoveNext()
        {
            while (!_remaining.IsEmpty)
            {
                ReadOnlySpan<byte> segment;

                var delimiterIndex = _remaining.IndexOf((byte)'&');
                if (delimiterIndex >= 0)
                {
                    segment = _remaining[..delimiterIndex];
                    _remaining = _remaining[(delimiterIndex + 1)..];
                }
                else
                {
                    segment = _remaining;
                    _remaining = default;
                }

                var equalIndex = segment.IndexOf((byte)'=');
                if (equalIndex >= 0)
                {
                    Current = new NameValuePair(segment[..equalIndex], segment[(equalIndex + 1)..]);
                    return true;
                }

                if (segment.IsEmpty)
                    continue;

                Current = new NameValuePair(segment, default);
                return true;
            }

            Current = default;
            return false;
        }
    }
}
