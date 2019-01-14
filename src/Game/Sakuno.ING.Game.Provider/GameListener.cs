using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.Combat;
using Sakuno.ING.Game.Json.MasterData;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    [Export(typeof(GameProvider))]
    public sealed partial class GameProvider
    {
        private readonly ITimedMessageProvider<HttpMessage> provider;
        private readonly JsonSerializer jSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new GameContractResolver()
        };

        public GameProvider(IHttpProviderSelector providerSelector)
        {
            provider = new SynchronizedMessageProvider<HttpMessage>(providerSelector.Current);
            jSerializer.Error += JsonError;

            masterDataUpdated = RegisterResponse<MasterDataJson>("api_start2")
                .CombineWith(RegisterResponse<MasterDataJson>("api_start2/getData"))
                .Select(ParseMasterData);

            var requireInfo = RegisterResponse<GameStartupInfoJson>("api_get_member/require_info");
            allEquipmentUpdated = requireInfo.Select(x => x.api_slot_item)
                .CombineWith(RegisterResponse<RawEquipment[]>("api_get_member/slot_item"));
            useItemUpdated = requireInfo.Select(x => x.api_useitem)
                .CombineWith(RegisterResponse<RawUseItemCount[]>("api_get_member/useitem"));
            freeEquipmentUpdated = requireInfo.Select(x => x.api_unsetslot)
                .CombineWith(RegisterResponse<Dictionary<string, int[]>>("api_get_member/unsetslot"));

            var homeport = RegisterResponse<HomeportJson>("api_port/port");
            admiralUpdated = homeport.Select(x => x.api_basic)
                .CombineWith<RawAdmiral>(
                    RegisterResponse<BasicAdmiral>("api_get_member/basic"),
                    RegisterResponse<RecordAdmiral>("api_get_member/record"));
            repairingDockUpdated = homeport.Select(x => x.api_ndock)
                .CombineWith(RegisterResponse<RawRepairingDock[]>("api_get_member/ndock"));
            homeportReturned = homeport.Select(ParseHomeport);
            compositionChanged = RegisterRequest("api_req_hensei/change")
                .Select(ParseCompositionChange);
            fleetPresetSelected = RegisterResponse<RawFleet>("api_req_hensei/preset_select");
            shipExtraSlotOpened = RegisterRequest("api_req_kaisou/open_exslot")
                .Select(ParseShipExtraSlotOpen);
            shipEquipmentUpdated = RegisterRaw<ShipEquipmentJson>("api_req_kaisou/slot_exchange_index")
                .Select(x => ParseShipEquipmentUpdate(x.Request, x.Response));
            expeditionCompleted = RegisterRaw<ExpeditionCompletionJson>("api_req_mission/result")
                .Select(x => ParseExpeditionCompletion(x.Request, x.Response));

            var ship3 = RegisterResponse<Ship3Json>("api_get_member/ship3")
                .CombineWith(RegisterResponse<Ship3Json>("api_get_member/ship_deck"));
            partialFleetsUpdated = ship3.Select(x => x.api_deck_data);
            partialShipsUpdated = ship3.Select(x => x.api_ship_data)
                .CombineWith(RegisterResponse<RawShip[]>("api_get_member/ship2"),
                RegisterResponse<DepriveJson>("api_req_kaisou/slot_deprive").Select(ParseShipDeprive),
                RegisterResponse<RawShip>("api_req_kaisou/marriage").Select(x => new[] { x }));

            repairStarted = RegisterRequest("api_req_nyukyo/start")
                .Select(ParseRepairStart);
            instantRepaired = RegisterRequest("api_req_nyukyo/speedchange")
                .Select(ParseInstantRepair);
            shipCreated = RegisterRequest("api_req_kousyou/createship")
                .Select(ParseShipCreation);
            instantBuilt = RegisterRequest("api_req_kousyou/createship_speedchange")
                .Select(ParseInstantBuilt);

            var charge = RegisterResponse<ShipsSupplyJson>("api_req_hokyu/charge");
            shipSupplied = charge.Select(x => x.api_ship);

            var getShip = RegisterResponse<ShipBuildCompletionJson>("api_req_kousyou/getship");
            buildingDockUpdated = requireInfo.Select(x => x.api_kdock)
                .CombineWith(getShip.Select(x => x.api_kdock),
                    RegisterResponse<RawBuildingDock[]>("api_get_member/kdock"));
            shipBuildCompleted = getShip.Select(ParseShipBuildCompletion);

            var createItem = RegisterRaw<EquipmentCreationJson>("api_req_kousyou/createitem");
            equipmentCreated = createItem.Select(x => ParseEquipmentCreation(x.Request, x.Response));

            var destroyShip = RegisterRaw<ShipDismantleJson>("api_req_kousyou/destroyship");
            shipDismantled = destroyShip.Select(x => ParseShipDismantling(x.Request));

            var destroyItem = RegisterRaw<EquipmentDismantleJson>("api_req_kousyou/destroyitem2");
            equipmentDismantled = destroyItem.Select(x => ParseEquipmentDimantling(x.Request));
            var improveItem = RegisterRaw<EquipmentImproveJson>("api_req_kousyou/remodel_slot");
            equipmentImproved = improveItem.Select(x => ParseEquipmentImprove(x.Request, x.Response));

            var powerup = RegisterRaw<ShipPowerupJson>("api_req_kaisou/powerup");
            fleetsUpdated = homeport.Select(x => x.api_deck_port)
                .CombineWith(powerup.Select(x => x.Response.api_deck),
                    RegisterResponse<RawFleet[]>("api_get_member/deck"));
            shipPoweruped = powerup.Select(x => ParseShipPowerup(x.Request, x.Response));

            questUpdated = RegisterResponse<QuestPageJson>("api_get_member/questlist")
                .Select(ParseQuestPage);
            questCompleted = RegisterRequest("api_req_quest/clearitemget")
                .Select(ParseQuestComplete);

            var mapinfo = RegisterResponse<MapsJson>("api_get_member/mapinfo");
            mapsUpdated = mapinfo.Select(x => x.api_map_info);
            airForceUpdated = mapinfo.Select(x => x.api_air_base);

            var setPlane = RegisterRaw<AirForceSetPlaneJson>("api_req_air_corps/set_plane");
            airForcePlaneSet = setPlane.Select(x => ParseAirForcePlaneSet(x.Request, x.Response));

            airForceActionSet = RegisterRequest("api_req_air_corps/set_action")
                .Select(ParseAirForceActionSet);
            airForceExpanded = RegisterResponse<RawAirForceGroup>("api_req_air_corps/expand_base");

            var airSupply = RegisterRaw<AirForceSupplyJson>("api_req_air_corps/supply");
            airForceSupplied = airSupply.Select(x => ParseAirForceSupply(x.Request, x.Response));

            materialsUpdated = homeport.Select(x => x.api_material)
                .CombineWith<IMaterialsUpdate>
                    (charge,
                    instantRepaired.Select(x => new InstantRepair()),
                    shipCreated,
                    RegisterResponse<MaterialJsonArray>("api_get_member/material"),
                    createItem.Select(x => x.Response),
                    destroyShip.Select(x => x.Response),
                    destroyItem.Select(x => x.Response),
                    improveItem.Select(x => x.Response),
                    airSupply.Select(x => x.Response),
                    setPlane.Select(x => x.Response).Where(x => x.api_after_bauxite.HasValue));

            incentiveRewarded = RegisterResponse<IncentiveJson>("api_req_member/get_incentive")
                .Select(x => x.api_item);

            enemyDebuffConfirmed = homeport.Where(x => x.api_event_object?.api_m_flag2 == true)
                .Select(x => new Events.Combat.EnemyDebuffConfirm());
            var mapStart = RegisterRaw<MapRoutingJson>("api_req_map/start");
            sortieStarting = mapStart.Select(x => ParseSortieStart(x.Request));
            var routing = mapStart.Select(x => x.Response)
                .CombineWith(RegisterResponse<MapRoutingJson>("api_req_map/next"));
            mapRouting = routing;
            var practice = RegisterRaw<BattleJson>("api_req_practice/battle");
            practiceStarted = practice.Select(x => ParsePracticeStart(x.Request));
            battleStarted = RegisterResponse<BattleJson>("api_req_sortie/battle")
                .CombineWith(RegisterResponse<BattleJson>("api_req_combined_battle/ec_battle"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/battle"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/each_battle"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/battle_water"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/each_battle_water"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/ec_night_to_day"),
                    RegisterResponse<BattleJson>("api_req_sortie/night_to_day"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/sp_midnight"),
                    RegisterResponse<BattleJson>("api_req_battle_midnight/sp_midnight"),
                    RegisterResponse<BattleJson>("api_req_sortie/airbattle"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/airbattle"),
                    RegisterResponse<BattleJson>("api_req_sortie/ld_airbattle"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/ld_airbattle"),
                    RegisterResponse<BattleJson>("api_req_sortie/ld_shooting"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/ld_shooting"),
                    practice.Select(x => x.Response));
            battleAppended = RegisterResponse<BattleJson>("api_req_battle_midnight/battle")
                .CombineWith(RegisterResponse<BattleJson>("api_req_combined_battle/midnight_battle"),
                    RegisterResponse<BattleJson>("api_req_combined_battle/ec_midnight_battle"),
                    RegisterResponse<BattleJson>("api_req_practice/midnight_battle"));
            var battleResult = RegisterResponse<BattleResultJson>("api_req_sortie/battleresult")
                .CombineWith(RegisterResponse<BattleResultJson>("api_req_practice/battleresult"),
                    RegisterResponse<BattleResultJson>("api_req_combined_battle/battleresult"));
            battleCompleted = battleResult;
            mapPartUnlocked = routing.Where(x => x.api_m1).Select(x => new Events.Combat.MapPartUnlock())
                .CombineWith(battleResult.Where(x => x.api_m1).Select(x => new Events.Combat.MapPartUnlock()));
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            SerialzationError?.Invoke(new SerializationError(e.ErrorContext.Error, e.ErrorContext.Path));
            e.ErrorContext.Handled = true;
        }

        public event Action<SerializationError> SerialzationError;
    }
}
