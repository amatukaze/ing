using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    class ModernizationTrigger : Trigger
    {
        public ModernizationTrigger()
        {
            Observable = SessionService.Instance.GetProcessSucceededSubject("api_req_kaisou/powerup")
                .Where(r => ((RawModernization)r.Data).Success);
        }

        public override string ToString() => "Modernization";
    }
}
