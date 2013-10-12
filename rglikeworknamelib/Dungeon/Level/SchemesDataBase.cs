using System.Collections.Generic;
using System.IO;
using System.Linq;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level
{
    public class SchemesDataBase
    {
        public static List<Schemes> Data;
        public static List<Schemes> Houses;
        public static List<Schemes> Storages;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public SchemesDataBase()
        {
            Data = new List<Schemes>();
            var a = ParsersCore.ParseDirectory<Schemes>(Directory.GetCurrentDirectory() + @"\Content\Data\Schemes\", ChemesParser.Parser);
            foreach (var pair in a)
            {
                Data.Add((Schemes)pair);
            }

            Houses = Data.Where(x => x.type == SchemesType.House).ToList();
            Storages = Data.Where(x => x.type == SchemesType.Storage).ToList();
        }
    }
}
