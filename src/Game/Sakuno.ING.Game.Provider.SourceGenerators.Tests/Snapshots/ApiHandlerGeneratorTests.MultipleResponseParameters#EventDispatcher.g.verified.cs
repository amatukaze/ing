//HintName: EventDispatcher.g.cs
#nullable enable
using System;
using System.Text.Json;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Provider;
partial class EventDispatcher
{
    private partial bool HandleApiMessageCore(ApiMessage message)
    {
        switch (message.Api)
        {
        }

        return false;
    }
}