using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemDataBase
    {
        public Dictionary<int, ItemData> data;
        public Collection<Texture2D> texatlas;

        public ItemDataBase(Collection<Texture2D> texatlas_) {
            texatlas = texatlas_;
            data = new Dictionary<int, ItemData>();
            var a = ParsersCore.ParseDirectory<KeyValuePair<int, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetItemDataDirectory(), ItemParser.Parser);
            foreach (var pair in a) {
                data.Add(pair.Key, (ItemData)pair.Value);
            }
        }
    }
}