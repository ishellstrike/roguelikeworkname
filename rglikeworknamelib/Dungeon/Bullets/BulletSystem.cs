using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class BulletSystem {
        private static Collection<Texture2D> bulletAtlas_;
        private static Collection<Bullet> bullet_;
        private static SpriteFont font;
        private static LineBatch lb;
        public static GameLevel level;
        private static SpriteBatch spriteBatch_;

        public BulletSystem(SpriteBatch spr, Collection<Texture2D> atlas, GameLevel gl, SpriteFont fnt, LineBatch l) {
            bulletAtlas_ = atlas;
            spriteBatch_ = spr;
            bullet_ = new Collection<Bullet>();
            level = gl;
            font = fnt;
            lb = l;
        }

        public static void AddBullet(Vector2 pos, float vel, float an, int dam) {
            bullet_.Add(new Bullet(pos, vel, an, 0, 1, TimeSpan.FromSeconds(1)) {Damage = dam, Owner = null});
        }

        public static void AddBullet(Creature who, float vel, float an, int dam) {
            bullet_.Add(new Bullet(new Vector2(who.WorldPosition().X, who.WorldPosition().Y), vel, an, 0, 1, TimeSpan.FromSeconds(1)) { Damage = dam, Owner = who });
        }

        public static void Update(GameTime gameTime) {
            for (int i = 0; i < bullet_.Count; i++) {
                Bullet bullet = bullet_[i];
                Bullet a = bullet;
                bullet.Update(gameTime);
                float f = a.Vel*(float) Math.Cos(a.Angle);
                float f1 = a.Vel*(float) Math.Sin(a.Angle);

                bullet.Pos += new Vector2(f, f1);

                bullet.Angle += bullet.ASpeed;

                bullet.Life -= gameTime.ElapsedGameTime;

                bullet.Start += new Vector2(f/2, f1/2);

                Vector2 positionInBlocks = bullet.GetPositionInBlocks();
                Block bl = level.GetBlock((int) positionInBlocks.X, (int) positionInBlocks.Y,
                                           true);
                if (bl != null) {
                    if (!bl.Data.IsWalkable) {
                        bullet.Life = TimeSpan.Zero;
                    }

                    bool nosector;
                    Creature creature = level.GetCreatureAtCoord(bullet.Pos, bullet.Start, out nosector);

                    if (nosector) {
                        bullet.Life = TimeSpan.Zero;
                    } else {
                        if (creature != null && creature != bullet.Owner) {
                            creature.GiveDamage(bullet.Damage, DamageType.Default);
                            bullet.Life = TimeSpan.Zero;
                        }

                        if (bullet.Life <= TimeSpan.Zero) {
                            bullet_.Remove(bullet);
                        }
                    }
                }
                else {
                    bullet_.Remove(bullet);
                }
            }
        }


        public static void Draw() {
            bool b = Settings.DebugInfo;
            foreach (Bullet bullet in bullet_) {
                //spriteBatch_.Draw(bulletAtlas_[bullet.Mtex],
                //                  bullet.Pos - cam, null,
                //                  Color.White, 0,
                //                  new Vector2(bulletAtlas_[bullet.Mtex].Width/2f,
                //                              bulletAtlas_[bullet.Mtex].Height/2f), 1, SpriteEffects.None, 0);

                lb.AddLine3D(new Vector3(bullet.Start.X/32f, bullet.Start.Y/32f, 0.1f), new Vector3(bullet.Pos.X/32f, bullet.Pos.Y/32f, 0.1f), Color.Yellow);

                //if (b) {
                //    SpriteBatch3dBinded.DrawStringCenteredUppedProjected(spriteBatch_, cam, bullet.Pos);
                //    spriteBatch_.DrawString(font, bullet.Pos.ToString(), bullet.Pos - cam, Color.White);
                //}
            }
        }

        public static int GetCount() {
            return bullet_.Count;
        }
    }
}