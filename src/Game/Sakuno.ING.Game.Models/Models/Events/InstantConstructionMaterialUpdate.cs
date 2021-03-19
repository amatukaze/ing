using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models.Events
{
    internal class InstantConstructionMaterialUpdate : IMaterialUpdate
    {
        private readonly bool _isLSC;

        public InstantConstructionMaterialUpdate(bool isLSC)
        {
            _isLSC = isLSC;
        }

        public void Apply(Materials materials) => materials.Development -= _isLSC ? 10 : 1;
    }
}
