using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Creatures {
    public class MonsterDataBase {
        public static Dictionary<string, CreatureData> Data;

        public MonsterDataBase()
        {
            Data = new Dictionary<string, CreatureData>();
            var a = ParsersCore.UniversalParseDirectory<KeyValuePair<string, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetCreatureDataDirectory(), UniversalParser.Parser<CreatureData>, typeof(Creature));
            foreach (var pair in a)
            {
                Data.Add(pair.Key, (CreatureData)pair.Value);
            }
        }
    }
}