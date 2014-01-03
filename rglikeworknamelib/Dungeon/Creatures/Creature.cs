using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using jarg;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class Creature {
        internal Color Col;

        internal float Percenter = (float) Settings.rnd.NextDouble()/5f + 0.8f;
        public Vector2 Velocity;
        private Abilities abilities_ = new Abilities();
        private List<IBuff> buffs_ = new List<IBuff>();
        internal Stat hp_ = new Stat(200);
        private Vector2 lastpos_;

        private Order order_ = new Order(), lastOrder_;

        public Order CurrentOrder { get { return order_; } }
        public Order LastOrder { get { return lastOrder_; } }

        internal Vector2 position_;
        private TimeSpan reactionT_ = TimeSpan.Zero;
        private TimeSpan sec_ = TimeSpan.Zero;
        private Vector2 sectoroffset_;
        [NonSerialized]
        private MapSector ms_;

        [NonSerialized]
        private CreatureData data_;

        public Vector2 LastPos {
            get { return lastpos_; }
            set { lastpos_ = value; }
        }

        public Vector2 ShootPoint { get; set; }
        //public string Id { get; internal set; }

        public Vector2 Position {
            get { return position_; }
            set {
                lastpos_ = position_;
                position_ = value;
            }
        }

        public Stat Hp {
            get { return hp_; }
            set { hp_ = value; }
        }

        public bool isDead { get; internal set; }
        private string id_;
        public string Id {
            get { return id_; }
            set
            {
                id_ = value;
                Data = CreatureDataBase.Data[value];
            }
        }

        public CreatureData Data {
            get { return data_; }
            internal set { data_ = value; }
        }

        public Rectangle Source
        {
            get;
            private set;
        }

        [NonSerialized]
        private string mTex_;
        public string MTex
        {
            get
            {
                return mTex_;
            }
            set
            {
                Source = BlockData.GetSource(value);
                mTex_ = value;
            }
        }

        /// <summary>
        /// Issure concrete order
        /// </summary>
        /// <param name="value"></param>
        public void IssureOrder(Order value)
        {
            lastOrder_ = order_;
            order_ = value;
        }
        /// <summary>
        /// Issure move order
        /// </summary>
        /// <param name="value"></param>
        public void IssureOrder(Vector2 value)
        {
            lastOrder_ = order_;
            order_ = new Order(OrderType.Move, value);
        }
        /// <summary>
        /// Issure idle order
        /// </summary>
        public void IssureOrder()
        {
            order_ = new Order();
        }
        /// <summary>
        /// Issure attack order
        /// </summary>
        public void IssureOrder(Creature value)
        {
            order_ = new Order(OrderType.Attack, value);
        }

        /// <summary>
        ///     Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetWorldPositionInBlocks() {
            Vector2 po = WorldPosition();
            po.X = po.X < 0 ? po.X/32 - 1 : po.X/32;
            po.Y = po.Y < 0 ? po.Y/32 - 1 : po.Y/32;
            return po;
        }

        public event EventHandler OnDamageRecieve;
        public event EventHandler OnDeath;

        public List<IBuff> Buffs {
            get { return buffs_; }
            set { buffs_ = value; }
        }

        public Abilities Abilities {
            get { return abilities_; }
            set { abilities_ = value; }
        }

        public MapSector ms {
            get { return ms_; }
            set { ms_ = value; }
        }

        public bool IsIddle { get { return order_.Type == OrderType.Iddle; } }

        public virtual void Update(GameTime gt, MapSector ms_, Player hero) {
            ms = ms_;
            double time = gt.ElapsedGameTime.TotalSeconds;
            reactionT_ += gt.ElapsedGameTime;
            sec_ += gt.ElapsedGameTime;
            if (!Skipp || !ms_.Ready) {
                sectoroffset_ = new Vector2(ms_.SectorOffsetX, ms_.SectorOffsetY);
                Vector2 worldPositionInBlocks = GetWorldPositionInBlocks();
                Block block = ms_.Parent.GetBlock((int) worldPositionInBlocks.X, (int) worldPositionInBlocks.Y);
                if (IsIddle && block != null && block.Lightness == Color.White &&
                    reactionT_.TotalMilliseconds > Data.ReactionTime) {

                    //IssureOrder(hero);
                    IssureOrder(WorldPosition() + new Vector2(Settings.rnd.Next(-100, 100), Settings.rnd.Next(-100, 100)));

                    Col = Color.White;
                    if (sec_.TotalSeconds > 1 && ms_.Parent.IsCreatureMeele(hero, this)) {
                        hero.GiveDamage(Data.Damage, DamageType.Default, ms_);
                        sec_ = TimeSpan.Zero;
                    }
                }
                else {
                    Col = Color.Black;
                }

                OrdersMaker(ms_, time);

                //Sector changer
                if (Position.Y >= 32*MapSector.Ry) {
                    MapSector t = ms_.Parent.GetDownN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.Y = position_.Y - 32*MapSector.Ry;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.Y = 32*MapSector.Ry - 1;
                    }
                }
                else if (Position.Y < 0) {
                    MapSector t = ms_.Parent.GetUpN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.Y = position_.Y + 32*MapSector.Ry;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.Y = 0;
                    }
                }
                if (Position.X >= 32*MapSector.Rx) {
                    MapSector t = ms_.Parent.GetRightN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.X = position_.X - 32*MapSector.Rx;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.X = 32*MapSector.Rx - 1;
                    }
                }
                else if (Position.X < 0) {
                    MapSector t = ms_.Parent.GetLeftN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.X = position_.X + 32*MapSector.Rx;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.X = 0;
                    }
                }
            }

            for (int i = 0; i < Buffs.Count; i++) {
                IBuff buff = Buffs[i];
                buff.Update(gt);
                if (!buff.Applied) {
                    Buffs.Remove(buff);
                }
            }
        }

        public bool Skipp { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, LineBatch lineBatch, Vector2 camera) {
                Vector2 p = WorldPosition() - camera;
                spriteBatch.Draw(Atlases.Instance.MajorAtlas, p + new Vector2(-16, 0), Source, Col);
                if (Settings.DebugInfo) {
                    spriteBatch.DrawString(Settings.Font, order_.Type+", "+position_.ToString(), p, Color.White);
                    if (order_.Type == OrderType.Move) { lineBatch.AddLine(WorldPosition() - camera, order_.Point - camera, Color.LimeGreen, 1); }
                    if (order_.Type == OrderType.Attack) { lineBatch.AddLine(WorldPosition() - camera, order_.Target.Position - camera, Color.Red, 1); }
                }
        }

        public Vector2 WorldPosition() {
            return Position + new Vector2(-16, -32) +
                   new Vector2(sectoroffset_.X*MapSector.Rx*32, sectoroffset_.Y*MapSector.Ry*32);
        }

        public virtual void Kill(MapSector ms) {
            Hp = new Stat(0, 0);
            isDead = true;
            ms.AddDecal(new Particle(WorldPosition(), 3) {Rotation = -3.14f/2, Life = new TimeSpan(0, 0, 1, 0)});
            if (OnDeath != null) {
                OnDeath(null, null);
            }
        }

        public void GiveDamage(float value, DamageType type) {
            hp_.Current -= value;
            EventLog.Add(string.Format("{0} �������� {1} �����", Data.Name, value),
                         GlobalWorldLogic.CurrentTime, Color.Pink, LogEntityType.Damage);
            if (Hp.Current <= 0) {
                Kill(ms);
                EventLog.Add(string.Format("{0} �������!", Data.Name.ToUpper()),
                             GlobalWorldLogic.CurrentTime, Color.Pink, LogEntityType.Dies);
            }
            var adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));

            ms.AddDecal(new Particle(WorldPosition() + adder, 3)
            {Rotation = Settings.rnd.Next()%360, Life = new TimeSpan(0, 0, 1, 0)});

            if (OnDamageRecieve != null) {
                OnDamageRecieve(null, null);
            }
        }

        public void SetPositionInBlocks(int x, int y) {
            position_ = new Vector2((x + 0.5f)*32, (y + 0.5f)*32);
        }

        /// <summary>
        ///     Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks() {
            Vector2 po = Position;
            po.X = po.X < 0 ? po.X/32 - 1 : po.X/32;
            po.Y = po.Y < 0 ? po.Y/32 - 1 : po.Y/32;
            return po;
        }

        public Ability GetAbility(string s) {
            return abilities_.list[s];
        }

        private void OrdersMaker(MapSector ms, double time) {
            if (order_.Type == OrderType.Move) {
                Vector2 wp = WorldPosition();
                Vector2 mover = order_.Point - wp;
                float percenterMax = Data.Speed * Percenter;

                if (mover.Length() > time * percenterMax)
                {
                    mover.Normalize();
                    mover *= (float)time * percenterMax;
                }

                Vector2 newwposx = GetWorldPositionInBlocks() + new Vector2(mover.X, 0);
                Vector2 newwposy = GetWorldPositionInBlocks() + new Vector2(0, mover.Y);

                Block key = ms.Parent.GetBlock((int)newwposx.X, (int)newwposx.Y);
                if (key != null && key.Id != null && key.Data.IsWalkable)
                {
                    position_.X += mover.X;
                }
                key = ms.Parent.GetBlock((int)newwposy.X, (int)newwposy.Y);
                if (key != null && (key.Id != null && key.Data.IsWalkable))
                {
                    position_.Y += mover.Y;
                }

                wp = WorldPosition();
                if (Math.Abs(wp.X - order_.Point.X) < 10 && Math.Abs(wp.Y - order_.Point.Y) < 10)
                {
                    IssureOrder();
                }
            } else if (order_.Type == OrderType.Attack)
                {
                    Vector2 wp = WorldPosition();
                    Vector2 mover = order_.Target.Position - wp;
                    float percenterMax = Data.Speed * Percenter;

                    if (mover.Length() > time * percenterMax)
                    {
                        mover.Normalize();
                        mover *= (float)time * percenterMax;
                    }

                    Vector2 newwposx = GetWorldPositionInBlocks() + new Vector2(mover.X, 0);
                    Vector2 newwposy = GetWorldPositionInBlocks() + new Vector2(0, mover.Y);

                    Block key = ms.Parent.GetBlock((int)newwposx.X, (int)newwposx.Y);
                    if (key != null && key.Id != null && key.Data.IsWalkable)
                    {
                        position_.X += mover.X;
                    }
                    key = ms.Parent.GetBlock((int)newwposy.X, (int)newwposy.Y);
                    if (key != null && (key.Id != null && key.Data.IsWalkable))
                    {
                        position_.Y += mover.Y;
                    }

                    wp = WorldPosition();
                    if (Math.Abs(wp.X - order_.Target.Position.X) < 10 && Math.Abs(wp.Y - order_.Target.Position.Y) < 10)
                    {
                        IssureOrder();
                    }
                }
        }
    }
}