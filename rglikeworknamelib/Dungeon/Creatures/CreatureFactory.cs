using System;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Creatures {
    public class CreatureFactory {
        private static readonly Logger logger = LogManager.GetLogger("ItemFactory");
        public static Creature GetInstance(string id, Vector2 pos = default(Vector2))
        {
            if (!CreatureDataBase.Data.ContainsKey(id))
            {
                logger.Error(string.Format("Missing CreatureData id={0}!!!", id));
                return null;
            }
            CreatureData creatureData = CreatureDataBase.Data[id];
            var a = new Creature();
            a.Id = id;
            a.OnLoad();
            a.Position = pos;
            a.Hp = new Stat(creatureData.Hp, creatureData.Hp);

            return a;
        }
    }
}