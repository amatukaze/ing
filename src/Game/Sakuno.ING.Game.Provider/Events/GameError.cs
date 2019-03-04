namespace Sakuno.ING.Game.Events
{
    public readonly struct GameError
    {
        public GameError(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public int ErrorCode { get; }
        public string Message { get; }
    }
}
