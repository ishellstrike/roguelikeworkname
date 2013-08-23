using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Creatures {
    public class MonsterSystem {
        private Dictionary<string, Texture2D> _atlas;
        private SpriteBatch _spriteBatch;
        private Collection<Creature> a;

        public MonsterSystem(SpriteBatch spr, Dictionary<string, Texture2D> atlas)
        {
            _atlas = atlas;
            _spriteBatch = spr;
        }

        public void Update(GameTime gt)
        {

        }

        public void Draw(GameTime gt)
        {
            foreach (var creature in a) {
                
            }
        }
    }
}