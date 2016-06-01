using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    class ApiTrigger : Trigger
    {
        public string Api { get; }

        public ApiTrigger(string rpApi)
        {
            Api = rpApi;

            Observable = SessionService.Instance.GetObservable(rpApi);
        }

        public override string ToString() => "Api: " + Api;
    }
}
