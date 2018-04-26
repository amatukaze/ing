using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public struct MaterialUpdates
    {
        private readonly MaterialJson[] raw;
        internal MaterialUpdates(MaterialJson[] raw) => this.raw = raw;

        public void Apply(ref Materials materials)
        {
            foreach (var r in raw)
                switch (r.api_id)
                {
                    case 1:
                        materials.Fuel = r.api_value;
                        break;
                    case 2:
                        materials.Bullet = r.api_value;
                        break;
                    case 3:
                        materials.Steel = r.api_value;
                        break;
                    case 4:
                        materials.Bauxite = r.api_value;
                        break;
                    case 5:
                        materials.InstantBuild = r.api_value;
                        break;
                    case 6:
                        materials.InstantRepair = r.api_value;
                        break;
                    case 7:
                        materials.Development = r.api_value;
                        break;
                    case 8:
                        materials.Improvement = r.api_value;
                        break;
                }
        }
    }
}
