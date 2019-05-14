namespace Sakuno.ING.Game.Models.Combat
{
    public abstract partial class BattleBase : BindableObject
    {
        public Side Ally { get; }

        protected readonly BindableCollection<BattlePhase> phases = new BindableCollection<BattlePhase>();
        public IBindableCollection<BattlePhase> OrderedPhases => phases;

        protected BattleBase(Side ally)
        {
            Ally = ally;
        }
    }
}
