using System.Globalization;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    static class UnhandledExceptionDialogStringResources
    {
        public static string ProductName
        {
            get
            {
                switch (CultureInfo.CurrentUICulture.Name)
                {
                    case "ja-JP":
                        return "いんてりじぇんと連装砲くん";

                    case "zh-CN":
                        return "智能型连装炮君";

                    case "zh-HK":
                    case "zh-TW":
                        return "智能型連裝砲君";

                    default:
                        return "Intelligent Naval Gun";
                }
            }
        }

        public static string Instruction
        {
            get
            {
                switch (CultureInfo.CurrentUICulture.Name)
                {
                    case "ja-JP":
                        return "しまった！";

                    case "zh-CN":
                    case "zh-HK":
                    case "zh-TW":
                        return "哎呀！";

                    default:
                        return "Oops!";
                }
            }
        }

        public static string Content
        {
            get
            {
                switch (CultureInfo.CurrentUICulture.Name)
                {
                    case "ja-JP":
                        return "予期しないエラーが発生しました。";

                    case "zh-CN":
                        return "遇到了一些无法处理的错误。";

                    case "zh-HK":
                    case "zh-TW":
                        return "遇到了一些無法處理的錯誤。";

                    default:
                        return "An unhandled exception occurred.";
                }
            }
        }

        public static string Footer
        {
            get
            {
                switch (CultureInfo.CurrentUICulture.Name)
                {
                    case "ja-JP":
                        return "エラーのログは {0} に保存されました。";

                    case "zh-CN":
                        return "该错误的详细内容已保存到 {0}。";

                    case "zh-HK":
                    case "zh-TW":
                        return "該錯誤的詳細內容已保存到 {0}。";

                    default:
                        return "The detail has been saved to {0}.";
                }
            }
        }
    }
}
