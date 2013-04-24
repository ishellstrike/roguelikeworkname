using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Parser
{
    internal class ItemParser
    {
        public static List<KeyValuePair<int, object>> Parser(string s)
        {
            var temp = new List<KeyValuePair<int, object>>();

            s = Regex.Replace(s, "//.*\n", "");
            s = Regex.Replace(s, "//.*", "");

            string[] blocks = s.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    string[] lines = Regex.Split(block, "\n");
                    string[] header = lines[0].Split(',');

                    temp.Add(new KeyValuePair<int, object>(Convert.ToInt32(header[0]), new ItemData()));
                    KeyValuePair<int, object> cur = temp.Last();
                    //switch (header[0])
                    //{
                    //    default:
                    //        ((FloorInfo)cur.Value).MnodePrototype = typeof(MNode);
                    //        break;
                    //}

                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].StartsWith("name=")) {
                            string extractedstring =
                                ParsersCore.stringExtractor.Match(lines[i]).ToString();
                            ((ItemData)cur.Value).name = extractedstring.Substring(1, extractedstring.Length - 2);
                        }
                        if (lines[i].StartsWith("description=")) {
                            string extractedstring =
                                ParsersCore.stringExtractor.Match(lines[i]).ToString();
                            ((ItemData)cur.Value).description = extractedstring.Substring(1,
                                                                                            extractedstring.Length - 2);
                        }
                        if (lines[i].StartsWith("mmcol=")) {
                            ((ItemData)cur.Value).mmcol = ParsersCore.ParseStringToColor(lines[i]);
                        }
                    }
                }
            }
            return temp;
        }
    }
}
