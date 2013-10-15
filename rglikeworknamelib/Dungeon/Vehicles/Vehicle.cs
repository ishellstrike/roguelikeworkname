using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Dungeon.Vehicles
{
    public class Vehicle {
        public Collection<VehiclePart> Parts;
        public Vector2 position_;
        public Vector2 acceleration;
        public float angle_acc;

        public Vector2 sectoroffset_;
        public float Roration;
        private RenderTarget2D tex;
        public bool Prerendered;

        public Vehicle(MapSector parent) {
            Parts = new Collection<VehiclePart>();
            ms = parent;
        }

        public void Draw(SpriteBatch sb, Vector2 camera) {
            sb.Draw(tex, WorldPosition() - camera, null, Color.White, Roration, new Vector2(tex.Width/2f, tex.Height/2f), 1, SpriteEffects.None, 0 );
        }

        public Vector2 GetSize() {
            Vector2 max = new Vector2();
            foreach (var vehiclePart in Parts) {
                if (vehiclePart.Offset.X + vehiclePart.Source.Width > max.X) max.X = vehiclePart.Offset.X + vehiclePart.Source.Width;
                if (vehiclePart.Offset.Y + vehiclePart.Source.Height > max.Y) max.Y = vehiclePart.Offset.Y + vehiclePart.Source.Height;
            }
            return max;
        }

        public void Prerender(SpriteBatch sb, GraphicsDevice gd) {
            var f = GetSize();
            tex = new RenderTarget2D(gd, (int) f.X, (int) f.Y);
            gd.SetRenderTarget(tex);
            gd.Clear(Color.Transparent);
            sb.Begin();
            foreach (var vehiclePart in Parts) {
                sb.Draw(Atlases.VehicleAtlas, vehiclePart.Offset, vehiclePart.Source, vehiclePart.Color);
            }
            sb.End();
            gd.SetRenderTarget(null);
            Prerendered = true;
        }

        private MapSector ms;
        public float Vel;

        public void Update(GameTime gt, Player driver)
        {
            var time = gt.ElapsedGameTime.TotalSeconds;

            driver.Position = WorldPosition();

            acceleration = driver.Velocity;

            if (!Skipp || !ms.Ready)
            {

                Roration += acceleration.X/500f*(-Vel/10);
                angle_acc /= 1.1f;

                Vel = Vector2.Lerp(new Vector2(Vel, Vel), acceleration, (float)time).Y;
                var mover = (Vel*(float) time*40);
                position_.X += (float)(Math.Sin(-Roration - 3.14f / 4f) * mover + Math.Cos(-Roration - 3.14f / 4f) * mover);
                position_.Y += (float)(Math.Cos(-Roration - 3.14f / 4f) * mover - Math.Sin(-Roration - 3.14f / 4f) * mover);
                acceleration /= 1.1f;

                if (position_.Y >= 32 * MapSector.Ry)
                {
                    var t = ms.Parent.GetDownN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null)
                    {
                        ms.Vehicles.Remove(this);
                        ms = t;
                        t.Vehicles.Add(this);
                        position_.Y = position_.Y - 32 * MapSector.Ry;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.Y = 32 * MapSector.Ry - 1;
                    }
                }
                else if (position_.Y < 0)
                {
                    var t = ms.Parent.GetUpN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null)
                    {
                        ms.Vehicles.Remove(this);
                        ms = t;
                        t.Vehicles.Add(this);
                        position_.Y = position_.Y + 32 * MapSector.Ry;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.Y = 0;
                    }
                }
                if (position_.X >= 32 * MapSector.Rx)
                {
                    var t = ms.Parent.GetRightN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null)
                    {
                        ms.Vehicles.Remove(this);
                        ms = t;
                        t.Vehicles.Add(this);
                        position_.X = position_.X - 32 * MapSector.Rx;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.X = 32 * MapSector.Rx - 1;
                    }
                }
                else if (position_.X < 0)
                {

                    var t = ms.Parent.GetLeftN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null)
                    {
                        ms.Vehicles.Remove(this);
                        ms = t;
                        t.Vehicles.Add(this);
                        position_.X = position_.X + 32 * MapSector.Rx;
                        sectoroffset_ = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else
                    {
                        position_.X = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetWorldPositionInBlocks()
        {
            var po = WorldPosition();
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        public Vector2 WorldPosition()
        {
            return position_ + new Vector2(-16, -32) +
                   new Vector2(sectoroffset_.X * MapSector.Rx * 32, sectoroffset_.Y * MapSector.Ry * 32);
        }

        public bool Skipp;
    }

    public class TestVehicle : Vehicle {
        public TestVehicle(MapSector parent) : base(parent) {
            Color col = new Color(Settings.rnd.Next(0, 255), Settings.rnd.Next(0, 255), Settings.rnd.Next(0, 255));
            Parts.Add(new VehiclePart {MTex = "wheel1", Offset = new Vector2(0,32)});
            Parts.Add(new VehiclePart { MTex = "wheel1", Offset = new Vector2(0, 32*3) });
            Parts.Add(new VehiclePart { MTex = "wheel1", Offset = new Vector2(104, 32) });
            Parts.Add(new VehiclePart { MTex = "wheel1", Offset = new Vector2(104, 32*3) });

            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(52, 0) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(52, 32 * 1) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(52, 32 * 2) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(52, 32 * 3) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(52, 32 * 4) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(20, 32 * 1) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(20, 32 * 2) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(20, 32 * 3) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(84, 32 * 1) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(84, 32 * 2) });
            Parts.Add(new VehiclePart { MTex = "plate1", Offset = new Vector2(84, 32 * 3) });

            Parts.Add(new VehiclePart { MTex = "engine1", Offset = new Vector2(32, 0) });
            Parts.Add(new VehiclePart { MTex = "seat1", Offset = new Vector2(20, 32) });
            Parts.Add(new VehiclePart { MTex = "seat1", Offset = new Vector2(64, 32) });

            Parts.Add(new VehiclePart { MTex = "seat1", Offset = new Vector2(20, 32 * 3) });
            Parts.Add(new VehiclePart { MTex = "seat1", Offset = new Vector2(84, 32 * 3) });
            Parts.Add(new VehiclePart { MTex = "seat1", Offset = new Vector2(52, 32 * 3) });

            Parts.Add(new VehiclePart { MTex = "plate2", Offset = new Vector2(20, 0), Color = col });
            Parts.Add(new VehiclePart { MTex = "plate3", Offset = new Vector2(84, 0), Color = col });
            Parts.Add(new VehiclePart { MTex = "plate4", Offset = new Vector2(52, 0), Color = col });
        }
    }

    public class VehiclePart {
        public Vector2 Offset;
        public Rectangle Source;
        public Color Color = Color.White;
        private string mTex_;
        public string MTex {
            get { return mTex_; }
            set
            {
                Source = GetSource(value); 
                mTex_ = value;
            }
        }

        public float Depth;

        public Rectangle GetSource(string s)
        {
            if (s == null) return new Rectangle(0, 0, 0, 0);
            int index = Atlases.VehicleIndexes[s];
            return new Rectangle(index % 32 * 32, index / 32 * 32, 32, 32);
        }
    }
}
