using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level
{
    public class SchemesDataBase
    {
        public List<Schemes> Data;
        public SchemesDataBase()
        {
            Data = new List<Schemes>();
            var a = ParsersCore.ParseDirectory<object>(Directory.GetCurrentDirectory() + @"\Content\Data\Schemes\", ChemesParser.Parser);
            foreach (var pair in a)
            {
                Data.Add((Schemes)pair);
            }
        }

        public SchemesDataBase(List<Schemes> t)
        {
            Data = t;
        }
    }
}
