using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemData
    {
        public int stackNo;

        public int weight;
        public string afteruseId;

        public int hasHealth; //if has health, count became health

        public string Name;
        public string Nameret;
        public string description;

        public string Buff1;
        public string Buff2;
        public string Buff3;
        public string Buff4;
        public string Buff5;

        public int mtex;

        public Microsoft.Xna.Framework.Color mmcol;

        public int volume;
        public ItemType stype;
    }
}