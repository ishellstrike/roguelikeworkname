using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class BulletSystem {
        private readonly Collection<Bullet> _bullet;
        private readonly Collection<Texture2D> _bulletAtlas;
        private readonly SpriteBatch _spriteBatch;

        public BulletSystem(SpriteBatch spr, Collection<Texture2D> atlas) {
            _bulletAtlas = atlas;
            _spriteBatch = spr;
            _bullet = new Collection<Bullet>();
        }

        public void AddBullet(Vector3 pos, Vector3 vel) {
            _bullet.Add(new Bullet {Position = pos, Velocity = vel, GApply = vel.Z != 0});
        }

        public void AddBullet(Creature pl, Vector3 dir) {
            _bullet.Add(new Bullet {Position = pl.Position + pl.ShootPoint, Mtex = 0, Velocity = dir * 50f + pl.Velocity});
        }

        public void Update(GameTime gameTime) {
            for (int i = 0; i < _bullet.Count; i++) {
                Bullet bullet = _bullet[i];
                if (bullet.GApply) {
                    bullet.Position.Z += Settings.G();
                }

                bullet.Position += bullet.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds * 20;

                if (bullet.Position.Z < 0) {
                    //kill
                    _bullet.RemoveAt(i);
                }
            }
        }


        public void Draw(GameTime gameTime, Vector2 cam) {
            foreach (Bullet bullet in _bullet) {
                _spriteBatch.Draw(_bulletAtlas[bullet.Mtex],
                                  new Vector2(bullet.Position.X, bullet.Position.Y - bullet.Position.Z) - cam, null,
                                  Color.White, 0,
                                  new Vector2(_bulletAtlas[bullet.Mtex].Width / 2,
                                              _bulletAtlas[bullet.Mtex].Height / 2), 1, SpriteEffects.None, 0);
            }
        }
    }
}