using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class BlockAction
    {
        public string Name;
        public dynamic Action;

        public BlockAction(dynamic openCan, string s)
        {
            Name = s;
            Action = openCan;
        }
    }

    public class BlockDataBase {
        private static readonly Logger logger = LogManager.GetLogger("BlockDataBase");
        private readonly ScriptRuntime ipy = Python.CreateRuntime();
        public static Dictionary<string, BlockData> Data;
        public static Dictionary<string, BlockData> Storages;
        public static Dictionary<string, BlockAction> BlockScripts; 


        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public BlockDataBase() {
            Storages = new Dictionary<string, BlockData>();

            Data = UniversalParser.JsonDictionaryDataLoader<BlockData>(Settings.GetObjectDataDirectory());

            foreach (var pair in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                Storages.Add(pair.Key, pair.Value);
            }

            var idb = ItemDataBase.Instance;

            int i;
            foreach (var source in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                foreach (var dropGroup in source.Value.ItemSpawn) {
                    for (i = 0; i < dropGroup.Ids.Count; i++) {
                        var id = dropGroup.Ids[i];
                        if (id.StartsWith("spawn_")) {
                            dropGroup.Ids.Remove(id);
                            dropGroup.Ids.AddRange(idb.GetBySpawnGroup(id).Select(itemData => itemData.Key));
                        }
                    }
                }
            }

            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Block assembly loading";

            ipy.LoadAssembly(Assembly.GetAssembly(typeof(Block)));
            ipy.LoadAssembly(Assembly.GetAssembly(typeof(ParticleSystem)));

            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Block base script loading";

            dynamic isNothing = ipy.UseFile(Settings.GetObjectDataDirectory() + "\\bs_nothing.py");

            string[] files = Directory.GetFiles(Settings.GetObjectDataDirectory(), "*.py");
            BlockScripts = new Dictionary<string, BlockAction>();
            i = 0;
            foreach (string f in files)
            {
                string name = Path.GetFileNameWithoutExtension(f);
                Settings.NeedToShowInfoWindow = true;
                Settings.NTS1 = "BScripts: ";
                Settings.NTS2 = string.Format("{0}/{1} ({2})", i + 1, files.Length, name);
                i++;
                dynamic temp;
                string disc = string.Empty;
                try
                {
                    temp = ipy.UseFile(f);
                }
                catch (Exception ex)
                {
                    if (name != null)
                    {
                        BlockScripts.Add(name, new BlockAction(isNothing, "ошибка"));
                    }
                    logger.Error(ex);
#if DEBUG
                    throw;
#else
                    continue;
#endif
                }
                var ia = new BlockAction(temp, disc);
                string ss;
                try
                {
                    ss = (string)temp.Name();
                }
                catch (Exception ex)
                {
                    ia.Name = "error";
                    BlockScripts.Add(name, ia);
#if DEBUG
                    throw;
#else
                    continue;
#endif
                }

                ia.Name = ss;
                BlockScripts.Add(name, ia);

            }

            Settings.NTS2 = string.Empty;
            Settings.NeedToShowInfoWindow = false;
        }

        public static void TrySpawnItems(Random rnd, Block block)
        {
            if (block.Data.ItemSpawn != null)
            {
                foreach (var drop in block.Data.ItemSpawn)
                {

                    for (int n = 0; n < drop.Repeat; n++)
                    {
                        var thr = rnd.Next(100) + 1;
                        if (drop.Prob >= thr)
                        {
                            var item = ItemFactory.GetInstance(drop.Ids[rnd.Next(drop.Ids.Count)],
                                rnd.Next(Math.Abs(drop.Max - drop.Min)) +
                                Math.Min(drop.Min, drop.Max));
                            if (item != null)
                            {
                                block.StoredItems.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}