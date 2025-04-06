using System.Collections.ObjectModel;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models;

public partial class AirForceGroup
{
    private ObservableCollection<AirForceSquadron> _squadrons;
    private ReadOnlyObservableCollection<AirForceSquadron> _squadronsReadOnly;
    public ReadOnlyObservableCollection<AirForceSquadron> Squadrons => _squadronsReadOnly;

    partial void CreateCore()
    {
        _squadrons = new();
        _squadronsReadOnly = new(_squadrons);
    }
    partial void UpdateCore(IAirForceGroupUpdated raw)
    {
        var i = 0;

        foreach (var squadron in raw.Squadrons)
        {
            while (i < _squadrons.Count && _squadrons[i].Id < squadron.Id)
                _squadrons.RemoveAt(i);

            if (i < _squadrons.Count && _squadrons[i].Id == squadron.Id)
                _squadrons[i++].Update(squadron);
            else
                _squadrons.Insert(i++, new(squadron));
        }
    }
}
