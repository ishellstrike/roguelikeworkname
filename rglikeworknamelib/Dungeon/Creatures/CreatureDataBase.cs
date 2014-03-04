using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Parser;
using System;
using rglikeworknamelib.Dungeon.Level;
using NLog;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Creatures {
    public class CreatureDataBase {
        public static Dictionary<string, CreatureData> Data;
        static readonly Logger Logger = LogManager.GetLogger("CreatureDataBase");

        public CreatureDataBase() {
            Data = UniversalParser.JsonDictionaryDataLoader<CreatureData>(Settings.GetCreatureDataDirectory());
        }

        private void Nothing(GameTime arg1, GameLevel arg2, Creature arg3) {
            
        }
    }
}