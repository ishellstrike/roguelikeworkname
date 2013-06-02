using System.Collections.Generic;
using System.IO;
using System.Linq;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level
{
    public class SchemesDataBase
    {
        public List<Schemes> Data;
        public List<Schemes> Houses;
        public List<Schemes> Storages;
        public SchemesDataBase()
        {
            Data = new List<Schemes>();
            var a = ParsersCore.ParseDirectory<object>(Directory.GetCurrentDirectory() + @"\Content\Data\Schemes\", ChemesParser.Parser);
            foreach (var pair in a)
            {
                Data.Add((Schemes)pair);
            }

            Houses = Data.Where(x => x.type == SchemesType.house).ToList();
            Storages = Data.Where(x => x.type == SchemesType.storage).ToList();
        }

        public SchemesDataBase(List<Schemes> t)
        {
            Data = t;
        }
    }
}
