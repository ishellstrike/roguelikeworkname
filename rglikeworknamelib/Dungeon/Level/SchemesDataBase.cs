using System.Collections.Generic;
using System.IO;
using System.Linq;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class SchemesDataBase {
        public static List<Schemes> Data;
        public static List<Schemes> Houses;
        public static List<Schemes> Storages;
        public static List<Schemes> NormalCity; 

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public SchemesDataBase() {
            Data = new List<Schemes>();
            Data = UniversalParser.JsonListDataLoaderNames<Schemes>(Directory.GetCurrentDirectory() + @"\Content\Data\Schemes\");

            Houses = Data.Where(x => x.type == SectorBiom.House).ToList();
            Storages = Data.Where(x => x.type == SectorBiom.Storage).ToList();
            NormalCity =
                Data.Where(
                    x => x.type == SectorBiom.House || x.type == SectorBiom.Shop || x.type == SectorBiom.FoodShop || x.type == SectorBiom.Fastfood || x.type == SectorBiom.Hospital || x.type == SectorBiom.WearShop).
                    ToList();
        }
    }
}