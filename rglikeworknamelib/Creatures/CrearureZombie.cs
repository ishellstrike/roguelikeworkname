using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class CrearureZombie : Creature, ICreature {

        public Dress Hat = new Dress("haer1", new Color(Settings.rnd.Next() % 255, Settings.rnd.Next() % 255, Settings.rnd.Next() % 255));
        public Dress Tshort = new Dress("t-short1", new Color(Settings.rnd.Next() % 255, Settings.rnd.Next() % 255, Settings.rnd.Next() % 255));
        public Dress Pants = new Dress("pants1", new Color(Settings.rnd.Next() % 255, Settings.rnd.Next() % 255, Settings.rnd.Next() % 255));

        public void Update(GameTime gt, MapSector ms, Creature hero) {
            throw new NotImplementedException();
        }

        public override void Kill(MapSector ms)
        {
            Hp = new Stat(0, 0);
            isDead = true;
            ms.AddDecal(new Particle(WorldPosition()+new Vector2(15,15), 4) { Rotation = Settings.rnd.Next()%360, Life = new TimeSpan(0, 0, 1, 0) });
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera, MapSector ms)
        {
            var a = GetPositionInBlocks();
            var p = WorldPosition() - camera;
            spriteBatch.Draw(Atlases.CreatureAtlas[MonsterDataBase.Data[Id].MTex], p, base.col);

            var origin = Vector2.Zero;
            spriteBatch.Draw(Atlases.DressAtlas[Hat.id], WorldPosition() - camera, null, Hat.col, 0, origin, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(Atlases.DressAtlas[Pants.id], WorldPosition() - camera, null, Pants.col, 0, origin, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(Atlases.DressAtlas[Tshort.id], WorldPosition() - camera, null, Tshort.col, 0, origin, 1, SpriteEffects.None, 1);

            if (Settings.DebugInfo)
            {
                spriteBatch.DrawString(Settings.Font, position_.ToString(), p, Color.White);
            }
        }
    }
}