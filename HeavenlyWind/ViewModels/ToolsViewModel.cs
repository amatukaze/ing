using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    [ViewInfo(typeof(ToolCenter))]
    public class ToolsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Tools; }
            protected set { }
        }

        GameInformationViewModel r_Owner;

        ICommand r_OpenToolPaneCommand;
        public IList<ToolViewModel> ToolPanes { get; }

        public ToolsViewModel(GameInformationViewModel rpOwner)
        {
            r_Owner = rpOwner;

            r_OpenToolPaneCommand = new DelegatedCommand<ToolViewModel>(r_Owner.AddTabItem);
            ToolPanes = PluginService.Instance.ToolPanes?.Select(r =>
            {
                ToolViewModel rResult;

                var rScrollBarVisibilities = r as IToolPaneScrollBarVisibilities;
                if (rScrollBarVisibilities == null)
                    rResult = new ToolWithoutScrollBarViewModel(r) { OpenCommand = r_OpenToolPaneCommand };
                else
                    rResult = new ToolWithScrollBarViewModel(r, rScrollBarVisibilities) { OpenCommand = r_OpenToolPaneCommand };

                return rResult;
            }).ToArray();
        }
    }
}
