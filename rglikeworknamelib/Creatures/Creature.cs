using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Creatures {
    public class Creature {
        public int ID;
        public int Mtex;

        private Vector3 lastpos_;

        public bool IsWarmCloth() {
            return true;
        }

        private float hunger_=100;
        public float Hunger {
            get { return hunger_; }
        }

        private float thirst_=100;
        public float Thirst {
            get { return thirst_; }
        }

        private float heat_=36;
        public float Heat {
            get { return heat_; }
        }

        private Vector3 position_;
        public Vector3 Position  {
            get { return position_; }
            set {
                lastpos_ = position_; 
                position_ = value;
            }
        }

        public void SetPositionInBlocks(int x, int y)
        {
            position_ = new Vector3((x + 0.5f) * 32, (y + 0.5f) * 32, 0);
        }

        public Vector3 LastPos
        {
            get { return lastpos_; }
            set { lastpos_ = value; }
        }

        public void Update(GameTime gt) {
            
            if (hunger_ > 0) {
                hunger_ -= (float) gt.ElapsedGameTime.TotalDays*60f;
            }  else {
                int a = IsWarmCloth() ? 10 : 1;
                heat_ -= heat_ * (float)gt.ElapsedGameTime.TotalMinutes / 10 / a;
                heat_ += GlobalWorldLogic.temperature * (float)gt.ElapsedGameTime.TotalMinutes / 10 / a;
            }
        }

        public Vector2 InScreenPosition {
            get { return new Vector2(position_.X, position_.Y - position_.Z); }
        }

        public Vector3 ShootPoint { get; set; }

        public Vector3 Velocity;
    }
}