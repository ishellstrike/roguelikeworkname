using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemDataBase
    {
        public static Dictionary<string, ItemData> Data;
        public static Dictionary<string, ItemData> DataMedicineItems;
        public static Dictionary<string, ItemData> DataFoodItems;
        private static Logger logger_ = LogManager.GetCurrentClassLogger();
       // public Collection<Texture2D> texatlas;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public ItemDataBase() {
            //texatlas = texatlas_;
            Data = new Dictionary<string, ItemData>();
            DataFoodItems = new Dictionary<string, ItemData>();
            DataMedicineItems = new Dictionary<string, ItemData>();
            var a = ParsersCore.UniversalParseDirectory(Settings.GetItemDataDirectory(), UniversalParser.Parser<ItemData>);
            foreach (var pair in a) {
                Data.Add(pair.Key, (ItemData)pair.Value);
                if (((ItemData)pair.Value).SType == ItemType.Medicine) DataMedicineItems.Add(pair.Key, (ItemData)pair.Value);
                if (((ItemData)pair.Value).SType == ItemType.Food) DataFoodItems.Add(pair.Key, (ItemData)pair.Value);
            }
        }

        private static void JsonEyeCandy(List<KeyValuePair<int, object>> a) {
            MemoryStream sw = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (ItemData));
            foreach (var pair in a) {
                ser.WriteObject(sw, pair.Value as ItemData);
            }

            sw.Position = 0;
            StreamReader re = new StreamReader(sw);
            string jdata = re.ReadToEnd();

            jdata = jdata.Insert(1, Environment.NewLine);
            jdata = jdata.Replace("}{", Environment.NewLine + "}" + Environment.NewLine + "{" + Environment.NewLine);
            jdata = jdata.Replace(":", " : ");

            Regex reg1 = new Regex("({\n)?\"[a-zA-Z]+\".+?(},|,|\n}|})");
            var mac = reg1.Matches(jdata);

            StringBuilder sb = new StringBuilder();

            foreach (var vari in mac) {
                sb.Append(" " + vari + Environment.NewLine);
            }

            StreamWriter writer =
                new StreamWriter(Directory.GetCurrentDirectory() + @"/" + Settings.GetItemDataDirectory() + @"/items.json",
                                 false);
            writer.Write(sb);
            writer.Flush();
            writer.Close();
            sw.Close();
            re.Close();
        }

        public static Dictionary<string, ItemData> GetItemByItemDatasType(ItemType it) {
            switch (it) {
                case ItemType.Nothing:
                    return Data;
                case ItemType.Medicine:
                    return DataMedicineItems;
                case ItemType.Food:
                    return DataFoodItems;
                default:
                    return null;
            }
        }

        public static string GetItemDescription(Item.Item i)
        {
            return i.Data.Description;
        }

        public static string GetItemFullDescription(Item.Item i) {
            var item = Data[i.Id];
            StringBuilder sb = new StringBuilder();
            sb.Append(item.Name);
            if (i.Data.Description != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + i.Data.Description);
            }
            sb.Append(Environment.NewLine + string.Format("{0} г", item.Weight));
            sb.Append(Environment.NewLine + string.Format("{0} места", item.Volume));
            if(Settings.DebugInfo) {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, i.Uid));
            }

            if (item.AfteruseId != null) {
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", Data[item.AfteruseId].Name));
            }

            if(item.Buff != null) {
                sb.Append(Environment.NewLine +Environment.NewLine+ string.Format("Эффекты :"));

                foreach (var buff in item.Buff) {
                    sb.Append(Environment.NewLine + string.Format("{0}", BuffDataBase.Data[buff].Name));
                }
            }
            if (item.Ammo != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Калибр :"));

                sb.Append(Environment.NewLine + Data[item.Ammo].Name);
            }
            //switch (item.stype) {
            //    case ItemType.Medicine:
            //        sb.Append(item.)
            //        break;
            //}
            return sb.ToString();
        }
    }
}