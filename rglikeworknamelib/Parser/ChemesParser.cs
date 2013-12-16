using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Parser {
    public class ChemesParser {
        public static List<Schemes> Parser(string dataString) {
            var temp = new List<Schemes>();
            int parseVersion = 0;

            dataString = Regex.Replace(dataString, "//.*", "");
            if (dataString.StartsWith("#version = ")) {
                int a;
                if (int.TryParse(dataString.Substring(11, 1), out a)) {
                    parseVersion = a;
                    dataString = dataString.Remove(0, 14);
                }
            }

            string[] blocks = dataString.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    string[] lines = Regex.Split(block, "\n");
                    string[] header = lines[0].Split(',');

                    var cur = new Schemes();
                    cur.x = Convert.ToInt32(header[0]);
                    cur.y = Convert.ToInt32(header[1]);
                    cur.data = new string[cur.x*cur.y];
                    switch (header[2].Replace("\r", string.Empty)) {
                        case "house":
                            cur.type = SchemeSortType.House;
                            break;
                        case "hospital":
                            cur.type = SchemeSortType.Hospital;
                            break;
                        case "store":
                            cur.type = SchemeSortType.Shop;
                            break;
                        case "wearstore":
                            cur.type = SchemeSortType.WearShop;
                            break;
                        case "b_part_udlr":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_ud":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_lr":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_udl":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_udr":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_ulr":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_dlr":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_ul":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_ur":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_dl":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                        case "b_part_dr":
                            cur.type = SchemeSortType.BunkerParts;
                            break;
                    }
                    cur.filename = string.Format("{0}{1}", cur.type, Settings.rnd.Next());
                    //switch (header[0])
                    //{
                    //    default:
                    //        ((FloorInfo)cur.Value).MnodePrototype = typeof(MNode);
                    //        break;
                    //}
                    switch (parseVersion) {
                        case 0:
                            for (int i = 1; i < lines.Length; i++) {
                                if (lines[i].Length > 1) {
                                    int counter = 0;
                                    string[] aa = lines[i].Split(' ');
                                    foreach (string a in aa) {
                                        cur.data[counter] = a.Trim('\r');
                                        counter++;
                                    }
                                    temp.Add(cur);
                                }
                            }
                            break;
                        case 1:
                            for (int i = 1; i < lines.Length; i++) {
                                if (lines[i].Length > 1) {
                                    int counter = 0;
                                    string[] aa = lines[i].Split(' ');
                                    foreach (string a in aa) {
                                        if (a.StartsWith("!")) {
                                            string[] parts = a.Split('!');
                                            int co = int.Parse(parts[2]);
                                            string id = parts[1].Trim('\r');
                                            for (int j = 0; j < co; j++) {
                                                cur.data[counter] = id;
                                                counter++;
                                            }
                                        }
                                        else {
                                            cur.data[counter] = a.Trim('\r');
                                            counter++;
                                        }
                                    }
                                    temp.Add(cur);
                                }
                            }
                            break;
                    }
                }
            }
            return temp;
        }
    }
}