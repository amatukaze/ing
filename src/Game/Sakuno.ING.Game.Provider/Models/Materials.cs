namespace Sakuno.ING.Game.Models
{
    public record Materials
    {
        /// <summary>
        /// 燃料
        /// </summary>
        public int Fuel { get; set; }
        /// <summary>
        /// 弾薬
        /// </summary>
        public int Bullet { get; set; }
        /// <summary>
        /// 鋼材
        /// </summary>
        public int Steel { get; set; }
        /// <summary>
        /// ボーキサイト
        /// </summary>
        public int Bauxite { get; set; }
        /// <summary>
        /// 高速建造材
        /// </summary>
        public int InstantBuild { get; set; }
        /// <summary>
        /// 高速修復材
        /// </summary>
        public int InstantRepair { get; set; }
        /// <summary>
        /// 開発資材
        /// </summary>
        public int Development { get; set; }
        /// <summary>
        /// 改修資材
        /// </summary>
        public int Improvement { get; set; }
    }
}
