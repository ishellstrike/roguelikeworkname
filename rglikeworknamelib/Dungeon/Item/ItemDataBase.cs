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