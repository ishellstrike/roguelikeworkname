using System;
using System.Collections.Generic;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Generation.Names {
    public class NameDataBase {
        public static List<NameData> data = new List<NameData>();


        public NameDataBase() {
            List<object> temp = ParsersCore.UniversalParseDirectory(Settings.GetNamesDataDirectory(),
                                                                    UniversalParser.NoIdParser<NameData>);

            foreach (object o in temp) {
                data.Add((NameData) o);
            }
        }

        public static string GetRandom(Random rnd) {
            return data[rnd.Next(0, data.Count + 1)].Name;
        }
    }
}