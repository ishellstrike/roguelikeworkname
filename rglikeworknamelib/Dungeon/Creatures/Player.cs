using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Creatures {
    public class Player : ShootingCreature {
        private readonly SpriteBatch sb_;

        public SpriteFont Font;


        public Stat Heat = new Stat(100);
        public Stat Hunger = new Stat(100);
        public IItem ItemAmmo;
        public IItem ItemGun;
        public IItem ItemMeele;
        public Stat Morale = new Stat(100);

        public PerksSystem Perks;
        public Stat Sleep = new Stat(100);
        public Texture2D Tex;
        public Stat Thirst = new Stat(100);
        public Collection<IItem> Weared;
        public InventorySystem Inventory;

        /// <summary>
        ///     Experience for abilities. From rest
        /// </summary>
        public int XpPool;

        private TimeSpan secShoot_ = TimeSpan.Zero;

        public Player(SpriteBatch sb, Texture2D tex, SpriteFont font) {
            sb_ = sb;
            Font = font;
            Tex = tex;
            Position = new Vector2(1, 1);
            Perks = new PerksSystem(this);
            Weared = new Collection<IItem>();
        }

        public Vector2 CurrentActiveRoom { get; set; }

        public int MaxWeight {
            get { return 1000; }
        }

        public int MaxVolume {
            get { return 1000; }
        }


        public void EquipItem(IItem item, InventorySystem ins) {
            Settings.InventoryUpdate = true;
            switch (item.Data.SType) {
                case ItemType.Wear:
                    EquipWear(item, ins);
                    break;
                case ItemType.Gun:
                    EquipSloted(item, ins, ref ItemGun);
                    break;
                case ItemType.Meele:
                    EquipSloted(item, ins, ref ItemMeele);
                    break;
                case ItemType.Ammo:
                    EquipSloted(item, ins, ref ItemAmmo);
                    break;
            }
            if (OnUpdatedEquip != null) {
                OnUpdatedEquip(null, null);
            }
        }

        private void EquipWear(IItem i, InventorySystem ins) {
            if (i != null) {
                //if (Weared.Contains(i))
                //{
                //    ins.AddItem(ite);
                //    EventLog.Add(string.Format("Вы убрали в инвентарь {0}", ite.Data.Name), GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Equip);
                //    foreach (var buff in ite.Buffs.Where(buff => Buffs.Contains(buff)))
                //    {
                //        Buffs.Remove(buff);
                //        buff.RemoveFromTarget(this);
                //    }
                //}
                Weared.Add(i);
                if (ins.ContainsItem(i)) {
                    ins.RemoveItem(i);
                }
                EventLog.Add(string.Format("Вы надели {0}", i.Data.Name), GlobalWorldLogic.CurrentTime, Color.Yellow,
                             LogEntityType.Equip);
                foreach (IBuff buff in i.Buffs) {
                    Buffs.Add(buff);
                    buff.ApplyToTarget(this);
                }
            }
        }

        private void EquipSloted(IItem i, InventorySystem ins, ref IItem ite) {
            if (i != null) {
                if (ite != null) {
                    ins.AddItem(ite);
                    EventLog.Add(string.Format("Вы убрали в инвентарь {0}", ite.Data.Name), GlobalWorldLogic.CurrentTime,
                                 Color.Yellow, LogEntityType.Equip);
                    foreach (IBuff buff in ite.Buffs.Where(buff => Buffs.Contains(buff))) {
                        Buffs.Remove(buff);
                        buff.RemoveFromTarget(this);
                    }
                }
                ite = i;
                if (ins.ContainsItem(i)) {
                    ins.RemoveItem(i);
                }
                EventLog.Add(string.Format("Вы экипировали {0}", i.Data.Name), GlobalWorldLogic.CurrentTime,
                             Color.Yellow, LogEntityType.Equip);
                foreach (IBuff buff in i.Buffs) {
                    Buffs.Add(buff);
                    buff.ApplyToTarget(this);
                }
            }
        }

        public void UnEquipItem(Item i) {
        }

        public void Accelerate(Vector2 ac) {
            Velocity += ac;
        }

        public new void GiveDamage(float value, DamageType type, MapSector ms) {
            Hp = new Stat(Hp.Current - value, Hp.Max);
            EventLog.Add(string.Format("Вы получаете {0} урона", value), GlobalWorldLogic.CurrentTime, Color.Red,
                         LogEntityType.SelfDamage);
            if (Hp.Current <= 0 && !isDead) {
                Kill(ms);
                EventLog.Add(string.Format("Вы умерли! GAME OVER!"), GlobalWorldLogic.CurrentTime, Color.Red,
                             LogEntityType.Dies);
            }
            var adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));
            ms.AddDecal(new Particle(WorldPosition() + adder, 3) {
                Rotation = Settings.rnd.Next()%360,
                Life = new TimeSpan(0, 0, 1, 0)
            });
            Achievements.Stat["takedmg"].Count += value;
        }

        public override void Update(GameTime gt, MapSector ms, Player hero) {
            if (ms != null) {
                var time = (float) gt.ElapsedGameTime.TotalSeconds;

                Vector2 tpos = Position;
                tpos.X += Velocity.X;
                Vector2 tpos2 = Position;
                tpos2.Y += Velocity.Y;

                var a = (int) (tpos.X/32.0);
                var b = (int) (tpos.Y/32.0);

                var c = (int) (tpos2.X/32.0);
                var d = (int) (tpos2.Y/32.0);

                if (tpos.X < 0) {
                    a--;
                }
                if (tpos.Y < 0) {
                    b--;
                }
                if (tpos2.X < 0) {
                    c--;
                }
                if (tpos2.Y < 0) {
                    d--;
                }

                if (!ms.Parent.IsWalkable(a, b)) {
                    if (!Settings.Noclip) {
                        Velocity.X = 0;
                    }
                    IBlock key = ms.Parent.GetBlock(a, b);
                    if (key == null) {
                        return;
                    }
                    if (key.Data.SmartAction == SmartAction.ActionOpenClose) {
                        ms.Parent.OpenCloseDoor(a, b);
                    }
                }
                if (!ms.Parent.IsWalkable(c, d)) {
                    if (!Settings.Noclip) {
                        Velocity.Y = 0;
                    }
                    IBlock block = ms.Parent.GetBlock(c, d);
                    if (block != null && block.Data.SmartAction == SmartAction.ActionOpenClose) {
                        ms.Parent.OpenCloseDoor(c, d);
                    }
                }

                Vector2 perem = Velocity*time*20;
                Position += perem; /////////
                float meters = (perem/32).Length();
                Achievements.Stat["walk"].Count += meters;
                Abilities.list["atlet"].XpCurrent += meters/300.0;

                if (time != 0) {
                    Velocity /= Settings.H()/time;
                }

                TimeSpan elapsetGWL = GlobalWorldLogic.Elapse;
                Hunger.Current -= (float) elapsetGWL.TotalDays/4*100;
                Thirst.Current -= (float) elapsetGWL.TotalDays*100;
                Sleep.Current -= (float) elapsetGWL.TotalDays/1.5f*100;

                secShoot_ += gt.ElapsedGameTime;

                for (int i = 0; i < Buffs.Count; i++) {
                    IBuff buff = Buffs[i];
                    buff.Update(gt);
                    if (!buff.Applied) {
                        Buffs.Remove(buff);
                    }
                }
            }
        }

        public void Draw(GameTime gt, Vector2 cam) {
            Vector2 position = Position - cam;
            var origin = new Vector2(Tex.Width/2f, Tex.Height);
            sb_.Draw(Tex, position, null, Color.White, 0, origin, 1,
                     SpriteEffects.None, 1);
            foreach (var dress in Weared) {
                if (!string.IsNullOrEmpty(dress.Data.Dress)) {
                    sb_.Draw(Atlases.DressAtlas[dress.Data.Dress], position, null, Color.White, 0, origin, 1,
                             SpriteEffects.None, 1);
                }
            }

            if (Settings.DebugInfo) {
                sb_.DrawString(Font,
                               string.Format("Po:{0}{5}Hu:{1} Th:{2}{5}Sl:{3} He:{4}", Position, Hunger, Thirst, Sleep,
                                             Heat, Environment.NewLine),
                               new Vector2(32 + Position.X - cam.X, -32 + Position.Y - cam.Y), Color.White);
            }
        }

        public event EventHandler OnUpdatedEquip;
        public event EventHandler OnShoot;

        public void TryShoot(BulletSystem bs, InventorySystem ins, float playerSeeAngle) {
            if (ItemGun != null) {
                if (secShoot_.TotalMilliseconds > ItemGun.Data.FireRate) {
                    if ((ItemGun.Data.Ammo != null && ItemAmmo != null && ItemAmmo.Id == ItemGun.Data.Ammo) ||
                        ItemGun.Data.Ammo == null) {
                        int dam = ItemGun != null ? ItemGun.Data.Damage : 0;
                        bs.AddBullet(this, 50,
                                     playerSeeAngle +
                                     MathHelper.ToRadians((((float) Settings.rnd.NextDouble()*2f - 1)*
                                                           ItemGun.Data.Accuracy/10f)), dam);
                        secShoot_ = TimeSpan.Zero;
                        if (ItemAmmo != null) {
                            ItemAmmo.Count--;
                            if (ItemAmmo.Count <= 0) {
                                ItemAmmo = null;
                            }
                        }
                        Achievements.Stat["ammouse"].Count++;
                        Abilities.list["shoot"].XpCurrent += 0.2;
                        if (OnShoot != null) {
                            OnShoot(null, null);
                        }
                    }
                    else {
                        if (secShoot_.TotalMilliseconds > 1000) {
                            EquipItem(ins.TryGetId(ItemGun.Data.Ammo), ins);
                            EventLog.Add("No ammo", GlobalWorldLogic.CurrentTime, Color.Yellow,
                                         LogEntityType.NoAmmoWeapon);
                            secShoot_ = TimeSpan.Zero;
                        }
                    }
                }
            }
            else {
                if (secShoot_.TotalMilliseconds > 1000) {
                    EventLog.Add("No weapon", GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.NoAmmoWeapon);
                    secShoot_ = TimeSpan.Zero;
                }
            }
        }

        public bool EatItem(Item selectedItem) {
            //25 -- 2500 ml per day / 100 percent
            Hunger.Current += selectedItem.Data.NutCal/25f/selectedItem.Data.Doses;
            if (Hunger.Current > Hunger.Max) {
                Hunger.Current = Hunger.Max;
                EventLog.Add(string.Format("Вы не смогли съесть {0} целиком", selectedItem),
                             GlobalWorldLogic.CurrentTime, Color.Orange, LogEntityType.Consume);
            }
            Thirst.Current += selectedItem.Data.NutH2O/25f/selectedItem.Data.Doses;
            if (Thirst.Current > Hunger.Max) {
                Thirst.Current = Thirst.Max;
                EventLog.Add(string.Format("Вы не смогли выпить {0} целиком", selectedItem),
                             GlobalWorldLogic.CurrentTime, Color.Orange, LogEntityType.Consume);
            }
            return true;
        }

        public void Load() {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("caracter.rlp"))) {
                var binaryFormatter = new BinaryFormatter();

                var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("caracter.rlp"),
                                                FileMode.Open);
                var gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                Hp = (Stat) binaryFormatter.Deserialize(gZipStream);
                Position = (Vector2) binaryFormatter.Deserialize(gZipStream);
                Perks = (PerksSystem) binaryFormatter.Deserialize(gZipStream);
                Perks.Owner = this;
                Weared = (Collection<IItem>) binaryFormatter.Deserialize(gZipStream);
                foreach (Item item in Weared) {
                    foreach (IBuff buff in item.Buffs) {
                        buff.Target = this;
                    }
                    item.OnLoad();
                }
                var t = (bool) binaryFormatter.Deserialize(gZipStream);
                if (t) {
                    ItemAmmo = (Item) binaryFormatter.Deserialize(gZipStream);
                    foreach (IBuff buff in ItemAmmo.Buffs) {
                        buff.Target = this;
                    }
                    ItemAmmo.OnLoad();
                }

                t = (bool) binaryFormatter.Deserialize(gZipStream);
                if (t) {
                    ItemGun = (Item) binaryFormatter.Deserialize(gZipStream);
                    foreach (IBuff buff in ItemGun.Buffs) {
                        buff.Target = this;
                    }
                    ItemGun.OnLoad();
                }

                t = (bool) binaryFormatter.Deserialize(gZipStream);
                if (t) {
                    ItemMeele = (Item) binaryFormatter.Deserialize(gZipStream);
                    foreach (IBuff buff in ItemMeele.Buffs) {
                        buff.Target = this;
                    }
                    ItemMeele.OnLoad();
                }

                Hunger = (Stat) binaryFormatter.Deserialize(gZipStream);
                Thirst = (Stat) binaryFormatter.Deserialize(gZipStream);
                Heat = (Stat) binaryFormatter.Deserialize(gZipStream);
                Sleep = (Stat) binaryFormatter.Deserialize(gZipStream);
                Morale = (Stat) binaryFormatter.Deserialize(gZipStream);
                XpPool = (int) binaryFormatter.Deserialize(gZipStream);
                gZipStream.Close();
                gZipStream.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        public void Save() {
            var binaryFormatter = new BinaryFormatter();
            var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("caracter.rlp"),
                                            FileMode.Create);
            var gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
            binaryFormatter.Serialize(gZipStream, Hp);
            binaryFormatter.Serialize(gZipStream, Position);
            binaryFormatter.Serialize(gZipStream, Perks);
            binaryFormatter.Serialize(gZipStream, Weared);
            bool t = ItemAmmo != null;
            binaryFormatter.Serialize(gZipStream, t);
            if (t) {
                binaryFormatter.Serialize(gZipStream, ItemAmmo);
            }
            t = ItemGun != null;
            binaryFormatter.Serialize(gZipStream, t);
            if (t) {
                binaryFormatter.Serialize(gZipStream, ItemGun);
            }
            t = ItemMeele != null;
            binaryFormatter.Serialize(gZipStream, t);
            if (t) {
                binaryFormatter.Serialize(gZipStream, ItemMeele);
            }
            binaryFormatter.Serialize(gZipStream, Hunger);
            binaryFormatter.Serialize(gZipStream, Thirst);
            binaryFormatter.Serialize(gZipStream, Heat);
            binaryFormatter.Serialize(gZipStream, Sleep);
            binaryFormatter.Serialize(gZipStream, Morale);
            binaryFormatter.Serialize(gZipStream, XpPool);
            gZipStream.Close();
            gZipStream.Dispose();
            fileStream.Close();
            fileStream.Dispose();
        }
    }
}