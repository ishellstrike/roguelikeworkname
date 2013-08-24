using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Parser
{
    internal class BlockParser
    {
        //public static List<KeyValuePair<string, object>> Parser(string s)
        //{
        //    var temp = new List<KeyValuePair<string, object>>();

        //    s = s.Remove(0, s.IndexOf('~'));

        //    s = Regex.Replace(s, "//.*\n", "");
        //    s = Regex.Replace(s, "//.*", "");

        //    string[] blocks = s.Split('~');
        //    foreach (string block in blocks) {
        //        if (block.Length != 0) {
        //            string[] lines = Regex.Split(block, "\n");
        //            string[] header = lines[0].Split(',');

        //            temp.Add(new KeyValuePair<string, object>(header[1], new BlockData(header[2].Trim('\r'))));
        //            KeyValuePair<string, object> cur = temp.Last();
        //            switch (header[0]) {
        //                case "StorageBlock":
        //                    ((BlockData)cur.Value).BlockPrototype = typeof(StorageBlock);
        //                    break;
        //                default:
        //                    ((BlockData)cur.Value).BlockPrototype = typeof(Block);
        //                    break;
        //            }

        //            for (int i = 1; i < lines.Length; i++) {
        //                if (lines[i].StartsWith("name=")) {
        //                    string extractedstring =
        //                        ParsersCore.stringExtractor.Match(lines[i]).ToString();
        //                    ((BlockData)cur.Value).Name = extractedstring.Substring(1, extractedstring.Length - 2);
        //                }
        //                if (lines[i].StartsWith("afterid=")) {
        //                    string extractedstring =
        //                        ParsersCore.intextractor.Match(lines[i]).ToString();
        //                    ((BlockData)cur.Value).AfterDeathId = extractedstring;
        //                }
        //                if (lines[i].StartsWith("swide="))
        //                {
        //                    string extractedstring =
        //                        ParsersCore.intextractor.Match(lines[i]).ToString();
        //                    ((BlockData)cur.Value).swide = int.Parse(extractedstring);
        //                }
        //                if (lines[i].StartsWith("description=")) {
        //                    string extractedstring =
        //                        ParsersCore.stringExtractor.Match(lines[i]).ToString();
        //                    ((BlockData)cur.Value).Description = extractedstring.Substring(1,
        //                                                                                    extractedstring.Length - 2);
        //                }
        //                if (lines[i].StartsWith("mmcol=")) {
        //                    ((BlockData)cur.Value).MMCol = ParsersCore.ParseStringToColor(lines[i]);
        //                }
        //                if (lines[i].StartsWith("walkable")) {
        //                    ((BlockData)cur.Value).IsWalkable = true;
        //                }
        //                if (lines[i].StartsWith("transparent")) {
        //                    ((BlockData)cur.Value).IsTransparent = true;
        //                }
        //                if (lines[i].StartsWith("actionopencontainer")) {
        //                    ((BlockData)cur.Value).SmartAction = SmartAction.ActionOpenContainer;
        //                }
        //                if (lines[i].StartsWith("actionloot"))
        //                {
        //                    ((BlockData)cur.Value).SmartAction = SmartAction.ActionLoot;
        //                }
        //                if (lines[i].StartsWith("actionsmash"))
        //                {
        //                    ((BlockData)cur.Value).SmartAction = SmartAction.ActionSmash;
        //                }
        //                if (lines[i].StartsWith("actionopenclose")) {
        //                    ((BlockData)cur.Value).SmartAction = SmartAction.ActionOpenClose;
        //                }
        //                if (lines[i].StartsWith("altermtex=")) {
        //                    string sub = lines[i].Substring(10);
        //                    string[] subsub = sub.Replace(" ", "").Split(',');
        //                    List<string> mt = new List<string>();
        //                    foreach (var s1 in subsub) {
        //                        mt.Add(s1.Trim('\r'));
        //                    }
        //                    ((BlockData)cur.Value).AlterMtex = mt.ToArray();
        //                }
        //            }
        //        }
        //    }
        //    return temp;
        //}

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

                    Type type = Type.GetType("rglikeworknamelib.Dungeon.Level.Blocks."+header[0]);
                    temp.Add(new KeyValuePair<string, object>(header[1].Trim('\r'), new BlockData(header[2].Trim('\r'))));
                    if (type == null) {
                        logger.Error(string.Format("Subclass of Block \"{0}\" for {1} cannot be created", "rglikeworknamelib.Dungeon.Level.Blocks." + header[0], header[1]));
                        type = typeof (Block);
                    }
                    KeyValuePair<string, object> cur = temp.Last();
                    ((BlockData) cur.Value).BlockPrototype = type;

                    try {
                        for (int i = 1; i < lines.Length; i++) {
                            if (lines[i].Contains('=')) {
                                string sstart = lines[i].Substring(0, lines[i].IndexOf('='));
                                var extracted = lines[i].Substring(lines[i].IndexOf('=') + 1,
                                                                   lines[i].Length - (lines[i].IndexOf('=') + 1) - 1).
                                    Replace("\"", "");
                                FieldInfo finfo = typeof (BlockData).GetField(sstart);
                                if (finfo != null) {
                                    var converter = TypeDescriptor.GetConverter(finfo.FieldType);
                                    if (converter != null) {
                                        Color a;
                                        var converted = converter.ConvertFromString(extracted);
                                        finfo.SetValue(cur.Value, converted);
                                    }
                                }
                                else {
                                    logger.Error(cur.Value.GetType().Name + " didnt contains field \"" + sstart +
                                                 "\", \"" + extracted + "\" didn't assigned to ID " + header[0]);
                                }
                            }
                        }
                    } catch (Exception e) {
                        logger.ErrorException(e.ToString(), e);
                    }
                }
            }
            return temp;
        }
    }
}
