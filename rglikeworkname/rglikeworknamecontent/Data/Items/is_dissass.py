# -*- coding: utf-8 -*-

from rglikeworknamelib.Dungeon.Items import Item
from rglikeworknamelib.Dungeon.Items import InventorySystem
from rglikeworknamelib.Dungeon.Items import ItemFactory
from rglikeworknamelib.Dungeon.Items import ItemDataBase

def ItemScript(p, target, rnd):
    if p.Inventory.ContainsId("otvertka"):
        p.Inventory.TryRemoveItem(target.Id, 1);
        p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
        p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
        p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", rnd.Next(5) + 10));

        Item.Log("Вы успешно разбираете {0}", target.Data.Name)
    else:
        Item.Log("Чтобы разбирать электронику вам нужна отвертка");
def Name():
    return "Разобрать радио"
