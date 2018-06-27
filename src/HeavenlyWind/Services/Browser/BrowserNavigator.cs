using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserNavigator : ModelBase
    {
        static Regex r_LoadCompletedParameterRegex { get; } = new Regex(@"(True|False);(True|False);(.*)");

        BrowserService r_Owner;

        string r_Url;
        public string Url
        {
            get { return r_Url; }
            set
            {
                if (r_Url != value)
                {
                    r_Url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        bool r_IsNavigating;
        public bool IsNavigating
        {
            get { return r_IsNavigating; }
            private set
            {
                if (r_IsNavigating != value)
                {
                    r_IsNavigating = value;
                    OnPropertyChanged(nameof(IsNavigating));
                }
            }
        }

        bool r_CanGoBack;
        public bool CanGoBack
        {
            get { return r_CanGoBack; }
            private set
            {
                if (r_CanGoBack != value)
                {
                    r_CanGoBack = value;
                    OnPropertyChanged(nameof(CanGoBack));
                }
            }
        }
        bool r_CanGoForward;
        public bool CanGoForward
        {
            get { return r_CanGoForward; }
            private set
            {
                if (r_CanGoForward != value)
                {
                    r_CanGoForward = value;
                    OnPropertyChanged(nameof(CanGoForward));
                }
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand RefreshCommand { get; }

        public ICommand ResizeBrowserToFitGameCommand { get; }
        public ICommand SetCookieCommand { get; }

        public BrowserNavigator(BrowserService rpOwner)
        {
            r_Owner = rpOwner;

            r_Owner.RegisterMessageHandler(CommunicatorMessages.LoadCompleted, parameter =>
            {
                var rMatch = r_LoadCompletedParameterRegex.Match(parameter);
                if (!rMatch.Success)
                    return;

                CanGoBack = bool.Parse(rMatch.Groups[1].Value);
                CanGoForward = bool.Parse(rMatch.Groups[2].Value);
                Url = rMatch.Groups[3].Value;
            });

            GoBackCommand = new DelegatedCommand(GoBack);
            GoForwardCommand = new DelegatedCommand(GoForward);
            NavigateCommand = new DelegatedCommand(Navigate);
            RefreshCommand = new DelegatedCommand(Refresh);

            SetCookieCommand = new DelegatedCommand(SetCookie);

            ResizeBrowserToFitGameCommand = new DelegatedCommand(ResizeBrowserToFitGame);
        }

        public void GoBack() => r_Owner.SendMessage(CommunicatorMessages.GoBack).Forget();
        public void GoForward() => r_Owner.SendMessage(CommunicatorMessages.GoForward).Forget();

        public void Navigate() => Navigate(Url);
        public void Navigate(string rpUrl)
        {
            if (rpUrl.IsNullOrEmpty() || !Uri.TryCreate(rpUrl, UriKind.Absolute, out var rUri))
                return;

            r_Owner.SendMessage(CommunicatorMessages.Navigate + ":" + rUri.ToString()).Forget();
        }

        public void Refresh()
        {
            if (r_Url.IsNullOrEmpty())
                return;

            r_Owner.SendMessage(CommunicatorMessages.Refresh).Forget();
        }

        public void ResizeBrowserToFitGame() => r_Owner.ResizeBrowserToFitGame().Forget();

        void SetCookie()
        {
            var rYear = DateTime.Now.AddYears(4).ToString("yyyy");
            var rScript = $"javascript:void(eval(\"document.cookie = 'ckcy=1;expires=Sun, 09 Feb {rYear} 09:00:09 GMT;domain=osapi.dmm.com;path=/'; document.cookie = 'ckcy=1;expires=Sun, 09 Feb  {rYear} 09:00:09 GMT;domain=203.104.209.7;path=/'; document.cookie = 'ckcy=1;expires=Sun, 09 Feb  {rYear} 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\")); location.href = \"{Url}\";";

            Uri rUri;
            if (!Uri.TryCreate(rScript, UriKind.Absolute, out rUri))
                return;

            r_Owner.SendMessage(CommunicatorMessages.Navigate + ":" + rUri.ToString()).Forget();

            var rDialog = new TaskDialog()
            {
                Caption = StringResources.Instance.Main.Product_Name,
                Instruction = StringResources.Instance.Main.Browser_Navigator_SetCookie_Instruction,
                Icon = TaskDialogIcon.Information,

                OwnerWindow = App.Current.MainWindow,
                ShowAtTheCenterOfOwner = true,
            };

            rDialog.ShowAndDispose();
        }
    }
}
