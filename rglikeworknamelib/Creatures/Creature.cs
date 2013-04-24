using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Creatures {
    public class Creature {
        public int ID;
        public int Mtex;
        private Vector3 _position;
        private Vector3 _lastpos;

        public Vector3 Position {
            get { return _position; }
            set { _position = value; }
        }

        public void SetPositionInBlocks(int x, int y)
        {
            _position = new Vector3((x + 0.5f) * 32, (y + 0.5f) * 32, 0);
        }

        public Vector3 LastPos
        {
            get { return _lastpos; }
            set { _lastpos = value; }
        }

        public Vector2 InScreenPosition {
            get { return new Vector2(_position.X, _position.Y - _position.Z); }
        }

        public Vector3 ShootPoint { get; set; }

        public Vector3 Velocity;
    }
}