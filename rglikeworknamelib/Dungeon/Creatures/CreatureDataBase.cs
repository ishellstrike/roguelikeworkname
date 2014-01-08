using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Parser;
using Microsoft.Xna.Framework;
using System;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Level.Blocks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NLog;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Creatures {
    public class CreatureDataBase {
        public static Dictionary<string, CreatureData> Data;
        public static Dictionary<string, dynamic> Scripts;
        static readonly ScriptRuntime Ipy = Python.CreateRuntime();
        static readonly Logger Logger = LogManager.GetLogger("CreatureDataBase");

        public CreatureDataBase() {
            Data = UniversalParser.JsonDataLoader<CreatureData>(Settings.GetCreatureDataDirectory());

            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Creature assembly loading";

            Ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Creature)));
            Ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Block)));
            Ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Item)));
            Ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(BulletSystem)));

            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Creature base script loading";

            dynamic bsNothing = Ipy.UseFile(Settings.GetCreatureDataDirectory() + "\\bs_nothing.py");
            var files = Directory.GetFiles(Settings.GetCreatureDataDirectory(), "*.py");
            Scripts = new Dictionary<string, dynamic>();
            var i = 0;
            foreach (var f in files)
            {
                var name = Path.GetFileNameWithoutExtension(f);
                Settings.NeedToShowInfoWindow = true;
                Settings.NTS1 = "BScripts: ";
                Settings.NTS2 = string.Format("{0}/{1} ({2})", i+1, files.Length, name);
                i++;
                dynamic temp;
                try
                {
                    temp = Ipy.UseFile(f);
                }
                catch(Exception ex)
                {
                    Scripts.Add(name, bsNothing);
                    Logger.Error(ex);
#if DEBUG
                    throw;
#else
                    continue;
#endif

                }
                Scripts.Add(name, temp);
            }
            Settings.NTS2 = string.Empty;
            Settings.NeedToShowInfoWindow = false;
        }
    }
}