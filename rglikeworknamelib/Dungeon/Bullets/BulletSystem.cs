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
            _bullet.Add(new Bullet {_position = pos, _velocity = vel, _gApply = vel.Z != 0});
        }

        public void AddBullet(Creature pl, Vector3 dir) {
            _bullet.Add(new Bullet {_position = pl.Position + pl.ShootPoint, _mtex = 0, _velocity = dir * 50f + pl.Velocity});
        }

        public void Update(GameTime gameTime) {
            for (int i = 0; i < _bullet.Count; i++) {
                Bullet bullet = _bullet[i];
                if (bullet._gApply) {
                    bullet._position.Z += Settings.G();
                }

                bullet._position += bullet._velocity * (float) gameTime.ElapsedGameTime.TotalSeconds * 20;

                if (bullet._position.Z < 0) {
                    //kill
                    _bullet.RemoveAt(i);
                }
            }
        }


        public void Draw(GameTime gameTime, Vector2 cam) {
            foreach (Bullet bullet in _bullet) {
                _spriteBatch.Draw(_bulletAtlas[bullet._mtex],
                                  new Vector2(bullet._position.X, bullet._position.Y - bullet._position.Z) - cam, null,
                                  Color.White, 0,
                                  new Vector2(_bulletAtlas[bullet._mtex].Width / 2,
                                              _bulletAtlas[bullet._mtex].Height / 2), 1, SpriteEffects.None, 0);
            }
        }
    }
}