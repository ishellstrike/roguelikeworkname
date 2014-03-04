using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Registry
{
    public class Registry : Singleton<Registry> {
        public SneakyDictionary<string, NewBlockData> Blocks;
        public SneakyDictionary<string, Floor> Floors;
        public SneakyDictionary<string, Item> Items;
        public List<Schemes> Schemes;
        public List<ItemCraftData> Crafts;
        
        public void RegisterBlock(Block block) {
            
        }
    }
}
