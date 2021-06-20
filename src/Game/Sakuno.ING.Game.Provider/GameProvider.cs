using Sakuno.ING.Composition;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Messaging;
using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Sakuno.ING.Game
{
    [Export]
    public sealed partial class GameProvider
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public GameProvider(IApiMessageSource apiMessageSource)
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            _serializerOptions.Converters.Add(new IdentifierConverterFactory());
            _serializerOptions.Converters.Add(new MaterialsConverter());
            _serializerOptions.Converters.Add(new ShipModernizationConverter());
            _serializerOptions.Converters.Add(new ExpeditionUseItemRewardConverter());
            _serializerOptions.Converters.Add(new TimestampInMillisecondConverter());
            _serializerOptions.Converters.Add(new UnequippedSlotItemInfoConverter());

            apiMessageSource.ApiMessageSource.Subscribe(message =>
            {
                HandleApiMessageCore(message);
            });
        }

        private partial void HandleApiMessageCore(ApiMessage message);

        private NameValueCollection ParseRequest(ReadOnlyMemory<char> request) =>
            HttpUtility.ParseQueryString(request.ToString());

        private SvData<T> Deserialize<T>(ApiMessage message) =>
            JsonSerializer.Deserialize<SvData<T>>(message.Response.Span, _serializerOptions)!;
        private SvDataRequestOnly DeserializeRequestOnly(ApiMessage message)
        {
            var result = JsonSerializer.Deserialize<SvDataRequestOnly>(message.Response.Span, _serializerOptions)!;
            result.Api = message.Api;
            result.Request = ParseRequest(message.Request);

            return result;
        }
        private SvDataWithRequest<T> DeserializeWithRequest<T>(ApiMessage message)
        {
            var result = JsonSerializer.Deserialize<SvDataWithRequest<T>>(message.Response.Span, _serializerOptions)!;
            result.Request = ParseRequest(message.Request);

            return result;
        }

        private void CheckResultCode(SvData response)
        {
            if (response.api_result is not 1)
                throw new Exception();
        }
    }
}
