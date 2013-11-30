using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class BulletSystem {
        private readonly Collection<Texture2D> bulletAtlas_;
        private readonly Collection<Bullet> bullet_;
        private readonly SpriteFont font;
        private readonly LineBatch lb;
        private readonly GameLevel level;
        private readonly SpriteBatch spriteBatch_;

        public BulletSystem(SpriteBatch spr, Collection<Texture2D> atlas, GameLevel gl, SpriteFont fnt, LineBatch l) {
            bulletAtlas_ = atlas;
            spriteBatch_ = spr;
            bullet_ = new Collection<Bullet>();
            level = gl;
            font = fnt;
            lb = l;
        }

        public void AddBullet(Vector2 pos, float vel, float an, int dam) {
            bullet_.Add(new Bullet(pos, vel, an, 0, 1, TimeSpan.FromSeconds(1)) {Damage = dam, Owner = null});
        }

        public void AddBullet(Creature who, float vel, float an, int dam) {
            bullet_.Add(new Bullet(who.Position, vel, an, 0, 1, TimeSpan.FromSeconds(1)) {Damage = dam, Owner = who});
        }

        public void Update(GameTime gameTime) {
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
                IBlock bl = level.GetBlock((int) positionInBlocks.X, (int) positionInBlocks.Y,
                                           true);
                if (bl != null) {
                    if (!bl.Data.IsWalkable) {
                        bullet.Life = TimeSpan.Zero;
                    }

                    bool nosector;
                    ICreature sect = level.GetCreatureAtCoord(bullet.Pos, bullet.Start, out nosector);

                    if (nosector) {
                        bullet.Life = TimeSpan.Zero;
                    } else {
                        if (sect != null) {
                            sect.GiveDamage(bullet.Damage, DamageType.Default);
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


        public void Draw(GameTime gameTime, Vector2 cam) {
            bool b = Settings.DebugInfo;
            foreach (Bullet bullet in bullet_) {
                spriteBatch_.Draw(bulletAtlas_[bullet.Mtex],
                                  bullet.Pos - cam, null,
                                  Color.White, 0,
                                  new Vector2(bulletAtlas_[bullet.Mtex].Width/2,
                                              bulletAtlas_[bullet.Mtex].Height/2), 1, SpriteEffects.None, 0);

                lb.AddLine(bullet.Start - cam, bullet.Pos - cam, Color.Yellow, 2);

                if (b) {
                    spriteBatch_.DrawString(font, bullet.Pos.ToString(), bullet.Pos - cam, Color.White);
                }
            }
        }

        public int GetCount() {
            return bullet_.Count;
        }
    }
}