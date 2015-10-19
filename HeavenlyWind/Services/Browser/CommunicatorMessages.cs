namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    public static class CommunicatorMessages
    {
        public const string Ready = nameof(Ready);
        public const string SetPort = nameof(SetPort);
        public const string Attach = nameof(Attach);

        public const string GoBack = nameof(GoBack);
        public const string GoForward = nameof(GoForward);
        public const string Navigate = nameof(Navigate);
        public const string Refresh = nameof(Refresh);

        public const string LoadCompleted = nameof(LoadCompleted);

        public const string SetZoom = nameof(SetZoom);
        public const string Resize = nameof(Resize);
        public const string InvalidateArrange = nameof(InvalidateArrange);

        public const string TryExtractFlash = nameof(TryExtractFlash);
        public const string ExtractionResult = nameof(ExtractionResult);

    }
}
