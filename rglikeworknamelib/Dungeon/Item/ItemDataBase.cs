using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemDataBase
    {
        public Dictionary<int, ItemData> data;
       // public Collection<Texture2D> texatlas;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public ItemDataBase() {
            //texatlas = texatlas_;
            data = new Dictionary<int, ItemData>();
            var a = ParsersCore.ParseDirectory<KeyValuePair<int, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetItemDataDirectory(), ItemParser.Parser);
            foreach (var pair in a) {
                data.Add(pair.Key, (ItemData)pair.Value);
            }
        }

        public string GetItemDescription(int id) {
            return data[id].description;
        }

        public string GetItemFullDescription(int id) {
            var item = data[id];
            StringBuilder sb = new StringBuilder();
            sb.Append(GetItemDescription(id));
            sb.Append(Environment.NewLine+string.Format("{0} г", item.weight));
            sb.Append(Environment.NewLine+string.Format("{0} места", item.volume));
            if(Settings.DebugInfo) {
                sb.Append(Environment.NewLine + string.Format("id{0}", id));
            }

            if (item.afteruseId != 0) {
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