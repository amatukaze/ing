namespace Sakuno.ING.Game;

public record ApiMessage(string Api, ReadOnlyMemory<byte> Request, ReadOnlyMemory<byte> Response);
