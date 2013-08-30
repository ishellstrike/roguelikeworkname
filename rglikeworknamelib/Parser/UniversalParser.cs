using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NLog;

namespace rglikeworknamelib.Parser {
    internal class UniversalParser {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Universal jarg data file parser
        /// </summary>
        /// <param name="s">Readed data file</param>
        /// <param name="basetype">Basic type of parsing objects (for Prototype Parser) (e.g. typeof(Block) (null if you have no Prototype in parsed data))</param>
        /// <returns>List of data of type KeyValuePair&lt;string (id), object (parseof type)&gt;</returns>
        public static List<KeyValuePair<string, object>> Parser<T>(string s, string filePos, Type basetype = null)
        {
            var temp = new List<KeyValuePair<string, object>>();

            s = s.Remove(0, s.IndexOf('~'));

            s = Regex.Replace(s, "//.*\r\n", "");
            s = Regex.Replace(s, "//.*", "");

            string[] blocks = s.Split('~');
            foreach (string block in blocks)
            {
                if (block.Length != 0)
                {
                    string[] lines = Regex.Split(block, "\n").Select(x=>x.Trim('\r')).ToArray();
                    string[] header = lines[0].Split(',');

                    temp.Add(new KeyValuePair<string, object>(header[1], Activator.CreateInstance(typeof(T))));
                    KeyValuePair<string, object> cur = temp.Last();

                    if (basetype != null) { //Prototype parser
                        string prototypeNamespace = basetype.ToString();
                        prototypeNamespace = prototypeNamespace.Substring(0, prototypeNamespace.LastIndexOf('.')+1);
                        if (prototypeNamespace != "") {

                            Type type = Type.GetType(prototypeNamespace + header[0]); //create base type namespace (e.g. "rglikeworknamelib.Dungeon.Buffs.")
                            if (type == null) {
                                logger.Error(string.Format("Subclass of {2} \"{0}\" for {1} cannot be created",
                                                           prototypeNamespace + header[0], header[1], basetype));
                                type = basetype; //prototype creation error, create base type
                            }

                            var blockPrototypeField = typeof(T).GetField("Prototype");
                            if (blockPrototypeField != null)
                            {
                                blockPrototypeField.SetValue(cur.Value, type);
                            }
                            else
                            {
                                logger.Error(cur.Value.GetType().Name +
                                             " didnt contains Prototype field! Prototype " + type +
                                             " for ID " +
                                             header[0] + " didn't created");
                            }
                        }
                    }

                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Contains('='))
                        {
                            string sstart = lines[i].Substring(0, lines[i].IndexOf('='));
                            FieldInfo finfo = typeof(T).GetField(sstart);
                            string extracted = lines[i].Substring(lines[i].IndexOf('=') + 1,
                                                                  lines[i].Length - (lines[i].IndexOf('=') + 1)).Replace("\"", "");

                            if (finfo != null)
                            {
                                var converter = TypeDescriptor.GetConverter(finfo.FieldType);
                                if (extracted.StartsWith("{")) //Array parser
                                {
                                    extracted = extracted.Substring(1, extracted.Length - 2);
                                    var arrayextractor = extracted.Split(',').Select(x => x.Trim(' '));

                                    foreach (var arrextr in arrayextractor)
                                    {

                                    }
                                }
                                else
                                {
                                    if (converter != null)
                                    {
                                        var converted = converter.ConvertFromString(extracted);
                                        finfo.SetValue(cur.Value, converted);
                                    }
                                    else
                                    {
                                        logger.Error("Could't convert \""+extracted+"\" to "+finfo+" to ID "+header[0]);
                                    }
                                }
                            }
                            else
                            {
                                logger.Error(cur.Value.GetType().Name + " didnt contains field \"" + sstart + "\", \"" + extracted + "\" didn't assigned to ID " + header[0]);
                            }
                        }
                    }
                }
            }
            logger.Info("{0} entities for {1} loaded from {2}", temp.Count, typeof(T).ToString(), filePos);
            return temp;
        }
    }
}