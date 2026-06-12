namespace Sakuno.ING.Game.Provider;

[RegisterSingleton(Registration = RegistrationStrategy.SelfWithProxyFactory)]
internal partial class GameProvider : IGameProvider, IGameProviderSource
{
}
