using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly object lockObj = new object();
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

            lock (lockObj)
                switch (m.Key)
                {
                    case "api_start2":
                    case "api_start2/getData":
                        if (savedMasterData != null)
                            savedMasterData.Value = m.Response.ToString();
                        masterDataUpdated?.InvokeEach(t, ParseMasterData(Response<MasterDataJson>(m)), HandlerError);
                        break;
                    case "api_get_member/require_info":
                        var requireInfo = Response<GameStartupInfoJson>(m);
                        AllEquipmentUpdated?.InvokeEach(t, requireInfo.api_slot_item, HandlerError);
                        UseItemUpdated?.InvokeEach(t, requireInfo.api_useitem, HandlerError);
                        FreeEquipmentUpdated?.InvokeEach(t, requireInfo.api_unsetslot, HandlerError);
                        BuildingDockUpdated?.InvokeEach(t, requireInfo.api_kdock, HandlerError);
                        break;
                    case "api_req_member/get_incentive":
                        IncentiveRewarded?.InvokeEach(t, Response<IncentiveJson>(m).api_item, HandlerError);
                        break;

                    case "api_port/port":
                        var homeport = Response<HomeportJson>(m);
                        AdmiralUpdated?.InvokeEach(t, homeport.api_basic, HandlerError);
                        RepairingDockUpdated?.InvokeEach(t, homeport.api_ndock, HandlerError);
                        HomeportReturned?.InvokeEach(t, ParseHomeport(homeport), HandlerError);
                        FleetsUpdated?.InvokeEach(t, homeport.api_deck_port, HandlerError);
                        MaterialsUpdated?.InvokeEach(t, homeport.api_material, HandlerError);
                        if (homeport.api_event_object?.api_m_flag2 == true)
                            EnemyDebuffConfirmed?.InvokeEach(t, default, HandlerError);
                        break;
                    case "api_get_member/slot_item":
                        AllEquipmentUpdated?.InvokeEach(t, Response<RawEquipment[]>(m), HandlerError);
                        break;
                    case "api_get_member/basic":
                        AdmiralUpdated?.InvokeEach(t, Response<BasicAdmiral>(m), HandlerError);
                        break;
                    case "api_get_member/record":
                        AdmiralUpdated?.InvokeEach(t, Response<RecordAdmiral>(m), HandlerError);
                        break;
                    case "api_get_member/ndock":
                        RepairingDockUpdated?.InvokeEach(t, Response<RawRepairingDock[]>(m), HandlerError);
                        break;
                    case "api_req_hensei/change":
                        CompositionChanged?.InvokeEach(t, ParseCompositionChange(Request(m)), HandlerError);
                        break;
                    case "api_req_hensei/preset_select":
                        FleetPresetSelected?.InvokeEach(t, Response<RawFleet>(m), HandlerError);
                        break;
                    case "api_req_kaisou/open_exslot":
                        ShipExtraSlotOpened?.InvokeEach(t, ParseShipExtraSlotOpen(Request(m)), HandlerError);
                        break;
                    case "api_req_mission/result":
                        ExpeditionCompleted?.InvokeEach(t, ParseExpeditionCompletion(Request(m), Response<ExpeditionCompletionJson>(m)), HandlerError);
                        break;

                    case "api_get_member/ship3":
                    case "api_get_member/ship_deck":
                        var ship3 = Response<Ship3Json>(m);
                        PartialFleetsUpdated?.InvokeEach(t, ship3.api_deck_data, HandlerError);
                        PartialShipsUpdated?.InvokeEach(t, ship3.api_ship_data, HandlerError);
                        break;
                    case "api_req_kaisou/slot_exchange_index":
                        var equipmentExchange = Response<EquipmentExchangeJson>(m);
                        PartialShipsUpdated?.InvokeEach(t, new[] { equipmentExchange.api_ship_data }, HandlerError);
                        MaterialsUpdated?.InvokeEach(t, equipmentExchange, HandlerError);
                        break;
                    case "api_req_kaisou/slot_deprive":
                        var equipmentDeprive = Response<DepriveJson>(m);
                        PartialShipsUpdated?.InvokeEach(t, ParseShipDeprive(equipmentDeprive), HandlerError);
                        MaterialsUpdated?.InvokeEach(t, equipmentDeprive, HandlerError);
                        break;
                    case "api_get_member/ship2":
                        PartialShipsUpdated?.InvokeEach(t, Response<RawShip[]>(m), HandlerError);
                        break;
                    case "api_req_kaisou/marriage":
                        PartialShipsUpdated?.InvokeEach(t, new[] { Response<RawShip>(m) }, HandlerError);
                        break;
                    case "api_get_member/material":
                        MaterialsUpdated?.InvokeEach(t, Response<MaterialJsonArray>(m), HandlerError);
                        break;
                    case "api_req_kaisou/slotset":
                        MaterialsUpdated?.InvokeEach(t, Response<EquipmentSetupJson>(m), HandlerError);
                        break;

                    case "api_req_nyukyo/start":
                        RepairStarted?.InvokeEach(t, ParseRepairStart(Request(m)), HandlerError);
                        break;
                    case "api_req_nyukyo/speedchange":
                        InstantRepaired?.InvokeEach(t, ParseInstantRepair(Request(m)), HandlerError);
                        break;
                    case "api_req_kousyou/createship":
                        ShipCreated?.InvokeEach(t, ParseShipCreation(Request(m)), HandlerError);
                        break;
                    case "api_req_kousyou/createship_speedchange":
                        InstantBuilt?.InvokeEach(t, ParseInstantBuilt(Request(m)), HandlerError);
                        break;

                    case "api_req_hokyu/charge":
                        var charge = Response<ShipsSupplyJson>(m);
                        ShipSupplied?.InvokeEach(t, charge.api_ship, HandlerError);
                        MaterialsUpdated?.InvokeEach(t, charge, HandlerError);
                        break;
                    case "api_req_kousyou/getship":
                        var getShip = Response<ShipBuildCompletionJson>(m);
                        BuildingDockUpdated?.InvokeEach(t, getShip.api_kdock, HandlerError);
                        ShipBuildCompleted?.InvokeEach(t, ParseShipBuildCompletion(getShip), HandlerError);
                        break;
                    case "api_get_member/kdock":
                        BuildingDockUpdated?.InvokeEach(t, Response<RawBuildingDock[]>(m), HandlerError);
                        break;

                    case "api_req_kousyou/createitem":
                        var createItem = Response<EquipmentCreationJson>(m);
                        EquipmentCreated?.InvokeEach(t, ParseEquipmentCreation(Request(m), createItem), HandlerError);
                        MaterialsUpdated?.InvokeEach(t, createItem, HandlerError);
                        break;
                    case "api_req_kousyou/destroyship":
                        ShipDismantled?.InvokeEach(t, ParseShipDismantling(Request(m)), HandlerError);
                        MaterialsUpdated?.InvokeEach(t, Response<ShipDismantleJson>(m), HandlerError);
                        break;
                    case "api_req_kousyou/destroyitem2":
                        EquipmentDismantled?.InvokeEach(t, ParseEquipmentDimantling(Request(m)), HandlerError);
                        MaterialsUpdated?.InvokeEach(t, Response<EquipmentDismantleJson>(m), HandlerError);
                        break;
                    case "api_req_kousyou/remodel_slot":
                        var improveItem = Response<EquipmentImproveJson>(m);
                        EquipmentImproved?.InvokeEach(t, ParseEquipmentImprove(Request(m), improveItem), HandlerError);
                        MaterialsUpdated?.InvokeEach(t, improveItem, HandlerError);
                        break;
                    case "api_req_kaisou/powerup":
                        var powerup = Response<ShipPowerupJson>(m);
                        FleetsUpdated?.InvokeEach(t, powerup.api_deck, HandlerError);
                        ShipPoweruped?.InvokeEach(t, ParseShipPowerup(Request(m), powerup), HandlerError);
                        break;
                    case "api_get_member/deck":
                        FleetsUpdated?.InvokeEach(t, Response<RawFleet[]>(m), HandlerError);
                        break;

                    case "api_get_member/questlist":
                        QuestUpdated?.InvokeEach(t, ParseQuestPage(Response<QuestPageJson>(m)), HandlerError);
                        break;
                    case "api_req_quest/clearitemget":
                        QuestCompleted?.InvokeEach(t, ParseQuestComplete(Request(m)), HandlerError);
                        break;

                    case "api_get_member/mapinfo":
                        var mapinfo = Response<MapsJson>(m);
                        MapsUpdated?.InvokeEach(t, mapinfo.api_map_info, HandlerError);
                        AirForceUpdated?.InvokeEach(t, mapinfo.api_air_base, HandlerError);
                        break;
                    case "api_req_air_corps/set_plane":
                        var setPlane = Response<AirForceSetPlaneJson>(m);
                        AirForcePlaneSet?.InvokeEach(t, ParseAirForcePlaneSet(Request(m), setPlane), HandlerError);
                        if (setPlane.api_after_bauxite != null)
                            MaterialsUpdated?.InvokeEach(t, setPlane, HandlerError);
                        break;
                    case "api_req_air_corps/set_action":
                        AirForceActionSet?.InvokeEach(t, ParseAirForceActionSet(Request(m)), HandlerError);
                        break;
                    case "api_req_air_corps/expand_base":
                        AirForceExpanded?.InvokeEach(t, Response<RawAirForceGroup>(m), HandlerError);
                        break;
                    case "api_req_air_corps/supply":
                        var airSupply = Response<AirForceSupplyJson>(m);
                        AirForceSupplied?.InvokeEach(t, ParseAirForceSupply(Request(m), airSupply), HandlerError);
                        MaterialsUpdated?.InvokeEach(t, airSupply, HandlerError);
                        break;

                    case "api_req_member/get_practice_enemyinfo":
                        ExerciseCandidateSelected?.InvokeEach(t, Response<RawExerciseCandidate>(m), HandlerError);
                        break;
                    case "api_req_map/start":
                        SortieStarting?.InvokeEach(t, ParseSortieStart(Request(m)), HandlerError);
                        goto case "api_req_map/next";
                    case "api_req_map/next":
                        var routing = Response<RawMapRouting>(m);
                        HandleMapRouting(routing);
                        MapRouting?.InvokeEach(t, routing, HandlerError);
                        if (routing.api_m1)
                            MapPartUnlocked?.InvokeEach(t, default, HandlerError);
                        break;
                    case "api_req_practice/battle":
                        ExerciseStarted?.InvokeEach(t, ParseExerciseStart(Request(m)), HandlerError);
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
                        BattleStarted?.InvokeEach(t, ParseBattleDetail(m), HandlerError);
                        break;
                    case "api_req_practice/midnight_battle":
                    case "api_req_battle_midnight/battle":
                    case "api_req_combined_battle/midnight_battle":
                    case "api_req_combined_battle/ec_midnight_battle":
                        BattleAppended?.InvokeEach(t, ParseBattleDetail(m), HandlerError);
                        break;
                    case "api_req_sortie/battleresult":
                    case "api_req_practice/battleresult":
                    case "api_req_combined_battle/battleresult":
                        var battleResult = Response<RawBattleResult>(m);
                        BattleCompleted?.InvokeEach(t, battleResult, HandlerError);
                        if (battleResult.api_m1)
                            MapPartUnlocked?.InvokeEach(t, default, HandlerError);
                        break;
                }
            AnyRequested?.InvokeEach(t, new ParsedMessage<JToken>(m.Key, Request(m), Convert<SvData<JToken>>(m.Response)), HandlerError);
        }

        public GameProvider(IHttpProviderSelector providerSelector, ISettingsManager settings = null)
        {
            if (settings != null)
                savedMasterData = settings.Register<string>("game.master_data");
            jSerializer.Error += JsonError;
            providerSelector.Current.Received += HandleMessage;
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            SerialzationError?.Invoke(new SerializationError(e.ErrorContext.Error, e.ErrorContext.Path));
            e.ErrorContext.Handled = true;
        }

        public event TimedMessageHandler<ParsedMessage<JToken>> AnyRequested;
        public event Action<SerializationError> SerialzationError;
        public event TimedMessageHandler<Events.GameError> GameError;
        public event TimedMessageHandler<Exception> HandlerError;
    }
}
