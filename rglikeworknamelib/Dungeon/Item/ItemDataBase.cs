using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemDataBase
    {
        public Dictionary<string, ItemData> data;
        public Dictionary<string, ItemData> dataMedicineItems;
        public Dictionary<string, ItemData> dataFoodItems;
        private Logger logger = LogManager.GetCurrentClassLogger();
       // public Collection<Texture2D> texatlas;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public ItemDataBase() {
            //texatlas = texatlas_;
            data = new Dictionary<string, ItemData>();
            dataFoodItems = new Dictionary<string, ItemData>();
            dataMedicineItems = new Dictionary<string, ItemData>();
            var a = ParsersCore.ParseDirectory<KeyValuePair<string, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetItemDataDirectory(), ItemParser.Parser);
            foreach (var pair in a) {
                data.Add(pair.Key, (ItemData)pair.Value);
                if (((ItemData)pair.Value).stype == ItemType.Medicine) dataMedicineItems.Add(pair.Key, (ItemData)pair.Value);
                if (((ItemData)pair.Value).stype == ItemType.Food) dataFoodItems.Add(pair.Key, (ItemData)pair.Value);
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

        public Dictionary<string, ItemData> GetItemByItemDatasType(ItemType it) {
            switch (it) {
                case ItemType.Nothing:
                    return data;
                case ItemType.Medicine:
                    return dataMedicineItems;
                case ItemType.Food:
                    return dataFoodItems;
                default:
                    return null;
            }
        }

        public string GetItemDescription(Item i)
        {
            return data[i.Id].description;
        }

        public string GetItemFullDescription(Item i) {
            var item = data[i.Id];
            StringBuilder sb = new StringBuilder();
            sb.Append(GetItemDescription(i));
            sb.Append(Environment.NewLine+string.Format("{0} г", item.weight));
            sb.Append(Environment.NewLine+string.Format("{0} места", item.volume));
            if(Settings.DebugInfo) {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, i.Uid));
            }

            if (item.afteruseId != null) {
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", data[item.afteruseId].name));
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