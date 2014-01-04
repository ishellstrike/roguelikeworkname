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

namespace rglikeworknamelib.Dungeon.Creatures {
    public class CreatureDataBase {
        public static Dictionary<string, CreatureData> Data;
        public static Dictionary<string, dynamic> Scripts;
        private static dynamic bs_nothing;
        static ScriptRuntime ipy = Python.CreateRuntime();

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
                catch
                {
                    Scripts.Add(name, bs_nothing);
                    continue;
                }
                Scripts.Add(name, temp);
            }
        }

        public static void Bs_zombie(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            if (target.IsIddle)
            {
                target.IssureOrder(new Order(OrderType.Wander));
            }

            Vector2 worldPositionInBlocks = target.GetWorldPositionInBlocks();
            Block block = ms_.Parent.GetBlock((int)worldPositionInBlocks.X, (int)worldPositionInBlocks.Y);
            if (target.IsIddleOrWander && block != null && block.Lightness == Color.White &&
                target.reactionT_.TotalMilliseconds > target.Data.ReactionTime && (hero.Position - target.WorldPosition()).Length() < 640)
            {

                target.IssureOrder(hero);

                target.Col = Color.White;
                if (target.sec_.TotalSeconds > 1 && ms_.Parent.IsCreatureMeele(hero, target))
                {
                    hero.GiveDamage(target.Data.Damage, DamageType.Default, ms_);
                    target.sec_ = TimeSpan.Zero;
                }
            }
            else
            {
                // Col = Color.Black;
            }
        }

        public static void Bs_wander(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            if (target.IsIddle)
            {
                if (Settings.rnd.Next(3) == 1)
                {
                    target.IssureOrder(new Order(OrderType.Wander));
                }
                else
                {
                    target.IssureOrder(new Order(OrderType.Sleep, Settings.rnd.Next(1000,10000)));
                }
            }
        }

        public static void Bs_rabbit(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            if ((hero.Position - target.WorldPosition()).Length() < 128)
            {
                var ptemp = target.WorldPosition();
                var p2 = new Vector3(hero.Position.X, hero.Position.Y, 0);
                var p1 = new Vector3(ptemp.X, ptemp.Y, 0);
                Ray a = new Ray(p1, -Vector3.Normalize(p2 - p1));
                var b = a.Position + a.Direction * 256;
                target.IssureOrder(new Vector2(b.X, b.Y));
            }

            if (target.IsIddle)
            {
                if (Settings.rnd.Next(3) == 1)
                {
                    target.IssureOrder(new Order(OrderType.Wander));
                }
                else
                {
                    target.IssureOrder(new Order(OrderType.Sleep, Settings.rnd.Next(1000,10000)));
                }
            }
        }

        public static void Bs_dog(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            if (target.behaviorTag != null && (int)target.behaviorTag > 0 && target.IsIddle)
            {
                target.IssureOrder(hero.Position + new Vector2(Settings.rnd.Next(-128, 128), Settings.rnd.Next(-128, 128)));
                target.behaviorTag = ((int)target.behaviorTag) - 1;
            }

            if (target.IsIddle)
            {
                var a = Settings.rnd.Next(3); 
                if (a == 1)
                {
                    target.IssureOrder(new Order(OrderType.Wander));
                }
                else if (a == 2)
                {
                    target.IssureOrder(new Order(OrderType.Sleep, Settings.rnd.Next(1000, 3000)));
                } else if ((hero.Position - target.WorldPosition()).Length() < 256)
                {
                    target.IssureOrder(hero.Position + new Vector2(Settings.rnd.Next(-128, 128), Settings.rnd.Next(-128, 128)));
                    target.behaviorTag = (int)10;
                }
            }
        }


        public static void ScriptExecute(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            Scripts[target.Data.BehaviorScript].BehaviorScript(gt, ms_, hero, target, Settings.rnd);
        }
        
    }
}