using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Creatures {
    public class MonsterSystem {
        private Collection<Texture2D> _atlas;
        private SpriteBatch _spriteBatch;
        private Collection<Creature> a;

        public MonsterSystem(SpriteBatch spr, Collection<Texture2D> atlas)
        {
            _atlas = atlas;
            _spriteBatch = spr;
        }

        public void Update(GameTime gt)
        {

        }

        public void Draw(GameTime gt)
        {

        }
    }
}