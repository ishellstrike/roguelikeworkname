using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCookableFood : Item, ICookable {
        public ItemCookableFood()
        {
            CookProgress = 0;
        }

        public double CookProgress { get; private set; }
        public void GiveHeat(double value, Player p, Vector3 secpos) {
            CookProgress += value;
            if (CookProgress >= 100)
            {
                CookProgress = 0;
                switch (Modifer)
                {
                    case ItemModifer.Razogretyi:
                        Modifer = ItemModifer.Prigotovlenniy;
                        EventLog.AddLocated("вы чувствуете запах приготовленной пищи", p, secpos);
                        break;
                    case ItemModifer.Prigotovlenniy:
                        Modifer = ItemModifer.Perejareniy;
                        break;
                    case ItemModifer.Perejareniy:
                        Modifer = ItemModifer.Obuglivshiysa;
                        EventLog.AddLocated("вы чуствуете запах дыма", p, secpos);
                        break;
                    case ItemModifer.Nothing:
                        Modifer = ItemModifer.Razogretyi;
                        break;
                }
            }
        }
    }
}