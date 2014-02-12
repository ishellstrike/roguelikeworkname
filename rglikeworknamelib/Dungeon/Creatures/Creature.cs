using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog.Targets;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using jarg;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Creatures
{
    [Serializable]
    public class Creature {
        internal Color Col;

        internal float Percenter = (float) Settings.rnd.NextDouble()/5f + 0.8f;
        public Vector3 Velocity;
        private Abilities abilities_ = new Abilities();
        private List<IBuff> buffs_ = new List<IBuff>();
        internal Stat hp_ = new Stat(200);
        private Vector3 lastpos_;

        public Matrix creatureWorld = Matrix.Identity;
        public VertexPositionNormalTexture[] vert;

        /// <summary>
        /// can be used for behavior proposes
        /// </summary>
        public object BehaviorTag;

        private Order order_ = new Order(), lastOrder_;

        public Order CurrentOrder { get { return order_; } }
        public Order LastOrder { get { return lastOrder_; } }

        internal Vector3 position_;
        public TimeSpan reactionT_ = TimeSpan.Zero;
        public TimeSpan sec_ = TimeSpan.Zero;
        public Vector2 sectoroffset_;
        [NonSerialized]
        private MapSector ms_;

        [NonSerialized]
        private CreatureData data_;

        public Vector3 LastPos {
            get { return lastpos_; }
            set { lastpos_ = value; }
        }

        public Vector2 ShootPoint { get; set; }
        //public string Id { get; internal set; }

        public Vector3 Position {
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

        public Vector2 Source
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
                if (!Settings.Server) {
                    BuildGeom();
                }
                mTex_ = value;
            }
        }

        private void BuildGeom() {
            var a = new List<VertexPositionNormalTexture>();
            a.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Up, Source));
            a.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Up, Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
            a.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Up, Source + new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));
            a.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Up, Source + new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));
            a.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0), Vector3.Up, Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
            a.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Up, Source));

            vert = a.ToArray();
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
        /// Issure sleep order
        /// </summary>
        public void IssureOrder(int x)
        {
            lastOrder_ = order_;
            order_ = new Order(OrderType.Sleep, x);
        }
        /// <summary>
        /// Issure move order
        /// </summary>
        public void IssureOrder(float x, float y)
        {
            lastOrder_ = order_;
            order_ = new Order(OrderType.Move, x, y);
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
            Vector2 po = new Vector2(WorldPosition().X, WorldPosition().Y);
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

        public bool IsIddleOrWander { get { return order_.Type == OrderType.Iddle || order_.Type == OrderType.Wander; } }

        public override string ToString() {
            return Data.Name + " " + Id + " " + position_;
        }

        public virtual void Update(GameTime gt, MapSector ms_, Player hero, bool test = false) {
            ms = ms_;
            
            reactionT_ += gt.ElapsedGameTime;
            sec_ += gt.ElapsedGameTime;
            Col = Color.White;
            if ((!Skipp || !ms_.Ready))
            {
                sectoroffset_ = new Vector2(ms_.SectorOffsetX, ms_.SectorOffsetY);
                CreatureDataBase.Scripts[Data.BehaviorScript].BehaviorScript(gt, ms_, hero, this, Settings.rnd);


                OrdersMaker(ms_, gt);

                if(test) {return;}
                //Sector changer
                if (Position.Y >= 32 * MapSector.Ry)
                {
                    MapSector t = ms_.Parent.GetDownN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null)
                    {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.Y = position_.Y - 32 * MapSector.Ry;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.Y = 32 * MapSector.Ry - 1;
                    }
                }
                else if (Position.Y < 0)
                {
                    MapSector t = ms_.Parent.GetUpN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null)
                    {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.Y = position_.Y + 32 * MapSector.Ry;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.Y = 0;
                    }
                }
                if (Position.X >= 32 * MapSector.Rx)
                {
                    MapSector t = ms_.Parent.GetRightN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null)
                    {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.X = position_.X - 32 * MapSector.Rx;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.X = 32 * MapSector.Rx - 1;
                    }
                }
                else if (Position.X < 0)
                {
                    MapSector t = ms_.Parent.GetLeftN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null)
                    {
                        ms_.Creatures.Remove(this);
                        t.Creatures.Add(this);
                        position_.X = position_.X + 32 * MapSector.Rx;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.X = 0;
                    }
                }
            }

            creatureWorld.Translation = new Vector3(position_.X / 32f + ms.SectorOffsetX * 16, position_.Y / 32f + ms.SectorOffsetY * 16, 0) + new Vector3(0.5f, 0.5f, 0.5f);

            for (int i = 0; i < Buffs.Count; i++) {
                IBuff buff = Buffs[i];
                buff.Update(gt);
                if (!buff.Applied) {
                    Buffs.Remove(buff);
                }
            }
        }

        public void Shoot(float attackAngle)
        {
            BulletSystem.AddBullet(this, 50,
                         attackAngle +
                         MathHelper.ToRadians((((float)Settings.rnd.NextDouble() * 2f - 1) *
                                               Data.Accuracy / 10f)), Data.Damage);
        }

        public bool Skipp { get; set; }

        public virtual Vector3 WorldPosition() {
            return Position + new Vector3(-16, -32, 0) +
                   new Vector3(sectoroffset_.X*MapSector.Rx*32, sectoroffset_.Y*MapSector.Ry*32, 0);
        }

        public virtual void Kill(MapSector decalMs) {
            Hp = new Stat(0, 0);
            isDead = true;
            decalMs.AddDecal(new Particle(new Vector2(WorldPosition().X, WorldPosition().Y), 3) {Rotation = -3.14f/2, Life = new TimeSpan(0, 0, 1, 0)});
            if (OnDeath != null) {
                OnDeath(null, null);
            }
        }

        public void GiveDamage(float value, DamageType type) {
            hp_.Current -= value;
            EventLog.Add(string.Format("{0} получает {1} урона", Data.Name, value),
                         GlobalWorldLogic.CurrentTime, Color.Pink, LogEntityType.Damage);
            if (Hp.Current <= 0) {
                Kill(ms);
                EventLog.Add(string.Format("{0} УМИРАЕТ!", Data.Name.ToUpper()),
                             GlobalWorldLogic.CurrentTime, Color.Pink, LogEntityType.Dies);
            }
            var adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));

            ms.AddDecal(new Particle(new Vector2(WorldPosition().X, WorldPosition().Y) +adder, 3)
            {Rotation = Settings.rnd.Next()%360, Life = new TimeSpan(0, 0, 1, 0)});

            if (OnDamageRecieve != null) {
                OnDamageRecieve(null, null);
            }
        }

        public void SetPositionInBlocks(int x, int y) {
            position_ = new Vector3((x + 0.5f)*32, (y + 0.5f)*32 , 0);
        }

        /// <summary>
        ///     Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks() {
            Vector2 po = new Vector2(Position.X, Position.Y);
            po.X = po.X < 0 ? po.X/32 - 1 : po.X/32;
            po.Y = po.Y < 0 ? po.Y/32 - 1 : po.Y/32;
            return po;
        }

        public Ability GetAbility(AbilityType s)
        {
            return abilities_.List[s];
        }

        private void OrdersMaker(MapSector ms, GameTime gt) {
            double time = gt.ElapsedGameTime.TotalSeconds;
            switch (order_.Type) {
                case OrderType.Move: {
                    Vector2 wp = new Vector2(WorldPosition().X, WorldPosition().Y);
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

                    wp = new Vector2(WorldPosition().X, WorldPosition().Y);
                    if (Math.Abs(wp.X - order_.Point.X) < 10 && Math.Abs(wp.Y - order_.Point.Y) < 10)
                    {
                        IssureOrder();
                    }
                }
                    break;
                case OrderType.Attack: {
                    Vector2 wp = new Vector2(WorldPosition().X, WorldPosition().Y);
                    Vector2 mover = new Vector2(order_.Target.Position.X, order_.Target.Position.Y) -wp;
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

                    wp = new Vector2(WorldPosition().X, WorldPosition().Y);
                    if (Math.Abs(wp.X - order_.Target.Position.X) < 10 && Math.Abs(wp.Y - order_.Target.Position.Y) < 10 || (wp - new Vector2(order_.Target.Position.X, order_.Target.Position.Y)).Length() > 640)
                    {
                        IssureOrder();
                    }
                }
                    break;
                case OrderType.Wander: {
                    Vector2 wp = new Vector2(WorldPosition().X, WorldPosition().Y);
                    if (order_.Point.Y == 0)
                    {
                        order_.Point = new Vector2(wp.X + Settings.rnd.Next(-100, 100), wp.Y + Settings.rnd.Next(-100, 100));
                    }

                    Vector2 mover = order_.Point - wp;
                    float percenterMax = Data.Speed * Percenter * 0.25f;

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

                    wp = new Vector2(WorldPosition().X, WorldPosition().Y);
                    if (Math.Abs(wp.X - order_.Point.X) < 10 && Math.Abs(wp.Y - order_.Point.Y) < 10)
                    {
                        IssureOrder();
                    }
                }
                    break;
                case OrderType.Sleep:
                    if (CurrentOrder.Value <= 0) {
                        IssureOrder();
                    }
                    else {
                        CurrentOrder.Value -= gt.ElapsedGameTime.Milliseconds;
                    }
                    break;
            }
        }

        public static float GetLength(float x, float y){
            return new Vector2(x, y).Length();
        }
        public static Vector2 GetInDirection(float centerx, float centery, float targx, float targy, float distance)
        {
            var p2 = new Vector3(centerx, centery, 0);
            var p1 = new Vector3(targx, targy, 0);
            Ray a = new Ray(p1, -Vector3.Normalize(p2 - p1));
            var t = a.Position + a.Direction * distance;
            return new Vector2(t.X, t.Y);
        }

        public void Say(string s)
        {
            EventLog.Add(Data.Name+": \""+s+"\"", Color.LightGray, LogEntityType.Default);
        }
    }

    public delegate void CreatureScript(GameTime gt, MapSector ms_, Player hero, Creature target);
}