using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Shell
{
    public interface IShell
    {
        void Run();
        void RegisterView(ViewDescriptor descriptor);
    }
}
