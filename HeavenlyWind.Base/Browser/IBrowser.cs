using System;

namespace Sakuno.KanColle.Amatsukaze.Browser
{
    public interface IBrowser
    {
        event Action<bool, bool, string> LoadCompleted;

        void GoBack();
        void GoForward();

        void Navigate(string rpUrl);

        void Refresh();

    }
}
