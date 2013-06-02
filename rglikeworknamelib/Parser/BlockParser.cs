using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Parser
{
    internal class BlockParser
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

                    temp.Add(new KeyValuePair<int, object>(Convert.ToInt32(header[1]), new BlockData(Convert.ToInt32(header[2]))));
                    KeyValuePair<int, object> cur = temp.Last();
                    switch (header[0]) {
                        case "StorageBlock":
                            ((BlockData)cur.Value).BlockPrototype = typeof(StorageBlock);
                            break;
                        default:
                            ((BlockData)cur.Value).BlockPrototype = typeof(Block);
                            break;
                    }

                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].StartsWith("name=")) {
                            string extractedstring =
                                ParsersCore.stringExtractor.Match(lines[i]).ToString();
                            ((BlockData)cur.Value).Name = extractedstring.Substring(1, extractedstring.Length - 2);
                        }
                        if (lines[i].StartsWith("afterid=")) {
                            string extractedstring =
                                ParsersCore.intextractor.Match(lines[i]).ToString();
                            ((BlockData)cur.Value).AfterDeathId = Convert.ToInt32(extractedstring);
                        }
                        if (lines[i].StartsWith("description=")) {
                            string extractedstring =
                                ParsersCore.stringExtractor.Match(lines[i]).ToString();
                            ((BlockData)cur.Value).Description = extractedstring.Substring(1,
                                                                                            extractedstring.Length - 2);
                        }
                        if (lines[i].StartsWith("mmcol=")) {
                            ((BlockData)cur.Value).MMCol = ParsersCore.ParseStringToColor(lines[i]);
                        }
                        if (lines[i].StartsWith("walkable")) {
                            ((BlockData)cur.Value).IsWalkable = true;
                        }
                        if (lines[i].StartsWith("transparent")) {
                            ((BlockData)cur.Value).IsTransparent = true;
                        }
                        if (lines[i].StartsWith("actionopencontainer")) {
                            ((BlockData)cur.Value).SmartAction = SmartAction.ActionOpenContainer;
                        }
                        if (lines[i].StartsWith("actionloot"))
                        {
                            ((BlockData)cur.Value).SmartAction = SmartAction.ActionLoot;
                        }
                        if (lines[i].StartsWith("actionsmash"))
                        {
                            ((BlockData)cur.Value).SmartAction = SmartAction.ActionSmash;
                        }
                        if (lines[i].StartsWith("actionopenclose")) {
                            ((BlockData)cur.Value).SmartAction = SmartAction.ActionOpenClose;
                        }
                        if (lines[i].StartsWith("altermtex=")) {
                            string sub = lines[i].Substring(10);
                            string[] subsub = sub.Replace(" ", "").Split(',');
                            List<int> mt = new List<int>();
                            foreach (var s1 in subsub) {
                                int a;
                                if (int.TryParse(s1, out a)) {
                                    mt.Add(a);
                                }
                            }
                            ((BlockData)cur.Value).AlterMtex = mt.ToArray();
                        }
                    }
                }
            }
            return temp;
        }
    }
}
