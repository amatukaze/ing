using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class HomeportMaterialUpdate : IMaterialUpdate
    {
        private readonly RawMaterialItem[] _items;

        public HomeportMaterialUpdate(RawMaterialItem[] items)
        {
            _items = items;
        }

        public void Apply(Materials materials)
        {
            foreach (var item in _items)
            {
                switch (item.api_id)
                {
                    case 1:
                        materials.Fuel = item.api_value;
                        break;

                    case 2:
                        materials.Bullet = item.api_value;
                        break;

                    case 3:
                        materials.Steel = item.api_value;
                        break;

                    case 4:
                        materials.Bauxite = item.api_value;
                        break;

                    case 5:
                        materials.InstantBuild = item.api_value;
                        break;

                    case 6:
                        materials.InstantRepair = item.api_value;
                        break;

                    case 7:
                        materials.Development = item.api_value;
                        break;

                    case 8:
                        materials.Improvement = item.api_value;
                        break;
                }
            }
        }
    }
}
