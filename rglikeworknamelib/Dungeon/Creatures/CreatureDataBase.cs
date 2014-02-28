using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Parser;
using System;
using rglikeworknamelib.Dungeon.Level;
using NLog;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Creatures {
    public class CreatureDataBase {
        public static Dictionary<string, CreatureData> Data;
        public static Dictionary<string, dynamic> Scripts;
        static readonly Logger Logger = LogManager.GetLogger("CreatureDataBase");

        public CreatureDataBase() {
            Data = UniversalParser.JsonDictionaryDataLoader<CreatureData>(Settings.GetCreatureDataDirectory());
        }
    }
}