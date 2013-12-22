using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace rglikeworknamelib.Parser {
    [Serializable]
    public class NoPrototypeException : Exception {
    }

    internal class UniversalParser {
        private static readonly Logger logger = LogManager.GetLogger("UniversalParser");

        /// <summary>
        ///     Universal jarg data file parser
        /// </summary>
        /// <param name="dataString">Readed data file</param>
        /// <param name="filePos">way to data file (for log)</param>
        /// <param name="basetype">Basic type of parsing objects (for Prototype Parser) (e.g. typeof(Block) (null if you have no Prototype in parsed data))</param>
        /// <returns>List of data of type KeyValuePair&lt;string (id), object (parseof type)&gt;</returns>
        public static List<KeyValuePair<string, object>> Parser<T>(string dataString, string filePos,
                                                                   Type basetype = null) {
            var temp = new List<KeyValuePair<string, object>>();

            dataString = dataString.Remove(0, dataString.IndexOf('~'));

            dataString = Regex.Replace(dataString, "//.*\r\n", "");
            dataString = Regex.Replace(dataString, "//.*", "");

            string[] blocks = dataString.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    string[] lines = Regex.Split(block, "\n").Select(x => x.Trim('\r')).ToArray();
                    string[] header = lines[0].Split(',');

                    temp.Add(new KeyValuePair<string, object>(header[1], Activator.CreateInstance(typeof (T))));
                    KeyValuePair<string, object> cur = temp.Last();

                    if (basetype != null) {
                        //Prototype parser
                        string prototypeNamespace = basetype.ToString();
                        prototypeNamespace = prototypeNamespace.Substring(0, prototypeNamespace.LastIndexOf('.') + 1);
                        if (!string.IsNullOrEmpty(prototypeNamespace)) {
                            Type type = Type.GetType(prototypeNamespace + header[0]);
                                //create base type namespace (e.g. "rglikeworknamelib.Dungeon.Buffs.")
                            if (type == null) {
                                logger.Error(string.Format("Subclass of {2} \"{0}\" for {1} cannot be created",
                                                           prototypeNamespace + header[0], header[1], basetype));
                                type = basetype; //prototype creation error, create base type
                            }

                            FieldInfo blockPrototypeField = typeof (T).GetField("Prototype");
                            if (blockPrototypeField != null) {
                                blockPrototypeField.SetValue(cur.Value, type);
                            }
                            else {
                                logger.Error(cur.Value.GetType().Name +
                                             " didnt contains Prototype field! Prototype " + type +
                                             " for ID " +
                                             header[0] + " didn't created");
                            }
                        }
                    }

                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].Contains('=')) {
                            string sstart = lines[i].Substring(0, lines[i].IndexOf('='));
                            FieldInfo finfo = typeof (T).GetField(sstart);
                            string extracted = lines[i].Substring(lines[i].IndexOf('=') + 1,
                                                                  lines[i].Length - (lines[i].IndexOf('=') + 1))
                                                       .Replace("\"", "");

                            if (finfo != null) {
                                TypeConverter converter = TypeDescriptor.GetConverter(finfo.FieldType);
                                if (extracted.StartsWith("{")) //Array parser
                                {
                                    extracted = extracted.Substring(1, extracted.Length - 2);
                                    IEnumerable<string> arrayextractor = extracted.Split(',').Select(x => x.Trim(' '));
                                    string[] ar = arrayextractor.ToArray();
                                    finfo.SetValue(cur.Value, ar);
                                }
                                else {
                                    object converted = converter.ConvertFromString(extracted);
                                    finfo.SetValue(cur.Value, converted);
                                }
                            }
                            else {
                                logger.Error(cur.Value.GetType().Name + " didnt contains field \"" + sstart + "\", \"" +
                                             extracted + "\" didn't assigned to ID " + header[0]);
                            }
                        }
                    }
                }
            }
            logger.Info("{0} entities for {1} loaded from {2}", temp.Count, typeof (T).ToString(), filePos);
            return temp;
        }
        public static List<object> NoIdParser<T>(string dataString, string filePos, Type basetype = null) {
            var temp = new List<object>();

            dataString = dataString.Remove(0, dataString.IndexOf('~'));

            dataString = Regex.Replace(dataString, "//.*\r\n", "");
            dataString = Regex.Replace(dataString, "//.*", "");

            string[] blocks = dataString.Split('~');
            foreach (string block in blocks) {
                if (block.Length != 0) {
                    string[] lines = Regex.Split(block, "\n").Select(x => x.Trim('\r')).ToArray();
                    string[] header = lines[0].Split(',');

                    temp.Add(Activator.CreateInstance(typeof (T)));
                    object cur = temp.Last();

                    if (basetype != null) {
                        //Prototype parser
                        string prototypeNamespace = basetype.ToString();
                        prototypeNamespace = prototypeNamespace.Substring(0, prototypeNamespace.LastIndexOf('.') + 1);
                        if (!string.IsNullOrEmpty(prototypeNamespace)) {
                            Type type = Type.GetType(prototypeNamespace + header[0]);
                                //create base type namespace (e.g. "rglikeworknamelib.Dungeon.Buffs.")
                            if (type == null) {
                                logger.Error(string.Format("Subclass of {2} \"{0}\" for {1} cannot be created",
                                                           prototypeNamespace + header[0], header[1], basetype));
                                type = basetype; //prototype creation error, create base type
                            }

                            FieldInfo blockPrototypeField = typeof (T).GetField("Prototype");
                            if (blockPrototypeField != null) {
                                blockPrototypeField.SetValue(cur, type);
                            }
                            else {
                                logger.Error(cur.GetType().Name +
                                             " didnt contains Prototype field! Prototype " + type +
                                             " for ID " +
                                             header[0] + " didn't created");
                            }
                        }
                    }

                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].Contains('=')) {
                            string sstart = lines[i].Substring(0, lines[i].IndexOf('='));
                            FieldInfo finfo = typeof (T).GetField(sstart);
                            string extracted = lines[i].Substring(lines[i].IndexOf('=') + 1,
                                                                  lines[i].Length - (lines[i].IndexOf('=') + 1))
                                                       .Replace("\"", "");

                            if (finfo != null) {
                                TypeConverter converter = TypeDescriptor.GetConverter(finfo.FieldType);
                                if (extracted.StartsWith("{")) //Array parser
                                {
                                    extracted = extracted.Substring(1, extracted.Length - 2);
                                    IEnumerable<string> arrayextractor = extracted.Split(',').Select(x => x.Trim(' '));
                                    List<string> ar = arrayextractor.ToList();
                                    finfo.SetValue(cur, ar);
                                }
                                else {
                                    object converted = converter.ConvertFromString(extracted);
                                    finfo.SetValue(cur, converted);
                                }
                            }
                            else {
                                logger.Error(cur.GetType().Name + " didnt contains field \"" + sstart + "\", \"" +
                                             extracted + "\" didn't assigned to ID " + header[0]);
                            }
                        }
                    }
                }
            }
            logger.Info("{0} entities for {1} loaded from {2}", temp.Count, typeof (T).ToString(), filePos);
            return temp;
        }

        public static Dictionary<string, T> JsonDataLoader<T>(string directory)
        {
            var serializer = new JsonSerializer { Formatting = Formatting.Indented };
            serializer.Converters.Add(new StringEnumConverter());
            var data = new Dictionary<string, T>();

            var dir = Directory.GetFiles(directory, "*.json");
            foreach (var patch in dir)
            {
                using (var sr = new StreamReader(patch, Encoding.Default))
                {
                    var t = serializer.Deserialize<Dictionary<string, T>>(new JsonTextReader(sr));
                    foreach (var itemData in t)
                    {

                        data.Add(itemData.Key, itemData.Value);
                    }
                }
            }
            return data;
        }
    }
}