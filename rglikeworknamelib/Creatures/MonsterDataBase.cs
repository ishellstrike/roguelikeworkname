using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Creatures {
    public class MonsterDataBase {
        public Dictionary<int, CreatureData> data;

        public MonsterDataBase() {
            data = new Dictionary<int, CreatureData>();
            //var a = ParsersCore.ParseDirectory<KeyValuePair<int, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetObjectDataDirectory(), BlockParser.Parser);
            //foreach (var pair in a) {
            //    data.Add(pair.Key, (CreatureData)pair.Value);
            //}
        }
    }
}