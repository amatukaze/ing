namespace Sakuno.ING.Game.Provider.Json;

public partial class RawAdmiral
{
    AdmiralId IIdentifiable<AdmiralId>.Id => api_member_id;
}
