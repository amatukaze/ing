namespace Sakuno.ING.Shell
{
    public class CategorizedSettingViews
    {
        internal CategorizedSettingViews(string title, object[] content)
        {
            Title = title;
            Content = content;
        }

        public string Title { get; }
        public object[] Content { get; }
    }
}
