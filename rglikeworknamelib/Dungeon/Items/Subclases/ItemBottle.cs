using System;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items.Subclases {
    [Serializable]
    public class ItemBottle : Item
    {

        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction { Name = "Открыть бутылку", Action = OpenCan });
        }

        public void OpenCan(Player p)
        {
            p.Inventory.TryRemoveItem(Id, 1);
            p.Inventory.AddItem(ItemFactory.GetInstance(ItemDataBase.Data[Id].AfteruseId, 1));
        }
    }
}