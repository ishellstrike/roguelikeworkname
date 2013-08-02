using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class BulletSystem {
        private readonly Collection<Bullet> bullet_;
        private readonly Collection<Texture2D> _bulletAtlas;
        private readonly SpriteBatch _spriteBatch;
        private ParticleSystem p;

        public BulletSystem(SpriteBatch spr, Collection<Texture2D> atlas, ParticleSystem ps) {
            _bulletAtlas = atlas;
            _spriteBatch = spr;
            bullet_ = new Collection<Bullet>();

            p = ps;
        }

        public void AddBullet(Vector2 pos, float vel, float an) {
            bullet_.Add(new Bullet(pos, vel, an, 0, 1, TimeSpan.FromSeconds(10), p));
        }

        public void AddBullet(Creature who, float vel, float an)
        {
            bullet_.Add(new Bullet(who.Position, vel, an, 0, 1, TimeSpan.FromSeconds(10), p));
        }

        public void Update(GameTime gameTime) {
            for (int i = 0; i < bullet_.Count; i++) {
                var a = bullet_[i];
                bullet_[i].Update(gameTime);
                bullet_[i].Pos = new Vector2(a.Pos.X + a.Vel * (float)Math.Cos(a.Angle), bullet_[i].Pos.Y + a.Vel * (float)Math.Sin(a.Angle));

                bullet_[i].Angle += bullet_[i].ASpeed;

                bullet_[i].Life -= gameTime.ElapsedGameTime;

                if (bullet_[i].Life <= TimeSpan.Zero)
                {
                    bullet_.Remove(bullet_[i]);
                    continue;
                }
            }
        }


        public void Draw(GameTime gameTime, Vector2 cam) {
            foreach (Bullet bullet in bullet_) {
                _spriteBatch.Draw(_bulletAtlas[bullet.Mtex],
                                  bullet.Pos - cam, null,
                                  Color.White, 0,
                                  new Vector2(_bulletAtlas[bullet.Mtex].Width / 2,
                                              _bulletAtlas[bullet.Mtex].Height / 2), 1, SpriteEffects.None, 0);
            }
        }

        public int GetCount() {
            return bullet_.Count;
        }
    }
}