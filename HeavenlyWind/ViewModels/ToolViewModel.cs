using Sakuno.KanColle.Amatsukaze.Extensibility;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public abstract class ToolViewModel : TabItemViewModel
    {
        IToolPane r_Tool;

        public override string Name
        {
            get { return r_Tool.Name; }
            protected set { }
        }

        public override bool IsClosable => true;

        public object View => r_Tool.View;

        public ICommand OpenCommand { get; internal set; }

        internal ToolViewModel(IToolPane rpTool)
        {
            r_Tool = rpTool;
        }
    }

    class ToolWithoutScrollBarViewModel : ToolViewModel
    {
        public ToolWithoutScrollBarViewModel(IToolPane rpTool) : base(rpTool) { }
    }
    class ToolWithScrollBarViewModel : ToolViewModel
    {
        public IToolPaneScrollBarVisibilities ScrollBarVisibilities { get; }

        internal ToolWithScrollBarViewModel(IToolPane rpTool, IToolPaneScrollBarVisibilities rpVisibilities) : base(rpTool)
        {
            ScrollBarVisibilities = rpVisibilities;
        }
    }
}
