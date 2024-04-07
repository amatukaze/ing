using System.Buffers;

namespace Sakuno.ING.Game;

public record ApiMessage(string Api, ReadOnlySequence<byte> Request, ReadOnlySequence<byte> Response);
