using System.Collections.Generic;

namespace rglikeworknamelib.Creatures {
    public static class PerkDataBase {
        public static Dictionary<string, PerkData> Perks;

        static PerkDataBase() {
            Perks = new Dictionary<string, PerkData>();
            Perks.Add("huge", new PerkData("��������") {Initial = true, AddRemove = AddRemoveHuge});
            Perks.Add("evil", new PerkData("��������") {Initial = true, AddRemove = AddRemoveEvil});
            Perks.Add("badeye", new PerkData("����������") {Initial = true});
            Perks.Add("veg", new PerkData("������������") {Initial = true});
            Perks.Add("shiz", new PerkData("����������") {Initial = true});
            Perks.Add("para", new PerkData("��������") {Initial = true});
            Perks.Add("intra", new PerkData("����������") {Initial = true});
            Perks.Add("extra", new PerkData("���������") {Initial = true});
            Perks.Add("clever", new PerkData("������������") {Initial = true, AddRemove = AddRemoveClever});
            Perks.Add("tard", new PerkData("��������������") {Initial = true});
            Perks.Add("sport", new PerkData("���������") {Initial = true, AddRemove = AddRemoveAtlat});
            Perks.Add("thief", new PerkData("�����������"));
            Perks.Add("cfobia", new PerkData("������������") {Initial = true});
            Perks.Add("bandit", new PerkData("������ �����������") {Initial = true, AddRemove = AddRemoveBandit});
            Perks.Add("lightph", new PerkData("�����������"));
            Perks.Add("coldb", new PerkData("�������������") {AddRemove = AddRemoveColldb});
            Perks.Add("darkph", new PerkData("������ �������") {Initial = true});
        }

        private static void AddRemoveAtlat(bool arg1, Creature arg2) {
            int ar = arg1 ? 1 : -1;
            arg2.Abilities.list["atlet"].XpLevel += ar*2;
            arg2.Abilities.list["martial"].XpLevel += ar*2;
        }

        private static void AddRemoveColldb(bool arg1, Creature arg2) {
            int ar = arg1 ? 1 : -1;
            arg2.Abilities.list["survive"].XpLevel -= ar*2;
        }

        private static void AddRemoveClever(bool x, Creature owner) {
            int ar = x ? 1 : -1;
            owner.Abilities.list["phys"].XpLevel += ar;
            owner.Abilities.list["bio"].XpLevel += ar;
            owner.Abilities.list["chem"].XpLevel += ar;
        }

        private static void AddRemoveEvil(bool x, Creature owner) {
            int ar = x ? 1 : -1;
            owner.Abilities.list["survive"].XpLevel += ar*2;
        }

        private static void AddRemoveBandit(bool x, Creature owner) {
            int ar = x ? 1 : -1;
            owner.Abilities.list["pickpocket"].XpLevel += ar*2;
            owner.Abilities.list["lockpick"].XpLevel += ar*2;
            owner.Abilities.list["phys"].XpLevel -= ar*2;
            owner.Abilities.list["bio"].XpLevel -= ar*2;
            owner.Abilities.list["chem"].XpLevel -= ar*2;
        }

        private static void AddRemoveHuge(bool x, Creature owner) {
            int ar = x ? 1 : -1;
            owner.Hp = new Stat(owner.Hp.Current, owner.Hp.Max + 50*ar);
        }
    }
}