using Sakuno.SystemInterop;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Windows.Interop;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class PanicKeyService : ModelBase
    {
        public static PanicKeyService Instance { get; } = new PanicKeyService();

        IntPtr r_Handle;

        ushort r_HotKeyID;

        public bool IsHotKeyRegistered { get; private set; }

        bool r_IsHotKeyRegisteredByOther;
        public bool IsHotKeyRegisteredByOther
        {
            get { return r_IsHotKeyRegisteredByOther; }
            private set
            {
                if (r_IsHotKeyRegisteredByOther != value)
                {
                    r_IsHotKeyRegisteredByOther = value;
                    OnPropertyChanged(nameof(IsHotKeyRegisteredByOther));
                }
            }
        }

        public bool IsPanicKeyPressed { get; private set; }

        PanicKeyService() { }

        internal void Initialize(IntPtr rpHandle)
        {
            r_Handle = rpHandle;

            r_HotKeyID = NativeMethods.Kernel32.GlobalAddAtomW("Test");

            var rPCEL = PropertyChangedEventListener.FromSource(Preference.Current.Other.PanicKey);
            rPCEL.Add(nameof(Preference.Current.Other.PanicKey.Enabled), (s, e) => UpdateHotKey());
            rPCEL.Add(nameof(Preference.Current.Other.PanicKey.Key), (s, e) => UpdateHotKey());

            UpdateHotKey();

            HwndSource.FromHwnd(rpHandle).AddHook(WndProc);
        }

        void UpdateHotKey()
        {
            IsHotKeyRegisteredByOther = false;

            var rPreference = Preference.Current.Other.PanicKey;
            if (!rPreference.Enabled)
            {
                if (IsHotKeyRegistered)
                    NativeMethods.User32.UnregisterHotKey(r_Handle, r_HotKeyID);

                Debug.WriteLineIf(IsHotKeyRegistered, "PanicKeyService: Unregistered");

                return;
            }

            var rModifierKeys = GetModifierKeys();

            if (IsHotKeyRegistered)
                NativeMethods.User32.UnregisterHotKey(r_Handle, r_HotKeyID);

            IsHotKeyRegistered = NativeMethods.User32.RegisterHotKey(r_Handle, r_HotKeyID, rModifierKeys, rPreference.Key);
            if (!IsHotKeyRegistered)
            {
                NativeMethods.User32.UnregisterHotKey(r_Handle, r_HotKeyID);
                IsHotKeyRegistered = NativeMethods.User32.RegisterHotKey(r_Handle, r_HotKeyID, rModifierKeys, rPreference.Key);
                IsHotKeyRegisteredByOther = !IsHotKeyRegistered;
            }

            Debug.Write("PanicKeyService: ");
            Debug.WriteIf(IsHotKeyRegistered, "Registered - ");
            Debug.WriteIf(IsHotKeyRegisteredByOther, "Registered by other - ");
            Debug.WriteLine(GetModifierKeysString(Preference.Current.Other.PanicKey.ModifierKeys) + KeyInterop.KeyFromVirtualKey(rPreference.Key));
        }
        NativeEnums.ModifierKeys GetModifierKeys()
        {
            var rResult = (NativeEnums.ModifierKeys)Preference.Current.Other.PanicKey.ModifierKeys;
            if (OS.IsWin7OrLater)
                rResult |= NativeEnums.ModifierKeys.MOD_NOREPEAT;

            return rResult;
        }

        public static string GetModifierKeysString(int rpModifierKeys)
        {
            var rBuilder = new StringBuilder(8);

            GetModifierKeysString(rpModifierKeys, rBuilder);

            return rBuilder.ToString();
        }
        public static void GetModifierKeysString(int rpModifierKeys, StringBuilder rpBuilder)
        {
            var rBuilder = new StringBuilder(8);

            if ((rpModifierKeys & (int)ModifierKeys.Windows) != 0)
                rpBuilder.Append("Win + ");
            if ((rpModifierKeys & (int)ModifierKeys.Control) != 0)
                rpBuilder.Append("Ctrl + ");
            if ((rpModifierKeys & (int)ModifierKeys.Shift) != 0)
                rpBuilder.Append("Shift + ");
            if ((rpModifierKeys & (int)ModifierKeys.Alt) != 0)
                rpBuilder.Append("Alt + ");
        }

        public static string GetKeyString(Key rpKey)
        {
            var rBuilder = new StringBuilder(8);

            GetKeyString(rpKey, rBuilder);

            return rBuilder.ToString();
        }
        public static void GetKeyString(Key rpKey, StringBuilder rpBuilder)
        {
            switch (rpKey)
            {
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    rpBuilder.Append(rpKey - Key.D0);
                    break;

                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    rpBuilder.Append("Num ").Append(rpKey - Key.NumPad0);
                    break;

                case Key.Multiply:
                    rpBuilder.Append("Num *");
                    break;

                case Key.Add:
                    rpBuilder.Append("Num +");
                    break;

                case Key.Subtract:
                    rpBuilder.Append("Num -");
                    break;

                case Key.Decimal:
                    rpBuilder.Append("Num .");
                    break;

                case Key.Divide:
                    rpBuilder.Append("Num /");
                    break;

                case Key.OemSemicolon:
                    rpBuilder.Append(';');
                    break;

                case Key.OemPlus:
                    rpBuilder.Append('=');
                    break;

                case Key.OemComma:
                    rpBuilder.Append(',');
                    break;

                case Key.OemMinus:
                    rpBuilder.Append('-');
                    break;

                case Key.OemPeriod:
                    rpBuilder.Append('.');
                    break;

                case Key.OemQuestion:
                    rpBuilder.Append('?');
                    break;

                case Key.OemTilde:
                    rpBuilder.Append('~');
                    break;

                case Key.OemOpenBrackets:
                    rpBuilder.Append('[');
                    break;

                case Key.OemPipe:
                    rpBuilder.Append('\\');
                    break;

                case Key.OemCloseBrackets:
                    rpBuilder.Append(']');
                    break;

                case Key.OemQuotes:
                    rpBuilder.Append('"');
                    break;

                case Key.OemBackslash:
                    rpBuilder.Append('?');
                    break;

                default:
                    rpBuilder.Append(rpKey.ToString());
                    break;
            }
        }

        IntPtr WndProc(IntPtr rpHandle, int rpMessage, IntPtr rpWParam, IntPtr rpLParam, ref bool rrpHandled)
        {
            if (rpMessage == (int)NativeConstants.WindowMessage.WM_HOTKEY)
            {
                var rModifierKeys = NativeUtils.LoWord(rpLParam);
                var rKey = NativeUtils.HiWord(rpLParam);

                if (Preference.Current.Other.PanicKey.ModifierKeys == rModifierKeys && Preference.Current.Other.PanicKey.Key == rKey)
                {
                    var rCurrentProcessID = (uint)Process.GetCurrentProcess().Id;

                    NativeMethods.User32.EnumWindows((rpWindowHandle, _) =>
                    {
                        uint rProcessID;
                        NativeMethods.User32.GetWindowThreadProcessId(rpHandle, out rProcessID);
                        if (rProcessID != rCurrentProcessID && (BrowserService.Instance.BrowserProcessID.HasValue && rProcessID != BrowserService.Instance.BrowserProcessID.Value))
                            return false;

                        NativeMethods.User32.ShowWindowAsync(rpHandle, !IsPanicKeyPressed ? NativeConstants.ShowCommand.SW_HIDE : NativeConstants.ShowCommand.SW_SHOW);

                        return true;
                    }, IntPtr.Zero);

                    IsPanicKeyPressed = !IsPanicKeyPressed;
                }
            }

            return IntPtr.Zero;
        }
    }
}
