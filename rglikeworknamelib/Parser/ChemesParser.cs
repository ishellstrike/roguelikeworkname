using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Parser
{
    class ChemesParser
    {
        public static List<object> Parser(string s)
        {
            var temp = new List<object>();

            s = Regex.Replace(s, "//.*", "");

            string[] blocks = s.Split('~');
            foreach (string block in blocks)
            {
                if (block.Length != 0)
                {
                    string[] lines = Regex.Split(block, "\n");
                    string[] header = lines[0].Split(',');

                    var cur = new Schemes();
                    cur.x = Convert.ToInt32(header[0]);
                    cur.y = Convert.ToInt32(header[1]);
                    cur.data = new string[cur.x * cur.y];
                    switch (header[2])
                    {
                        case "house":
                            cur.type = SchemesType.house;
                            break;
                    }
                    //switch (header[0])
                    //{
                    //    default:
                    //        ((FloorInfo)cur.Value).MnodePrototype = typeof(MNode);
                    //        break;
                    //}
                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].Length > 1) {
                            int counter = 0;
                            var aa = lines[i].Split(' ');
                            foreach (var a in aa) {
                                    cur.data[counter] = a.Trim('\r');
                                    counter++;
                            }
                            temp.Add(cur);
                        }
                    }
                }
            }
            return temp;
        }
    }
}
