using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Parser
{
    public static class UnitParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static List<KeyValuePair<string, object>> Parser(string s)
        {
            var temp = new List<KeyValuePair<string, object>>();

            s = s.Remove(0, s.IndexOf('~'));

            s = Regex.Replace(s, "//.*\r\n", "");
            s = Regex.Replace(s, "//.*", "");

            string[] blocks = s.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    string[] lines = Regex.Split(block, "\n");
                    string[] header = lines[0].Split(',');

                    temp.Add(new KeyValuePair<string, object>(header[0], new CreatureData()));
                    KeyValuePair<string, object> cur = temp.Last();

                    ((CreatureData) cur.Value).MTex = header[1].Trim('\r');

                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].Contains('=')) {
                            string sstart = lines[i].Substring(0, lines[i].IndexOf('='));
                            var finfo = typeof (CreatureData).GetField(sstart);
                            var extracted = lines[i].Substring(lines[i].IndexOf('=') + 1,
                                                               lines[i].Length - (lines[i].IndexOf('=') + 1) - 1).Replace("\"","");

                            if (finfo != null) {
                                var converter = TypeDescriptor.GetConverter(finfo.FieldType);
                                if (converter != null) {
                                    var converted = converter.ConvertFromString(extracted);
                                    finfo.SetValue(cur.Value, converted);
                                }
                            } else {
                                logger.Error(cur.Value.GetType().Name+" didnt contains field \""+sstart+"\", \""+extracted+"\" didn't assigned to ID "+header[0]);
                            }
                        }
                    }
                }
            }
            return temp;
        }
    }
}
