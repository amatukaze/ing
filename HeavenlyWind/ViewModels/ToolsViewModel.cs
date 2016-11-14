using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels.Game;
using Sakuno.KanColle.Amatsukaze.ViewModels.Tools;
using Sakuno.KanColle.Amatsukaze.Views;
using Sakuno.KanColle.Amatsukaze.Views.Tools;
using Sakuno.UserInterface;
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

        SessionToolViewModel r_SessionTool = new SessionToolViewModel();

        public ICommand ShowSessionToolCommand { get; }

        ICommand r_OpenToolPaneCommand;
        public IList<ToolViewModel> ToolPanes { get; }

        public ToolsViewModel(GameInformationViewModel rpOwner)
        {
            r_Owner = rpOwner;

            ShowSessionToolCommand = new DelegatedCommand(() => WindowService.Instance.Show<SessionToolWindow>(r_SessionTool));

            r_OpenToolPaneCommand = new DelegatedCommand<ToolViewModel>(r_Owner.AddTabItem);
            ToolPanes = PluginService.Instance.ToolPanes?.Select(r => new ToolViewModel(r, r_OpenToolPaneCommand)).ToList().AsReadOnly();
        }
    }
}
