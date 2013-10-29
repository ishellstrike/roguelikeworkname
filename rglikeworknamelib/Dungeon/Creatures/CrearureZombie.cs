using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class CrearureZombie : Creature {
        public Dress Hat = new Dress("haer1",
                                     new Color(Settings.rnd.Next()%255, Settings.rnd.Next()%255, Settings.rnd.Next()%255));

        public Dress Pants = new Dress("pants1",
                                       new Color(Settings.rnd.Next()%255, Settings.rnd.Next()%255,
                                                 Settings.rnd.Next()%255));

        public Dress Tshort = new Dress("t-short1",
                                        new Color(Settings.rnd.Next()%255, Settings.rnd.Next()%255,
                                                  Settings.rnd.Next()%255));

        public override void Kill(MapSector ms) {
            Hp = new Stat(0, 0);
            isDead = true;
            ms.AddDecal(new Particle(WorldPosition() + new Vector2(15, 15), 4) {
                Rotation = Settings.rnd.Next()%360,
                Life = new TimeSpan(0, 0, 1, 0)
            });
            Achievements.Stat["zombiekill"].Count++;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera, MapSector ms) {
            Vector2 a = GetPositionInBlocks();
            Vector2 p = WorldPosition() - camera;
            Vector2 position = p + new Vector2(-16, 0);
            spriteBatch.Draw(Atlases.CreatureAtlas[MonsterDataBase.Data[Id].MTex], position, Col);

            Vector2 origin = Vector2.Zero;
            if (Col != Color.Black) {
                spriteBatch.Draw(Atlases.DressAtlas[Hat.id], position, null, Hat.col, 0, origin, 1,
                                 SpriteEffects.None, 1);
                spriteBatch.Draw(Atlases.DressAtlas[Pants.id], position, null, Pants.col, 0, origin, 1,
                                 SpriteEffects.None, 1);
                spriteBatch.Draw(Atlases.DressAtlas[Tshort.id], position, null, Tshort.col, 0, origin, 1,
                                 SpriteEffects.None, 1);
            }

            if (Settings.DebugInfo) {
                spriteBatch.DrawString(Settings.Font, position_.ToString(), p, Color.White);
            }
        }
    }
}