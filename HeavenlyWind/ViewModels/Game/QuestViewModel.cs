using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Models.Rewards;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestViewModel : RawDataWrapper<Quest>, IID
    {
        public Quest Source => RawData;

        public int ID => RawData.ID;

        public IList<QuestReward> Rewards { get; }

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

            if (rpQuest.Extra != null && rpQuest.Extra.Rewards != null)
            {
                var rExtraRewards = rpQuest.Extra.Rewards;

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
            }

            Rewards = rRewards;
        }
    }
}
