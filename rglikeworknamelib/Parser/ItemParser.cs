using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Parser
{
    internal class ItemParser
    {
        public static List<KeyValuePair<int, object>> Parser(string s)
        {
            var temp = new List<KeyValuePair<int, object>>();

            s = s.Remove(0, s.IndexOf('~'));

            s = Regex.Replace(s, "//.*\r\n", "");
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
                        if (lines[i].StartsWith("stype=")) {
                            string extractedstring =
                                ParsersCore.stringExtractor.Match(lines[i]).ToString();
                            ((ItemData)cur.Value).stype = extractedstring.Substring(1, extractedstring.Length - 2);
                        }
                        if (lines[i].StartsWith("weight=")) {
                            string extractedstring =
                                ParsersCore.intextractor.Match(lines[i]).ToString();
                            ((ItemData)cur.Value).weight = Convert.ToInt32(extractedstring);
                        }
                        if (lines[i].StartsWith("volume=")) {
                            string extractedstring =
                                ParsersCore.intextractor.Match(lines[i]).ToString();
                            ((ItemData)cur.Value).volume = Convert.ToInt32(extractedstring);
                        } if (lines[i].StartsWith("afteruse=")) {
                            string extractedstring =
                                ParsersCore.intextractor.Match(lines[i]).ToString();
                            ((ItemData)cur.Value).afteruseId = Convert.ToInt32(extractedstring);
                        }
                    }
                }
            }
            return temp;
        }
    }
}
