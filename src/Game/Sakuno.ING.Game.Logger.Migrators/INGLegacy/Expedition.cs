#pragma warning disable IDE1006

namespace Sakuno.ING.Game.Logger.Migrators.INGLegacy
{
    internal class Expedition
    {
        public long time { get; set; }
        public int expedition { get; set; }
        public int result { get; set; }
        public int? fuel { get; set; }
        public int? bullet { get; set; }
        public int? steel { get; set; }
        public int? bauxite { get; set; }
        public int? item1 { get; set; }
        public int? item1_count { get; set; }
        public int? item2 { get; set; }
        public int? item2_count { get; set; }
    }
}
