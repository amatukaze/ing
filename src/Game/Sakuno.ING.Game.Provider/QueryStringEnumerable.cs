using System.Buffers;
using System.Text;

namespace Sakuno.ING.Game.Provider;

internal readonly struct QueryStringEnumerable
{
    private readonly ReadOnlyMemory<char> _queryString;

    public QueryStringEnumerable(ReadOnlyMemory<char> queryString)
    {
        _queryString = queryString;
    }
    public QueryStringEnumerable(ReadOnlySequence<byte> queryString)
    {
        _queryString = Encoding.ASCII.GetString(queryString).AsMemory();
    }

    public Enumerator GetEnumerator() => new(_queryString);

    public record struct NameValuePair(ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Value);

    public struct Enumerator
    {
        private ReadOnlyMemory<char> _remaining;

        public NameValuePair Current { get; private set; }

        internal Enumerator(ReadOnlyMemory<char> queryString)
        {
            _remaining = queryString.IsEmpty || queryString.Span[0] is not '?' ? queryString : queryString[1..];
        }

        public bool MoveNext()
        {
            while (!_remaining.IsEmpty)
            {
                ReadOnlyMemory<char> segment;

                var delimiterIndex = _remaining.Span.IndexOf('&');
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

                var equalIndex = segment.Span.IndexOf('=');
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
