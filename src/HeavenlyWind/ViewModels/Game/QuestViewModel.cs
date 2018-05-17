using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Models.Rewards;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    class QuestViewModel : RawDataWrapper<Quest>, IID
    {
        public Quest Source => RawData;

        public int ID => RawData.ID;

        public IList<QuestReward> Rewards { get; }
        public IList<QuestReward> RewardSelections { get; }

        public QuestRealtimeProgressViewModel RealtimeProgress { get; }

        internal QuestViewModel(Quest rpQuest) : base(rpQuest)
        {
            var rRewards = new List<QuestReward>();

            if (Source.FuelReward > 0)
                rRewards.Add(new QuestMaterialReward(MaterialType.Fuel, Source.FuelReward));
            if (Source.BulletReward > 0)
                rRewards.Add(new QuestMaterialReward(MaterialType.Bullet, Source.BulletReward));
            if (Source.SteelReward > 0)
                rRewards.Add(new QuestMaterialReward(MaterialType.Steel, Source.SteelReward));
            if (Source.BauxiteReward > 0)
                rRewards.Add(new QuestMaterialReward(MaterialType.Bauxite, Source.BauxiteReward));

            if (rpQuest.Extra != null)
            {
                var rExtraRewards = rpQuest.Extra.Rewards;
                if (rExtraRewards != null)
                {
                    var rMaterials = rExtraRewards.Materials;
                    if (rMaterials != null)
                    {
                        if (rMaterials.InstantConstruction > 0)
                            rRewards.Add(new QuestMaterialReward(MaterialType.InstantConstruction, rMaterials.InstantConstruction));
                        if (rMaterials.Bucket > 0)
                            rRewards.Add(new QuestMaterialReward(MaterialType.Bucket, rMaterials.Bucket));
                        if (rMaterials.DevelopmentMaterial > 0)
                            rRewards.Add(new QuestMaterialReward(MaterialType.DevelopmentMaterial, rMaterials.DevelopmentMaterial));
                        if (rMaterials.ImprovementMaterial > 0)
                            rRewards.Add(new QuestMaterialReward(MaterialType.ImprovementMaterial, rMaterials.ImprovementMaterial));
                    }

                    if (rExtraRewards.Equipment != null)
                        rRewards.AddRange(rExtraRewards.Equipment.Select(r => new QuestEquipmentReward(r.ID, r.Count)));

                    if (rExtraRewards.Items != null)
                        rRewards.AddRange(rExtraRewards.Items.Select(r => new QuestItemReward(r.ID, r.Count)));

                    if (rExtraRewards.Furnitures != null)
                        rRewards.AddRange(rExtraRewards.Furnitures.Select(r => new QuestFurnitureReward(r.ID, r.Count)));
                }

                var rExtraRewardSelections = rpQuest.Extra.RewardSelections;
                if (rExtraRewardSelections != null)
                {
                    RewardSelections = rExtraRewardSelections.Select(r =>
                    {
                        QuestReward rResult = null;

                        switch (r.Type)
                        {
                            case 1:
                                rResult = new QuestItemReward(r.ID, r.Count);
                                break;

                            case 3:
                                rResult = new QuestEquipmentReward(r.ID, r.Count);
                                break;

                            case 4:
                                rResult = new QuestFurnitureReward(r.ID, r.Count);
                                break;
                        }

                        return rResult;
                    }).ToArray();
                }
            }

            Rewards = rRewards;

            RealtimeProgress = new QuestRealtimeProgressViewModel(rpQuest);
        }
    }
}
