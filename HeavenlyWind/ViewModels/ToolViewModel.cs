using Sakuno.KanColle.Amatsukaze.Extensibility;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public class ToolViewModel : TabItemViewModel
    {
        IToolPane r_Tool;

        public override string Name
        {
            get { return r_Tool.Name; }
            protected set { }
        }

        public object View => r_Tool.View;

        public ICommand OpenCommand { get; }

        internal ToolViewModel(IToolPane rpTool, ICommand rpOpenCommand)
        {
            r_Tool = rpTool;

            OpenCommand = rpOpenCommand;
        }
    }
}
