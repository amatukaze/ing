using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Knowledge
{
    public enum KnownEquipmentType
    {
        /// <summary>小口径主砲</summary>
        SmallMainGun = 1,
        /// <summary>中口径主砲</summary>
        MediumMainGun = 2,
        /// <summary>大口径主砲</summary>
        LargeMainGun = 3,
        /// <summary>副砲</summary>
        SecondaryGun = 4,
        /// <summary>魚雷</summary>
        Torpedo = 5,
        /// <summary>艦上戦闘機</summary>
        FighterAircraft = 6,
        /// <summary>艦上爆撃機</summary>
        DiveBomber = 7,
        /// <summary>艦上攻撃機</summary>
        TorpedoBomber = 8,
        /// <summary>艦上偵察機</summary>
        ReconnaissanceAircraft = 9,
        /// <summary>水上偵察機</summary>
        ReconnaissanceSeaplane = 10,
        /// <summary>水上爆撃機</summary>
        SeaplaneBomber = 11,
        /// <summary>小型電探</summary>
        SmallRadar = 12,
        /// <summary>大型電探</summary>
        LargeRadar = 13,
        /// <summary>ソナー</summary>
        Sonar = 14,
        /// <summary>爆雷</summary>
        DepthCharge = 15,
        /// <summary>機関部強化</summary>
        EngineImprovement = 17,
        /// <summary>対空強化弾</summary>
        AAShell = 18,
        /// <summary>対艦強化弾</summary>
        APShell = 19,
        /// <summary>対空機銃</summary>
        AAGun = 21,
        /// <summary>特殊潜航艇</summary>
        MidgetSubmarine = 22,
        /// <summary>応急修理要員</summary>
        DamageControl = 23,
        /// <summary>上陸用舟艇</summary>
        LandingCraft = 24,
        /// <summary>オートジャイロ</summary>
        Autogyro = 25,
        /// <summary>対潜哨戒機</summary>
        AntiSubmarinePatrolAircraft = 26,
        /// <summary>追加装甲(中型)</summary>
        MediumExtraArmor = 27,
        /// <summary>追加装甲(大型)</summary>
        LargeExtraArmor = 28,
        /// <summary>探照灯</summary>
        Searchlight = 29,
        /// <summary>簡易輸送部材</summary>
        TransportContainer = 30,
        /// <summary>艦艇修理施設</summary>
        RepairFacility = 31,
        /// <summary>潜水艦魚雷</summary>
        SubmarineTorpedo = 32,
        /// <summary>照明弾</summary>
        StarShell = 33,
        /// <summary>司令部施設</summary>
        CommandFacility = 34,
        /// <summary>航空要員</summary>
        AviationPersonnel = 35,
        /// <summary>高射装置</summary>
        AAFireDirector = 36,
        /// <summary>対地装備</summary>
        AntiGroundEquipment = 37,
        /// <summary>大口径主砲（II）</summary>
        VeryLargeMainGun = 38,
        /// <summary>水上艦要員</summary>
        SurfaceShipPersonnal = 39,
        /// <summary>大型ソナー</summary>
        LargeSonar = 40,
        /// <summary>大型飛行艇</summary>
        LargeFlyingBoat = 41,
        /// <summary>大型探照灯</summary>
        LargeSearchlight = 42,
        /// <summary>戦闘糧食</summary>
        CombatRation = 43,
        /// <summary>補給物資</summary>
        Supplies = 44,
        /// <summary>水上戦闘機</summary>
        SeaplaneFighter = 45,
        /// <summary>特型内火艇</summary>
        AmphibiousTank = 46,
        /// <summary>陸上攻撃機</summary>
        LandBasedAttacker = 47,
        /// <summary>局地戦闘機</summary>
        LandBasedFighter = 48,
        /// <summary>輸送機材</summary>
        TransportationMaterial = 50,
        /// <summary>潜水艦装備</summary>
        SubmarineEquipment = 51,
        /// <summary>噴式戦闘爆撃機</summary>
        JetBomber = 57,
        /// <summary>大型電探（II）</summary>
        VeryLargeRadar = 93
    }

    public static class KnownEquipmentTypeExtensions
    {
        public static bool IsPlane(this EquipmentTypeId id)
            => (KnownEquipmentType)id switch
            {
                KnownEquipmentType.FighterAircraft => true,
                KnownEquipmentType.DiveBomber => true,
                KnownEquipmentType.TorpedoBomber => true,
                KnownEquipmentType.ReconnaissanceAircraft => true,
                KnownEquipmentType.ReconnaissanceSeaplane => true,
                KnownEquipmentType.SeaplaneBomber => true,
                KnownEquipmentType.Autogyro => true,
                KnownEquipmentType.AntiSubmarinePatrolAircraft => true,
                KnownEquipmentType.LargeFlyingBoat => true,
                KnownEquipmentType.SeaplaneFighter => true,
                _ => false
            };

        public static bool IsRadar(this EquipmentTypeId id)
            => (KnownEquipmentType)id switch
            {
                KnownEquipmentType.SmallRadar => true,
                KnownEquipmentType.LargeRadar => true,
                KnownEquipmentType.VeryLargeRadar => true,
                _ => false
            };
    }
}
