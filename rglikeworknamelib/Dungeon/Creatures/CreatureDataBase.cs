using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Creatures {
    public class CreatureDataBase {
        public static Dictionary<string, CreatureData> Data;

        public CreatureDataBase() {
            Data = UniversalParser.JsonDataLoader<CreatureData>(Settings.GetCreatureDataDirectory());
        }
    }
}