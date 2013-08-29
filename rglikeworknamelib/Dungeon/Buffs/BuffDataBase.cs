using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Buffs
{
    public class BuffDataBase
    {
        public static Dictionary<string, BuffData> Data;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public BuffDataBase() {
            Data = new Dictionary<string, BuffData>();
            var a = ParsersCore.UniversalParseDirectory<KeyValuePair<string, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetEffectDataDirectory(), UniversalParser.Parser<BuffData>, typeof(Buff));
            foreach (var pair in a)
            {
                Data.Add(pair.Key, (BuffData)pair.Value);
            }
        }
    }
}
