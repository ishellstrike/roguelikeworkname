using System.Collections.Generic;

namespace rglikeworknamelib.Dungeon.Creatures
{
    public static class PerkDataBase {
        public static Dictionary<string, PerkData> Perks;

        static PerkDataBase() {
            Perks = new Dictionary<string, PerkData> {
                {"huge", new PerkData("��������") {Initial = true, AddRemove = AddRemoveHuge}},
                {"evil", new PerkData("��������") {Initial = true, AddRemove = AddRemoveEvil}},
                {"badeye", new PerkData("����������") {Initial = true}},
                {"veg", new PerkData("������������") {Initial = true}},
                {"shiz", new PerkData("����������") {Initial = true}},
                {"para", new PerkData("��������") {Initial = true}},
                {"intra", new PerkData("����������") {Initial = true}},
                {"extra", new PerkData("���������") {Initial = true}},
                {"clever", new PerkData("������������") {Initial = true, AddRemove = AddRemoveClever}},
                {"tard", new PerkData("��������������") {Initial = true}},
                {"sport", new PerkData("���������") {Initial = true, AddRemove = AddRemoveAtlat}},
                {"thief", new PerkData("�����������")},
                {"cfobia", new PerkData("������������") {Initial = true}},
                {"bandit", new PerkData("������ �����������") {Initial = true, AddRemove = AddRemoveBandit}},
                {"lightph", new PerkData("�����������")},
                {"coldb", new PerkData("�������������") {AddRemove = AddRemoveColldb}},
                {"darkph", new PerkData("������ �������") {Initial = true}}
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