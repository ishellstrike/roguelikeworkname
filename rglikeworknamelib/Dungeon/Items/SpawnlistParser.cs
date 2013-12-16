using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace rglikeworknamelib.Dungeon.Items {
    public class SpawnlistParser {
        public static List<KeyValuePair<string, DropGroup>> Parser(string dataString) {
            var temp = new List<KeyValuePair<string, DropGroup>>();

            dataString = Regex.Replace(dataString, "//.*", "");

            string[] blocks = dataString.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    var parts = block.Split(',');
                    var it = new DropGroup();
                    var ids =  parts[1].Split(' ').ToList();
                    for (int i = 0; i < ids.Count; i++) {
                        var id = ids[i];
                        if (id.StartsWith("spawn_")) {
                            ids.Remove(id);
                            ids.AddRange(ItemDataBase.GetBySpawnGroup(id).Select(itemData => itemData.Key));
                        }
                    }
                    it.Ids = ids.ToArray();
                    it.MinCount = int.Parse(parts[2]);
                    it.MaxCount = int.Parse(parts[3]);
                    it.Prob = int.Parse(parts[4]);
                    it.Repeat = int.Parse(parts[5]);
                    temp.Add(new KeyValuePair<string, DropGroup>(parts[0], it));
                }
                
            }
            return temp;
        }
    }
}