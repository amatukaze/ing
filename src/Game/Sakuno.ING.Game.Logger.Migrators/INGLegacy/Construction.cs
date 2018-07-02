#pragma warning disable IDE1006

namespace Sakuno.ING.Game.Logger.Migrators.INGLegacy
{
    internal class Construction
    {
        public long time { get; set; }
        public int ship { get; set; }
        public int fuel { get; set; }
        public int bullet { get; set; }
        public int steel { get; set; }
        public int bauxite { get; set; }
        public int dev_material { get; set; }
        public int flagship { get; set; }
        public int hq_level { get; set; }
        public bool is_lsc { get; set; }
        public int? empty_dock { get; set; }
    }
}
