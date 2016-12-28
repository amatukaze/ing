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

        object r_View;
        public object View
        {
            get
            {
                if (r_View == null)
                    View = r_Tool.View.Value;

                return r_View;
            }
            private set
            {
                if (r_View != value)
                {
                    r_View = value;
                    OnPropertyChanged(nameof(View));
                }
            }
        }

        public ICommand OpenCommand { get; internal set; }

        internal ToolViewModel(IToolPane rpTool)
        {
            r_Tool = rpTool;
        }
    }
}
