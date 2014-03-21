using System;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Creatures
{
    public class CreatureFactory {
        private static readonly Logger logger = LogManager.GetLogger("ItemFactory");
        public static Creature GetInstance(string id, Vector3 pos = default(Vector3))
        {
            if (!Registry.Instance.Creatures.ContainsKey(id))
            {
                logger.Error(string.Format("Missing CreatureData id={0}!!!", id));
                return null;
            }
            var creatureData = Registry.Instance.Creatures[id];
            var a = (Creature) Activator.CreateInstance(creatureData.TypeParsed);
            a.Id = id;
            a.MTex = creatureData.MTex;
            a.Position = pos;
            a.Hp = new Stat(creatureData.Hp, creatureData.Hp);

            return a;
        }
    }
}