using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Parser
{
        internal class FloorParser
        {
            public static List<KeyValuePair<int, object>> Parser(string s)
            {
                var temp = new List<KeyValuePair<int, object>>();

                s = Regex.Replace(s, "//.*\n", "");
                s = Regex.Replace(s, "//.*", "");

                string[] blocks = s.Split('~');
                foreach (string block in blocks)
                {
                    if (block.Length != 0)
                    {
                        string[] lines = Regex.Split(block, "\n");
                        string[] header = lines[0].Split(',');

                        temp.Add(new KeyValuePair<int, object>(Convert.ToInt32(header[0]),
                                                               new FloorData(Convert.ToInt32(header[1]))));
                        KeyValuePair<int, object> cur = temp.Last();
                        //switch (header[0])
                        //{
                        //    default:
                        //        ((FloorInfo)cur.Value).MnodePrototype = typeof(MNode);
                        //        break;
                        //}

                        for (int i = 1; i < lines.Length; i++)
                        {
                            if (lines[i].StartsWith("name="))
                            {
                                string extractedstring =
                                    ParsersCore.stringExtractor.Match(lines[i]).ToString();
                                ((FloorData)cur.Value).Name = extractedstring.Substring(1, extractedstring.Length - 2);
                            }
                            if (lines[i].StartsWith("description=")) {
                                string extractedstring =
                                    ParsersCore.stringExtractor.Match(lines[i]).ToString();
                                ((FloorData)cur.Value).Description = extractedstring.Substring(1,
                                                                                                extractedstring.Length - 2);
                            }
                            if (lines[i].StartsWith("mmcol=")) {
                                ((FloorData)cur.Value).MMCol = ParsersCore.ParseStringToColor(lines[i]);
                            }
                            if (lines[i].StartsWith("walkable")) {
                                ((FloorData)cur.Value).Walkable = true;
                            }
                            if (lines[i].StartsWith("walkable")) {
                            }
                        }
                    }
                }
                return temp;
            }
        }
    }
