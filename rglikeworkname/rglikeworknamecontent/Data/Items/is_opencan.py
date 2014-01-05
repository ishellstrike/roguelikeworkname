from rglikeworknamelib.Dungeon.Items import Item
from rglikeworknamelib.Dungeon.Items import ItemFactory
from rglikeworknamelib.Dungeon.Items import ItemDataBase

# -*- coding: utf-8 -*-

def ItemScript(p, target):
    if p.Inventory.ContainsId("knife") or p.Inventory.ContainsId("otvertka"):
        p.Inventory.TryRemoveItem(target.Id, 1)
        p.Inventory.AddItem(ItemFactory.GetInstance(ItemDataBase.Data[target.Id].AfteruseId, 1))
    else:
        Item.Log("Чтобы открывать банки вам нужен нож или отвертка");
def Name():
    return "Открыть банку"