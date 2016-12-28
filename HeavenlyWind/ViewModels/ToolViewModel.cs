using Sakuno.KanColle.Amatsukaze.Extensibility;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    class ToolViewModel : TabItemViewModel
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
}
