# -*- coding: utf-8 -*-

from rglikeworknamelib.Dungeon.Items import Item
from rglikeworknamelib.Dungeon.Items import InventorySystem
from rglikeworknamelib.Dungeon.Items import ItemFactory
from rglikeworknamelib.Dungeon.Items import ItemDataBase

def ItemScript(p, target):
    if p.Inventory.ContainsId("knife") or p.Inventory.ContainsId("otvertka"):
        p.Inventory.TryRemoveItem(target.Id, 1)
        if ItemDataBase.Instance.Data[target.Id].AfteruseId is not None:
            item = ItemDataBase.Instance.Data[target.Id].AfteruseId
            p.Inventory.AddItem(ItemFactory.GetInstance(item, 1))
    else:
        Item.Log("Чтобы открывать банки вам нужен нож или отвертка");
def Name():
    return "Открыть банку"
