using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Generation.Names
{
    public enum NameType {
        City,
        Male,
        Fermale,
        Unisex
    }

    public class NameClass {
        public string Name;
        public NameType NameType;
    }
    public class NameDataBase
    {
        public static List<NameClass> data = new List<NameClass>();


        public NameDataBase() {
            var temp = ParsersCore.UniversalParseDirectory(Settings.GetNamesDataDirectory(),
                                                           UniversalParser.NoIdParser<NameClass>);

            foreach (var o in temp) {
                data.Add((NameClass)o);
            }
        }
    }
}
