namespace Sakuno.ING.Game.Models.Battle
{
    public enum Formation
    {
        /// <summary>単縦陣</summary>
        LineAhead = 1,
        /// <summary>複縦陣</summary>
        DoubleLine = 2,
        /// <summary>輪形陣</summary>
        Diamond = 3,
        /// <summary>梯形陣</summary>
        Echelon = 4,
        /// <summary>単横陣</summary>
        LineAbreast = 5,
        /// <summary>警戒陣</summary>
        Vanguard = 6,
        /// <summary>第一警戒航行序列</summary>
        CruisingFormation1 = 11,
        /// <summary>第二警戒航行序列</summary>
        CruisingFormation2 = 12,
        /// <summary>第三警戒航行序列</summary>
        CruisingFormation3 = 13,
        /// <summary>第四警戒航行序列</summary>
        CruisingFormation4 = 14
    }
}
