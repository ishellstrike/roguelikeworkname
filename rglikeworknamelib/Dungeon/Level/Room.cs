using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level {
    public class Room {
        public const int RoomDimX = 10;
        public const int RoomDimY = 5;
        public const int RoomT = RoomDimX*RoomDimY;
        private readonly Texture2D _atlas;
        private readonly Block[] _block;
        private readonly SpriteBatch _spriteBatch;

        public Room(SpriteBatch sb, Texture2D atl) {
            _spriteBatch = sb;
            _atlas = atl;
            _block = new Block[RoomT];
            for (int i = 0; i < _block.Length; i++) {
                _block[i] = new Block();
            }
        }
    }
}