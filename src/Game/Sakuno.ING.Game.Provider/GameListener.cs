using System;
using Newtonsoft.Json;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.MasterData;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Game
{
    [Export(typeof(GameProvider))]
    public sealed partial class GameProvider
    {
        private readonly ISettingItem<string> savedMasterData;
        private readonly ITimedMessageProvider<HttpMessage> provider;
        private readonly JsonSerializer jSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new GameContractResolver()
        };

        private void HandleMessage(DateTimeOffset t, HttpMessage m)
        {
            var status = Convert<SvData>(m.Response);
            if (status.api_result != 1)
            {
                GameError?.Invoke(t, new Events.GameError(status.api_result, status.api_result_msg));
                return;
            }
            switch (m.Key)
            {
                case "api_start2":
                case "api_start2/getData":
                    if (savedMasterData != null)
                        savedMasterData.Value = m.Response.ToString();
                    masterDataUpdated?.Invoke(t, ParseMasterData(Response<MasterDataJson>(m)));
                    break;
                case "api_get_member/require_info":
                    var requireInfo = Response<GameStartupInfoJson>(m);
                    AllEquipmentUpdated?.Invoke(t, requireInfo.api_slot_item);
                    UseItemUpdated?.Invoke(t, requireInfo.api_useitem);
                    FreeEquipmentUpdated?.Invoke(t, requireInfo.api_unsetslot);
                    BuildingDockUpdated?.Invoke(t, requireInfo.api_kdock);
                    break;
                case "api_req_member/get_incentive":
                    IncentiveRewarded?.Invoke(t, Response<IncentiveJson>(m).api_item);
                    break;

                case "api_port/port":
                    var homeport = Response<HomeportJson>(m);
                    AdmiralUpdated?.Invoke(t, homeport.api_basic);
                    RepairingDockUpdated?.Invoke(t, homeport.api_ndock);
                    HomeportReturned?.Invoke(t, ParseHomeport(homeport));
                    FleetsUpdated?.Invoke(t, homeport.api_deck_port);
                    MaterialsUpdated?.Invoke(t, homeport.api_material);
                    if (homeport.api_event_object?.api_m_flag2 == true)
                        EnemyDebuffConfirmed?.Invoke(t, default);
                    break;
                case "api_get_member/basic":
                    AdmiralUpdated?.Invoke(t, Response<BasicAdmiral>(m));
                    break;
                case "api_get_member/record":
                    AdmiralUpdated?.Invoke(t, Response<RecordAdmiral>(m));
                    break;
                case "api_get_member/ndock":
                    RepairingDockUpdated?.Invoke(t, Response<RawRepairingDock[]>(m));
                    break;
                case "api_req_hensei/change":
                    CompositionChanged?.Invoke(t, ParseCompositionChange(Request(m)));
                    break;
                case "api_req_hensei/preset_select":
                    FleetPresetSelected?.Invoke(t, Response<RawFleet>(m));
                    break;
                case "api_req_kaisou/open_exslot":
                    ShipExtraSlotOpened?.Invoke(t, ParseShipExtraSlotOpen(Request(m)));
                    break;
                case "api_req_mission/result":
                    ExpeditionCompleted?.Invoke(t, ParseExpeditionCompletion(Request(m), Response<ExpeditionCompletionJson>(m)));
                    break;

                case "api_get_member/ship3":
                case "api_get_member/ship_deck":
                    var ship3 = Response<Ship3Json>(m);
                    PartialFleetsUpdated?.Invoke(t, ship3.api_deck_data);
                    PartialShipsUpdated?.Invoke(t, ship3.api_ship_data);
                    break;
                case "api_req_kaisou/slot_exchange_index":
                    var equipmentExchange = Response<EquipmentExchangeJson>(m);
                    PartialShipsUpdated?.Invoke(t, new[] { equipmentExchange.api_ship_data });
                    MaterialsUpdated?.Invoke(t, equipmentExchange);
                    break;
                case "api_req_kaisou/slot_deprive":
                    var equipmentDeprive = Response<DepriveJson>(m);
                    PartialShipsUpdated?.Invoke(t, ParseShipDeprive(equipmentDeprive));
                    MaterialsUpdated?.Invoke(t, equipmentDeprive);
                    break;
                case "api_get_member/ship2":
                    PartialShipsUpdated?.Invoke(t, Response<RawShip[]>(m));
                    break;
                case "api_req_kaisou/marriage":
                    PartialShipsUpdated?.Invoke(t, new[] { Response<RawShip>(m) });
                    break;
                case "api_get_member/material":
                    MaterialsUpdated?.Invoke(t, Response<MaterialJsonArray>(m));
                    break;
                case "api_req_kaisou/slotset":
                    MaterialsUpdated?.Invoke(t, Response<EquipmentSetupJson>(m));
                    break;

                case "api_req_nyukyo/start":
                    RepairStarted?.Invoke(t, ParseRepairStart(Request(m)));
                    break;
                case "api_req_nyukyo/speedchange":
                    InstantRepaired?.Invoke(t, ParseInstantRepair(Request(m)));
                    break;
                case "api_req_kousyou/createship":
                    ShipCreated?.Invoke(t, ParseShipCreation(Request(m)));
                    break;
                case "api_req_kousyou/createship_speedchange":
                    InstantBuilt?.Invoke(t, ParseInstantBuilt(Request(m)));
                    break;

                case "api_req_hokyu/charge":
                    var charge = Response<ShipsSupplyJson>(m);
                    ShipSupplied?.Invoke(t, charge.api_ship);
                    MaterialsUpdated?.Invoke(t, charge);
                    break;
                case "api_req_kousyou/getship":
                    var getShip = Response<ShipBuildCompletionJson>(m);
                    BuildingDockUpdated?.Invoke(t, getShip.api_kdock);
                    ShipBuildCompleted?.Invoke(t, ParseShipBuildCompletion(getShip));
                    break;
                case "api_get_member/kdock":
                    BuildingDockUpdated?.Invoke(t, Response<RawBuildingDock[]>(m));
                    break;

                case "api_req_kousyou/createitem":
                    var createItem = Response<EquipmentCreationJson>(m);
                    EquipmentCreated?.Invoke(t, ParseEquipmentCreation(Request(m), createItem));
                    MaterialsUpdated?.Invoke(t, createItem);
                    break;
                case "api_req_kousyou/destroyship":
                    ShipDismantled?.Invoke(t, ParseShipDismantling(Request(m)));
                    MaterialsUpdated?.Invoke(t, Response<ShipDismantleJson>(m));
                    break;
                case "api_req_kousyou/destroyitem2":
                    EquipmentDismantled?.Invoke(t, ParseEquipmentDimantling(Request(m)));
                    MaterialsUpdated?.Invoke(t, Response<EquipmentDismantleJson>(m));
                    break;
                case "api_req_kousyou/remodel_slot":
                    var improveItem = Response<EquipmentImproveJson>(m);
                    EquipmentImproved?.Invoke(t, ParseEquipmentImprove(Request(m), improveItem));
                    MaterialsUpdated?.Invoke(t, improveItem);
                    break;
                case "api_req_kaisou/powerup":
                    var powerup = Response<ShipPowerupJson>(m);
                    FleetsUpdated?.Invoke(t, powerup.api_deck);
                    ShipPoweruped?.Invoke(t, ParseShipPowerup(Request(m), powerup));
                    break;
                case "api_get_member/deck":
                    FleetsUpdated?.Invoke(t, Response<RawFleet[]>(m));
                    break;

                case "api_get_member/questlist":
                    QuestUpdated?.Invoke(t, ParseQuestPage(Response<QuestPageJson>(m)));
                    break;
                case "api_req_quest/clearitemget":
                    QuestCompleted?.Invoke(t, ParseQuestComplete(Request(m)));
                    break;

                case "api_get_member/mapinfo":
                    var mapinfo = Response<MapsJson>(m);
                    MapsUpdated?.Invoke(t, mapinfo.api_map_info);
                    AirForceUpdated?.Invoke(t, mapinfo.api_air_base);
                    break;
                case "api_req_air_corps/set_plane":
                    var setPlane = Response<AirForceSetPlaneJson>(m);
                    AirForcePlaneSet?.Invoke(t, ParseAirForcePlaneSet(Request(m), setPlane));
                    if (setPlane.api_after_bauxite != null)
                        MaterialsUpdated?.Invoke(t, setPlane);
                    break;
                case "api_req_air_corps/set_action":
                    AirForceActionSet?.Invoke(t, ParseAirForceActionSet(Request(m)));
                    break;
                case "api_req_air_corps/expand_base":
                    AirForceExpanded?.Invoke(t, Response<RawAirForceGroup>(m));
                    break;
                case "api_req_air_corps/supply":
                    var airSupply = Response<AirForceSupplyJson>(m);
                    AirForceSupplied?.Invoke(t, ParseAirForceSupply(Request(m), airSupply));
                    MaterialsUpdated?.Invoke(t, airSupply);
                    break;

                case "api_req_member/get_practice_enemyinfo":
                    ExerciseCandidateSelected?.Invoke(t, Response<RawExerciseCandidate>(m));
                    break;
                case "api_req_map/start":
                    SortieStarting?.Invoke(t, ParseSortieStart(Request(m)));
                    goto case "api_req_map/next";
                case "api_req_map/next":
                    var routing = Response<RawMapRouting>(m);
                    HandleMapRouting(routing);
                    MapRouting?.Invoke(t, routing);
                    if (routing.api_m1)
                        MapPartUnlocked?.Invoke(t, default);
                    break;
                case "api_req_practice/battle":
                    ExerciseStarted?.Invoke(t, ParseExerciseStart(Request(m)));
                    goto case "api_req_sortie/battle";
                case "api_req_sortie/battle":
                case "api_req_combined_battle/ec_battle":
                case "api_req_combined_battle/battle":
                case "api_req_combined_battle/each_battle":
                case "api_req_combined_battle/battle_water":
                case "api_req_combined_battle/each_battle_water":
                case "api_req_combined_battle/ec_night_to_day":
                case "api_req_sortie/night_to_day":
                case "api_req_combined_battle/sp_midnight":
                case "api_req_battle_midnight/sp_midnight":
                case "api_req_sortie/airbattle":
                case "api_req_combined_battle/airbattle":
                case "api_req_sortie/ld_airbattle":
                case "api_req_combined_battle/ld_airbattle":
                case "api_req_sortie/ld_shooting":
                case "api_req_combined_battle/ld_shooting":
                    BattleStarted?.Invoke(t, ParseBattleDetail(m));
                    break;
                case "api_req_practice/midnight_battle":
                case "api_req_battle_midnight/battle":
                case "api_req_combined_battle/midnight_battle":
                case "api_req_combined_battle/ec_midnight_battle":
                    BattleAppended?.Invoke(t, ParseBattleDetail(m));
                    break;
                case "api_req_sortie/battleresult":
                case "api_req_practice/battleresult":
                case "api_req_combined_battle/battleresult":
                    var battleResult = Response<RawBattleResult>(m);
                    BattleCompleted?.Invoke(t, battleResult);
                    if (battleResult.api_m1)
                        MapPartUnlocked?.Invoke(t, default);
                    break;
            }
        }

        public GameProvider(IHttpProviderSelector providerSelector, ISettingsManager settings = null)
        {
            if (settings != null)
                savedMasterData = settings.Register<string>("game.master_data");
            provider = new SynchronizedMessageProvider<HttpMessage>(providerSelector.Current);
            jSerializer.Error += JsonError;
            provider.Received += HandleMessage;
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            SerialzationError?.Invoke(new SerializationError(e.ErrorContext.Error, e.ErrorContext.Path));
            e.ErrorContext.Handled = true;
        }

        public event Action<SerializationError> SerialzationError;
        public event TimedMessageHandler<Events.GameError> GameError;
    }
}
