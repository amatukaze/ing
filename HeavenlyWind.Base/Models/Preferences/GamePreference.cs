namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class GamePreference
    {
        public Property<FleetLoSFormula> MainFleetLoSFormula { get; } = new Property<FleetLoSFormula>("game.formula.los", FleetLoSFormula.Formula33);

        public Property<FleetFighterPowerFormula> MainFighterPowerFormula { get; } = new Property<FleetFighterPowerFormula>("game.formula.fighter_power", FleetFighterPowerFormula.WithBonus);

        public Property<int> FatigueCeiling { get; } = new Property<int>("game.fatigue_ceiling", 40);

        public Property<bool> ShowBattleInfo { get; } = new Property<bool>("game.sortie.show_battle_info", true);

        public Property<bool> ShowDrop { get; } = new Property<bool>("game.sortie.show_drop", true);

        public Property<int[]> SelectedShipTypes { get; } = new Property<int[]>("game.overview.ship_types");
        public Property<int[]> SelectedEquipmentTypes { get; } = new Property<int[]>("game.overview.equipment_types");

        public Property<bool> ShipOverview_ExceptExpeditionShips { get; } = new Property<bool>("game.overview.ship.filter.except.expedition");
        public Property<bool> ShipOverview_ExceptSparklingShips { get; } = new Property<bool>("game.overview.ship.filter.except.sparkling");
        public Property<bool> ShipOverview_ExceptLevel1Ships { get; } = new Property<bool>("game.overview.ship.filter.except.lv1");
        public Property<bool> ShipOverview_ExceptMaxModernizationShips { get; } = new Property<bool>("game.overview.ship.filter.except.maxmodernization");
    }
}
