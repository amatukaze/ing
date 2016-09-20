using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public static class ApiParserManager
    {
        public static Regex TokenRegex { get; } = new Regex(@"(?<=api_token=)\w+");

        static Dictionary<string, ApiParserBase> r_Parsers = new Dictionary<string, ApiParserBase>(StringComparer.OrdinalIgnoreCase);

        static ApiParserManager()
        {
            var rAssembly = Assembly.GetExecutingAssembly();

            var rParserTypes = rAssembly.GetTypes().Where(r => !r.IsAbstract && r.IsSubclassOf(typeof(ApiParserBase)));
            foreach (var rType in rParserTypes)
            {
                var rAttributes = rType.GetCustomAttributes<ApiAttribute>();
                foreach (var rAttribute in rAttributes)
                {
                    var rParser = (ApiParserBase)Activator.CreateInstance(rType);
                    rParser.Api = rAttribute.Name;

                    r_Parsers.Add(rAttribute.Name, rParser);
                }
            }
        }

        internal static ApiParserBase GetParser(string rpApi)
        {
            ApiParserBase rParser;
            if (!r_Parsers.TryGetValue(rpApi, out rParser))
                r_Parsers.Add(rpApi, rParser = new DefaultApiParser());

            return rParser;
        }

        public static void Process(ApiSession rpSession) => Task.Run(() => ProcessCore(rpSession));
        static void ProcessCore(ApiSession rpSession)
        {
            var rApi = rpSession.DisplayUrl;
            var rRequest = Uri.UnescapeDataString(rpSession.RequestBodyString);
            var rResponse = rpSession.ResponseBodyString;

            try
            {
                var rContent = rResponse.Replace("svdata=", string.Empty);

                ApiParserBase rParser;
                if (!rContent.IsNullOrEmpty() && rContent.StartsWith("{") && r_Parsers.TryGetValue(rApi, out rParser))
                {
                    ListDictionary<string, string> rParameters = null;
                    if (!rRequest.IsNullOrEmpty() && rRequest.Contains('&'))
                    {
                        rParameters = new ListDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var rParameter in rRequest.Split('&').Where(r => r.Length > 0).Select(r => r.Split('=')))
                            rParameters.Add(rParameter[0], rParameter[1]);
                    }

                    var rJson = JObject.Parse(rContent);

                    var rResultCode = (int)rJson["api_result"];
                    if (rResultCode != 1)
                    {
                        Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_Failed, rApi, rResultCode));
                        return;
                    }

                    var rData = new ApiInfo(rpSession, rApi, rParameters, rJson);

                    rParser.Process(rData);
                }
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));

                rpSession.ErrorMessage = e.ToString();
                HandleException(rpSession, e);
            }
        }

        internal static void HandleException(ApiSession rpSession, Exception rException)
        {
            try
            {
                using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                {
                    rStreamWriter.WriteLine(TokenRegex.Replace(rpSession.FullUrl, "***************************"));
                    rStreamWriter.WriteLine("Request Data:");
                    rStreamWriter.WriteLine(TokenRegex.Replace(rpSession.RequestBodyString, "***************************"));
                    rStreamWriter.WriteLine();
                    rStreamWriter.WriteLine("Exception:");
                    rStreamWriter.WriteLine(rException.ToString());
                    rStreamWriter.WriteLine();
                    rStreamWriter.WriteLine("Response Data:");
                    rStreamWriter.WriteLine(Regex.Unescape(rpSession.ResponseBodyString));
                }
            }
            catch { }
        }
    }
}
