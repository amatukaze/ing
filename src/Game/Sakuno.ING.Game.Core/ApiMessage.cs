using System.Buffers;

namespace Sakuno.ING.Game;

public record ApiMessage(string Api, ReadOnlyMemory<byte> Request, ReadOnlySequence<byte> Response);
