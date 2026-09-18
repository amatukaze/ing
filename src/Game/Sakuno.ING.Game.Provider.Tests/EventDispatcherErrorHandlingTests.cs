using System.Buffers;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Sakuno.ING.Game.Events;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class EventDispatcherErrorHandlingTests
{
    [Fact]
    public void CorruptedResponseDoesNotKillMessageStream()
    {
        var (providerSource, _, messages) = CreateDispatcher();

        messages.OnNext(CreateMessage("api_get_member/material", """{"api_result":1,"api_data":[{"api_id":1,"api_value":100}]}"""));
        messages.OnNext(CreateMessage("api_get_member/material", """not json"""));
        messages.OnNext(CreateMessage("api_get_member/material", """{"api_result":1,"api_data":[{"api_id":1,"api_value":200}]}"""));

        providerSource.Received(2).OnMaterialsUpdated(Arg.Any<IMaterialsUpdated>());
        providerSource.Received(3).OnCommitted(Arg.Any<Unit>());
    }

    [Fact]
    public void RejectedResultCodeSkipsMessage()
    {
        var (providerSource, _, messages) = CreateDispatcher();

        messages.OnNext(CreateMessage("api_get_member/material", """{"api_result":100,"api_data":null}"""));
        messages.OnNext(CreateMessage("api_get_member/material", """{"api_result":1,"api_data":[{"api_id":1,"api_value":100}]}"""));

        providerSource.Received(1).OnMaterialsUpdated(Arg.Any<IMaterialsUpdated>());
        providerSource.Received(2).OnCommitted(Arg.Any<Unit>());
    }

    private static (IGameProviderSource ProviderSource, EventDispatcher Dispatcher, Subject<ApiMessage> Messages) CreateDispatcher()
    {
        var providerSource = Substitute.For<IGameProviderSource>();
        var apiMessageProvider = Substitute.For<IApiMessageProvider>();
        var messages = new Subject<ApiMessage>();
        apiMessageProvider.ApiMessages.Returns(messages);

        var dispatcher = new EventDispatcher(providerSource,
            Substitute.For<IMasterDataSnapshotService>(),
            Substitute.For<IPlayerDataSnapshotService>(),
            apiMessageProvider,
            NullLogger<EventDispatcher>.Instance);

        return (providerSource, dispatcher, messages);
    }

    private static ApiMessage CreateMessage(string api, string response)
    {
        var bytes = Encoding.UTF8.GetBytes(response);
        return new(api, ReadOnlyMemory<byte>.Empty, new ReadOnlySequence<byte>(bytes));
    }
}
