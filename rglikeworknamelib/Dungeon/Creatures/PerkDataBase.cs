using System.Collections.Generic;

namespace rglikeworknamelib.Dungeon.Creatures
{
    public static class PerkDataBase {
        public static Dictionary<string, PerkData> Perks;

        static PerkDataBase() {
            Perks = new Dictionary<string, PerkData> {
                {"huge", new PerkData("Здоровяк") {Initial = true, AddRemove = AddRemoveHuge}},
                {"evil", new PerkData("Жестокий") {Initial = true, AddRemove = AddRemoveEvil}},
                {"badeye", new PerkData("Близорукий") {Initial = true}},
                {"veg", new PerkData("Вегитерианец") {Initial = true}},
                {"shiz", new PerkData("Шизофреник") {Initial = true}},
                {"para", new PerkData("Параноик") {Initial = true}},
                {"intra", new PerkData("Интароверт") {Initial = true}},
                {"extra", new PerkData("Эктраверт") {Initial = true}},
                {"clever", new PerkData("Образованный") {Initial = true, AddRemove = AddRemoveClever}},
                {"tard", new PerkData("Необразованный") {Initial = true}},
                {"sport", new PerkData("Спортсмен") {Initial = true, AddRemove = AddRemoveAtlat}},
                {"thief", new PerkData("Клептомания")},
                {"cfobia", new PerkData("Клаутрофобия") {Initial = true}},
                {"bandit", new PerkData("Бывший заключенный") {Initial = true, AddRemove = AddRemoveBandit}},
                {"lightph", new PerkData("Светобоязнь")},
                {"coldb", new PerkData("Хладнокровный") {AddRemove = AddRemoveColldb}},
                {"darkph", new PerkData("Боязнь темноты") {Initial = true}}
            };
        }

        private static void AddRemoveAtlat(bool arg1, Creature arg2) {
            var ar = arg1 ? 1 : -1;
            arg2.Abilities.List[AbilityType.Atlet].XpLevel += ar*2;
            arg2.Abilities.List[AbilityType.Martial].XpLevel += ar * 2;
        }

        private static void AddRemoveColldb(bool arg1, Creature arg2) {
            var ar = arg1 ? 1 : -1;
            arg2.Abilities.List[AbilityType.Survive].XpLevel -= ar * 2;
        }

        private static void AddRemoveClever(bool x, Creature owner) {
            var ar = x ? 1 : -1;
            owner.Abilities.List[AbilityType.Phys].XpLevel += ar;
            owner.Abilities.List[AbilityType.Bio].XpLevel += ar;
            owner.Abilities.List[AbilityType.Chem].XpLevel += ar;
        }

        private static void AddRemoveEvil(bool x, Creature owner) {
            var ar = x ? 1 : -1;
            owner.Abilities.List[AbilityType.Survive].XpLevel += ar * 2;
        }

        private static void AddRemoveBandit(bool x, Creature owner) {
            var ar = x ? 1 : -1;
            owner.Abilities.List[AbilityType.Pickpocket].XpLevel += ar * 2;
            owner.Abilities.List[AbilityType.Lockpick].XpLevel += ar * 2;
            owner.Abilities.List[AbilityType.Phys].XpLevel -= ar * 2;
            owner.Abilities.List[AbilityType.Bio].XpLevel -= ar * 2;
            owner.Abilities.List[AbilityType.Chem].XpLevel -= ar * 2;
        }

        private static void AddRemoveHuge(bool x, Creature owner) {
            var ar = x ? 1 : -1;
            owner.Hp = new Stat(owner.Hp.Current, owner.Hp.Max + 50*ar);
        }
    }
}