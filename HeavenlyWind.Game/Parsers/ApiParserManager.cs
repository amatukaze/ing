using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public sealed class ApiParserManager
    {
        public static ApiParserManager Instance { get; } = new ApiParserManager();

        public static Regex TokenRegex { get; } = new Regex(@"(?<=api_token=)\w+");

        internal Dictionary<string, ApiParserBase> Parsers { get; } = new Dictionary<string, ApiParserBase>();

        Subject<ApiSession> r_SessionSources = new Subject<ApiSession>();
        public Subject<Tuple<ApiSession, Exception>> ExceptionSources { get; } = new Subject<Tuple<ApiSession, Exception>>();

        ApiParserManager()
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

                    Parsers.Add(rAttribute.Name, rParser);
                }
            }

            r_SessionSources.Subscribe(ProcessCore);
            ExceptionSources.Subscribe(rpData =>
            {
                var rSession = rpData.Item1;
                var rException = rpData.Item2;

                rSession.ErrorMessage = rException.ToString();

                try {
                    using (var rStreamWriter = new StreamWriter(Logger.GetNewExceptionLogFilename(), false, new UTF8Encoding(true)))
                    {
                        rStreamWriter.WriteLine(TokenRegex.Replace(rSession.FullUrl, "***************************"));
                        rStreamWriter.WriteLine("Request Data:");
                        rStreamWriter.WriteLine(TokenRegex.Replace(rSession.RequestBodyString, "***************************"));
                        rStreamWriter.WriteLine();
                        rStreamWriter.WriteLine("Exception:");
                        rStreamWriter.WriteLine(rException.ToString());
                        rStreamWriter.WriteLine();
                        rStreamWriter.WriteLine("Response Data:");
                        rStreamWriter.WriteLine(Regex.Unescape(rSession.ResponseBodyString));
                    }
                }
                catch { }
            });
        }

        public void Process(ApiSession rpSession) => r_SessionSources.OnNext(rpSession);
        void ProcessCore(ApiSession rpSession)
        {
            var rApi = rpSession.DisplayUrl;
            var rRequest = rpSession.RequestBodyString;
            var rResponse = rpSession.ResponseBodyString;

            try
            {
                var rContent = rResponse.Replace("svdata=", string.Empty);

                ApiParserBase rParser;
                if (!rContent.IsNullOrEmpty() && rContent.StartsWith("{") && Parsers.TryGetValue(rApi, out rParser))
                {
                    ListDictionary<string, string> rParameters = null;
                    if (!rRequest.IsNullOrEmpty() && rRequest.Contains('&'))
                    {
                        rParameters = new ListDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var rParameter in rRequest.Split('&').Where(r => r.Length > 0).Select(r => r.Split('=')))
                            rParameters.Add(Uri.UnescapeDataString(rParameter[0]), Uri.UnescapeDataString(rParameter[1]));
                    }

                    rParser.Parameters = rParameters;
                    rParser.Process(JObject.Parse(rContent));
                    rParser.Parameters = null;
                }
            }
            catch (ApiFailedException e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_Failed, rApi, e.ResultCode));
            }
            catch (Exception e)
            {
                Logger.Write(LoggingLevel.Error, string.Format(StringResources.Instance.Main.Log_Exception_API_ParseException, e.Message));
                ExceptionSources.OnNext(Tuple.Create(rpSession, e));
            }
        }
    }
}
