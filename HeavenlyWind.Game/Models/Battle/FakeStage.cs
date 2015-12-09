using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    class FakeStage : BattleStage
    {
        public override BattleStageType Type => 0;
        public override IList<BattlePhase> Phases => null;

        public FakeStage(BattleInfo rpOwner) : base(rpOwner)
        {
            Func<IParticipant, BattleParticipantSnapshot> rSelector = r =>
            {
                var rHP = ((FriendShip)r).Ship.HP;
                return new BattleParticipantSnapshot(rHP.Maximum, rHP.Current) { Participant = r };
            };

            FriendMain = rpOwner.Participants.FriendMain.Select(rSelector).ToList().AsReadOnly();
            FriendEscort = rpOwner.Participants.FriendEscort?.Select(rSelector).ToList().AsReadOnly();
        }
    }
}
