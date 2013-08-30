using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemData
    {
        public int stackNo;

        public int Weight;
        public string AfteruseId;

        public int hasHealth; //if has health, count became health

        public string Name;
        public string Nameret;
        public string Description;

        public int Doses;

        public int NutH2O, NutCal;

        public string Buff1;
        public string Buff2;
        public string Buff3;
        public string Buff4;
        public string Buff5;

        public string Ammo;
        public int Magazine;

        public int MTex;

        public Microsoft.Xna.Framework.Color MMCol;

        public int Volume;
        public ItemType SType;
    }
}