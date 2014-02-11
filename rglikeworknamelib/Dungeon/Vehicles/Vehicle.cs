using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Dungeon.Vehicles {
    [Serializable]
    public class Vehicle {
        public Collection<VehiclePart> Parts;
        public bool Prerendered;
        public float Roration;
        public bool Skipp;
        public float Vel;
        public Vector2 Acceleration;
        public float AngleAcc;
        private MapSector ms_;
        public Vector2 Position;

        public Vector2 SectorOffset;
        private RenderTarget2D tex_;

        public Vehicle(MapSector parent) {
            Parts = new Collection<VehiclePart>();
            ms_ = parent;
        }

        public void Draw(SpriteBatch sb, Vector2 camera) {
            sb.Draw(tex_, WorldPosition() - camera, null, Color.White, Roration, new Vector2(tex_.Width/2f, tex_.Height/2f),
                    1, SpriteEffects.None, 0);
        }

        public Vector2 GetSize() {
            var max = new Vector2();
            foreach (VehiclePart vehiclePart in Parts) {
                if (vehiclePart.Offset.X + vehiclePart.Source.Width > max.X) {
                    max.X = vehiclePart.Offset.X + vehiclePart.Source.Width;
                }
                if (vehiclePart.Offset.Y + vehiclePart.Source.Height > max.Y) {
                    max.Y = vehiclePart.Offset.Y + vehiclePart.Source.Height;
                }
            }
            return max;
        }

        public void Prerender(SpriteBatch sb, GraphicsDevice gd) {
            Vector2 f = GetSize();
            tex_ = new RenderTarget2D(gd, (int) f.X, (int) f.Y);
            gd.SetRenderTarget(tex_);
            gd.Clear(Color.Transparent);
            var atlases = Atlases.Instance;
            sb.Begin();
            foreach (VehiclePart vehiclePart in Parts) {
                sb.Draw(atlases.MajorAtlas, vehiclePart.Offset, vehiclePart.Source, vehiclePart.Color);
            }
            sb.End();
            gd.SetRenderTarget(null);
            Prerendered = true;
        }

        public void Update(GameTime gt, Player driver) {
            double time = gt.ElapsedGameTime.TotalSeconds;

            //driver.Position = WorldPosition();

            Acceleration = new Vector2(driver.Velocity.X, driver.Velocity.Y);

            if (!Skipp || !ms_.Ready) {
                Roration += Acceleration.X/500f*(-Vel/10);
                AngleAcc /= 1.1f;

                Vel = Vector2.Lerp(new Vector2(Vel, Vel), Acceleration, (float) time).Y;
                float mover = (Vel*(float) time*40);
                Position.X += (float) (Math.Sin(-Roration - 3.14f/4f)*mover + Math.Cos(-Roration - 3.14f/4f)*mover);
                Position.Y += (float) (Math.Cos(-Roration - 3.14f/4f)*mover - Math.Sin(-Roration - 3.14f/4f)*mover);
                Acceleration /= 1.1f;

                AchievementDataBase.Stat["drive"].Count += Math.Abs(mover/32);

                if (Position.Y >= 32*MapSector.Ry) {
                    MapSector t = ms_.Parent.GetDownN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Vehicles.Remove(this);
                        ms_ = t;
                        t.Vehicles.Add(this);
                        Position.Y = Position.Y - 32*MapSector.Ry;
                        SectorOffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        Position.Y = 32*MapSector.Ry - 1;
                    }
                }
                else if (Position.Y < 0) {
                    MapSector t = ms_.Parent.GetUpN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Vehicles.Remove(this);
                        ms_ = t;
                        t.Vehicles.Add(this);
                        Position.Y = Position.Y + 32*MapSector.Ry;
                        SectorOffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        Position.Y = 0;
                    }
                }
                if (Position.X >= 32*MapSector.Rx) {
                    MapSector t = ms_.Parent.GetRightN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Vehicles.Remove(this);
                        ms_ = t;
                        t.Vehicles.Add(this);
                        Position.X = Position.X - 32*MapSector.Rx;
                        SectorOffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        Position.X = 32*MapSector.Rx - 1;
                    }
                }
                else if (Position.X < 0) {
                    MapSector t = ms_.Parent.GetLeftN(ms_.SectorOffsetX, ms_.SectorOffsetY);
                    if (t != null) {
                        ms_.Vehicles.Remove(this);
                        ms_ = t;
                        t.Vehicles.Add(this);
                        Position.X = Position.X + 32*MapSector.Rx;
                        SectorOffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        Position.X = 0;
                    }
                }
            }
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

        public Vector2 WorldPosition() {
            return Position + new Vector2(-16, -32) +
                   new Vector2(SectorOffset.X*MapSector.Rx*32, SectorOffset.Y*MapSector.Ry*32);
        }
    }

    public class TestVehicle : Vehicle {
        public TestVehicle(MapSector parent) : base(parent) {
            var col = new Color(Settings.rnd.Next(0, 255), Settings.rnd.Next(0, 255), Settings.rnd.Next(0, 255));
            Parts.Add(new VehiclePart {MTex = "wheel1", Offset = new Vector2(0, 32)});
            Parts.Add(new VehiclePart {MTex = "wheel1", Offset = new Vector2(0, 32*3)});
            Parts.Add(new VehiclePart {MTex = "wheel1", Offset = new Vector2(104, 32)});
            Parts.Add(new VehiclePart {MTex = "wheel1", Offset = new Vector2(104, 32*3)});

            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(52, 0)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(52, 32*1)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(52, 32*2)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(52, 32*3)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(52, 32*4)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(20, 32*1)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(20, 32*2)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(20, 32*3)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(84, 32*1)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(84, 32*2)});
            Parts.Add(new VehiclePart {MTex = "plate1", Offset = new Vector2(84, 32*3)});

            Parts.Add(new VehiclePart {MTex = "engine1", Offset = new Vector2(32, 0)});
            Parts.Add(new VehiclePart {MTex = "seat1", Offset = new Vector2(20, 32)});
            Parts.Add(new VehiclePart {MTex = "seat1", Offset = new Vector2(64, 32)});

            Parts.Add(new VehiclePart {MTex = "seat1", Offset = new Vector2(20, 32*3)});
            Parts.Add(new VehiclePart {MTex = "seat1", Offset = new Vector2(84, 32*3)});
            Parts.Add(new VehiclePart {MTex = "seat1", Offset = new Vector2(52, 32*3)});

            Parts.Add(new VehiclePart {MTex = "plate2", Offset = new Vector2(20, 0), Color = col});
            Parts.Add(new VehiclePart {MTex = "plate3", Offset = new Vector2(84, 0), Color = col});
            Parts.Add(new VehiclePart {MTex = "plate4", Offset = new Vector2(52, 0), Color = col});
        }
    }

    public class VehiclePart {
        public Color Color = Color.White;
        public float Depth;
        public Vector2 Offset;
        public Rectangle Source;
        private string mTex_;

        public string MTex {
            get { return mTex_; }
            set {
                Source = GetSource(value);
                mTex_ = value;
            }
        }

        public Rectangle GetSource(string s) {
            if (s == null) {
                return new Rectangle(0, 0, 0, 0);
            }
            int index = Atlases.Instance.MajorIndexes[s];
            return new Rectangle(index%32*32, index/32*32, 32, 32);
        }
    }
}