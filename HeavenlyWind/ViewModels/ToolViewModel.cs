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

        public override bool IsClosable => true;

        public object View => r_Tool.View;
        public IToolPaneScrollBarVisibilities ScrollBarVisibilities { get; }

        public ICommand OpenCommand { get; }

        internal ToolViewModel(IToolPane rpTool, ICommand rpOpenCommand)
        {
            r_Tool = rpTool;
            ScrollBarVisibilities = rpTool as IToolPaneScrollBarVisibilities;

            OpenCommand = rpOpenCommand;
        }
    }
}
