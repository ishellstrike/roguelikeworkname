using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;
using Microsoft.Xna.Framework;
using System;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Level.Blocks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NLog;

namespace rglikeworknamelib.Dungeon.Creatures {
    public class CreatureDataBase {
        public static Dictionary<string, CreatureData> Data;
        public static Dictionary<string, dynamic> Scripts;
        private static dynamic bs_nothing;
        static ScriptRuntime ipy = Python.CreateRuntime();
        static Logger logger = LogManager.GetLogger("CreatureDataBase");

        public CreatureDataBase() {
            Data = UniversalParser.JsonDataLoader<CreatureData>(Settings.GetCreatureDataDirectory());


            ipy.LoadAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            bs_nothing = ipy.UseFile(Settings.GetCreatureDataDirectory() + "\\bs_nothing.py");
            var files = Directory.GetFiles(Settings.GetCreatureDataDirectory(), "*.py");
            Scripts = new Dictionary<string,dynamic>();
            foreach (var f in files)
            {
                var r = new FileInfo(f);
                string name = r.Name.Replace(r.Extension, string.Empty);
                dynamic temp = null;
                try
                {
                    temp = ipy.UseFile(f);
                }
                catch(Exception ex)
                {
                    Scripts.Add(name, bs_nothing);
                    logger.Error(ex);
#if DEBUG
                    throw ex;
#endif
                    continue;
                }
                Scripts.Add(name, temp);
            }
        }

        //public static void Bs_zombie(GameTime gt, MapSector ms_, Player hero, Creature target)
        //{
        //    if (target.IsIddle)
        //    {
        //        target.IssureOrder(new Order(OrderType.Wander));
        //    }

        //    Vector2 worldPositionInBlocks = target.GetWorldPositionInBlocks();
        //    Block block = ms_.Parent.GetBlock((int)worldPositionInBlocks.X, (int)worldPositionInBlocks.Y);
        //    if (target.IsIddleOrWander && block != null && block.Lightness == Color.White &&
        //        target.reactionT_.TotalMilliseconds > target.Data.ReactionTime && (hero.Position - target.WorldPosition()).Length() < 640)
        //    {

        //        target.IssureOrder(hero);

        //        target.Col = Color.White;
        //        if (target.sec_.TotalSeconds > 1 && ms_.Parent.IsCreatureMeele(hero, target))
        //        {
        //            hero.GiveDamage(target.Data.Damage, DamageType.Default, ms_);
        //            target.sec_ = TimeSpan.Zero;
        //        }
        //    }
        //    else
        //    {
        //        // Col = Color.Black;
        //    }
        //}



        public static void ScriptExecute(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            Scripts[target.Data.BehaviorScript].BehaviorScript(gt, ms_, hero, target, Settings.rnd);
        }
        
    }
}