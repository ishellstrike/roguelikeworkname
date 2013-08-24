using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Creatures {
    public struct Stat {
        public float Current, Max;

        public Stat(float a, float b) {
            Current = a;
            Max = b;
        }

        public Stat(float a)
        {
            Current = a;
            Max = a;
        }
    }
    public interface ICreature {
        Rectangle Location { get; set; }
        Stat Hp { get; set; }
        bool isDead { get; }
        void Kill();
        void GiveDamage(float value, DamageType type);
    }
    public class Creature : ICreature {
        public string ID;

        private Vector2 lastpos_;
        public MapSector parent;

        public bool IsWarmCloth() {
            return true;
        }

        public Rectangle Location { get; set; }

        public Stat Hunger  = new Stat(100);

        public Stat Thirst = new Stat(100);

        public Stat Heat = new Stat(36);

        private Vector2 position_;
        public Vector2 Position  {
            get { return position_; }
            set {
                lastpos_ = position_; 
                Location = new Rectangle((int)position_.X-16,(int)position_.Y-32,32,32);
                position_ = value;
            }
        }

        private Stat hp_;
        public Stat Hp {
            get { return hp_; }
            set { hp_ = value; }
        }

        public bool isDead { get; private set; }

        public void SetPositionInBlocks(int x, int y)
        {
            position_ = new Vector2((x + 0.5f) * 32, (y + 0.5f) * 32);
        }

        /// <summary>
        /// Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks() {
            var po = Position;
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        public Vector2 LastPos
        {
            get { return lastpos_; }
            set { lastpos_ = value; }
        }

        public void Update(GameTime gt, MapSector ms) {
            if (Hunger.Current > 0) {
                Hunger.Current -= (float) gt.ElapsedGameTime.TotalDays*60f;
            }  else {
                int a = IsWarmCloth() ? 10 : 1;
                Heat.Current -= Heat.Current * (float)gt.ElapsedGameTime.TotalMinutes / 10 / a;
                Heat.Current += GlobalWorldLogic.Temperature * (float)gt.ElapsedGameTime.TotalMinutes / 10 / a;
            }

            Position = new Vector2(position_.X+(float)Settings.rnd.NextDouble() - 0.5f, position_.Y+(float)Settings.rnd.NextDouble() - 0.5f);
        }


        public Vector2 ShootPoint { get; set; }

        public Vector2 Velocity;

        public void Draw(SpriteBatch spriteBatch, Vector2 camera, MapSector ms) {
            var a = GetPositionInBlocks();
            var p = WorldPosition() - camera;
            spriteBatch.Draw(Atlases.CreatureAtlas[MonsterDataBase.Data[ID].MTex], p, ms.GetBlock((int)a.X, (int)a.Y).Lightness);
            if (Settings.DebugInfo) {
                spriteBatch.DrawString(Settings.Font, position_.ToString(), p, Color.White);
            }
        }

        private Vector2 WorldPosition() {
            return Position + new Vector2(-16, -32) +
                   new Vector2(parent.SectorOffsetX*MapSector.Rx*32, parent.SectorOffsetY*MapSector.Ry*32);
        }

        public void Kill() {
            Hp = new Stat(0,0);
            isDead = true;
        }

        public void GiveDamage(float value, DamageType type) {

            hp_.Current -= value;
            if(Hp.Current <= 0 ) {
                Kill();
            }
            Vector2 adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));
            parent.decals.Add(new Particle(WorldPosition() + adder, 3) { Rotation = Settings.rnd.Next() % 360, Life = new TimeSpan(0, 0, 1, 0) });
        }

        public void GiveDamage(float value)
        {
            GiveDamage(value, DamageType.Default);
        }
    }

    public enum DamageType {
        Default
    }
}