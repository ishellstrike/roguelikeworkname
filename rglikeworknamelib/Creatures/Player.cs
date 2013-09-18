using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Creatures {

    [Serializable]
    public struct Dress {
        public string id;
        public Color col;
        public Dress(string i , Color c) {
            id = i;
            col = c;
        }
    }
    public class Player : ShootingCreature {
        private readonly SpriteBatch sb_;
        public Texture2D Tex;
        public SpriteFont Font;

        public Dress DressHat = new Dress("haer1", Color.White);
        public Dress DressTshort = new Dress("t-short1", Color.DarkRed);
        public Dress DressPants = new Dress("pants1", Color.DarkBlue);

        public Item ItemHat;
        public Item ItemGlaces;
        public Item ItemHelmet;
        public Item ItemChest;
        public Item ItemShirt;
        public Item ItemPants;
        public Item ItemGloves;
        public Item ItemBoots;
        public Item ItemGun;
        public Item ItemMeele;
        public Item ItemAmmo;
        public Item ItemBag;

        /// <summary>
        /// Experience for other abilities. From monsters
        /// </summary>
        public int XpPool;

        /// <summary>
        /// Experience for battle abilities. From rest
        /// </summary>
        public int RestPool; 

        public Player(SpriteBatch sb, Texture2D tex, SpriteFont font) {
            sb_ = sb;
            Font = font;
            Tex = tex;
            Position = new Vector2(1, 1);
        }

        public Player()
        {
        }

        public void EquipItem(Item i, InventorySystem ins) {
            switch (ItemDataBase.Data[i.Id].SType) {
                    case ItemType.Hat:
                        EquipExact(i, ins, ref ItemHat);
                    break;
                    case ItemType.Pants:
                        EquipExact(i, ins, ref ItemPants);
                    break;
                    case ItemType.Shirt:
                        EquipExact(i, ins, ref ItemShirt);
                    break;
                    case ItemType.Gun:
                        EquipExact(i, ins, ref ItemGun);
                    break;
                    case ItemType.Meele:
                        EquipExact(i, ins, ref ItemMeele);
                    break;
                    case ItemType.Ammo:
                    EquipExact(i, ins, ref ItemAmmo);
                    break;
            }
        }

        private void EquipExact(Item i, InventorySystem ins, ref Item ite) {
            if (i != null) {
                if (ite != null) {
                    ins.AddItem(ite);
                    EventLog.Add(string.Format("Вы убрали в инвентарь {0}", ItemDataBase.Data[ite.Id].Name), GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Equip);
                    foreach (var buff in ite.Buffs.Where(buff => buffs.Contains(buff))) {
                        buffs.Remove(buff);
                        buff.RemoveFromTarget(this);
                    }
                }
                ite = i;
                if (ins.ContainsItem(i)) {
                    ins.RemoveItem(i);
                }
                EventLog.Add(string.Format("Вы экипировали {0}", ItemDataBase.Data[i.Id].Name), GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Equip);
                foreach (var buff in i.Buffs)
                {
                    buffs.Add(buff);
                    buff.ApplyToTarget(this);
                }
            }
        }

        public void UnEquipItem(Item i) {
            
        }

        public Vector2 CurrentActiveRoom { get; set; }

        public void Accelerate(Vector2 ac) {
            Velocity += ac;
        }

        public new void GiveDamage(float value, DamageType type, MapSector ms)
        {

            Hp = new Stat(Hp.Current-value, Hp.Max);
            EventLog.Add(string.Format("Вы получаете {0} урона", value), GlobalWorldLogic.CurrentTime, Color.Red, LogEntityType.SelfDamage);
            if (Hp.Current <= 0 && !isDead)
            {
                Kill(ms);
                EventLog.Add(string.Format("Вы умерли! GAME OVER!"), GlobalWorldLogic.CurrentTime, Color.Red, LogEntityType.Dies);
            }
            var adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));
            ms.AddDecal(new Particle(WorldPosition() + adder, 3) { Rotation = Settings.rnd.Next() % 360, Life = new TimeSpan(0, 0, 1, 0) });
            Achievements.Stat["takedmg"].Count += value;
        }

        public override void Update(GameTime gt, MapSector ms, Player hero)
        {
            var time = (float)gt.ElapsedGameTime.TotalSeconds;

            var tpos = Position;
            tpos.X += Velocity.X;
            var tpos2 = Position;
            tpos2.Y += Velocity.Y;

            int a = (int)(tpos.X / 32.0);
            int b = (int)(tpos.Y / 32.0);

            int c = (int)(tpos2.X / 32.0);
            int d = (int)(tpos2.Y / 32.0);

            if (tpos.X < 0) a--;
            if (tpos.Y < 0) b--;
            if (tpos2.X < 0) c--;
            if (tpos2.Y < 0) d--;

            if (!ms.Parent.IsWalkable(a, b))
            {
                Velocity.X = 0;
                if (BlockDataBase.Data[ms.Parent.GetBlock(a, b).Id].SmartAction == SmartAction.ActionOpenClose)
                {
                    ms.Parent.OpenCloseDoor(a, b);
                }
            }
            if (!ms.Parent.IsWalkable(c, d))
            {
                Velocity.Y = 0;
                if (BlockDataBase.Data[ms.Parent.GetBlock(c, d).Id].SmartAction == SmartAction.ActionOpenClose)
                {
                    ms.Parent.OpenCloseDoor(c, d);
                }
            }

            Position += Velocity * time * 20; /////////

            if (time != 0)
            {
                Velocity /= Settings.H() / time;
            }

            secShoot_ += gt.ElapsedGameTime;

            foreach (var buff in buffs) {
                buff.Update(gt);
            }
        }

        public void Draw(GameTime gt, Vector2 cam) {
            var position = Position - cam;
            var origin = new Vector2(Tex.Width / 2f, Tex.Height);
            sb_.Draw(Tex, position, null, Color.White, 0, origin, 1,
                     SpriteEffects.None, 1);
            sb_.Draw(Atlases.DressAtlas[DressHat.id], position, null, DressHat.col, 0, origin, 1, SpriteEffects.None, 1);
            sb_.Draw(Atlases.DressAtlas[DressPants.id], position, null, DressPants.col, 0, origin, 1, SpriteEffects.None, 1);
            sb_.Draw(Atlases.DressAtlas[DressTshort.id], position, null, DressTshort.col, 0, origin, 1, SpriteEffects.None, 1);

            if (Settings.DebugInfo)
            {
                sb_.DrawString(Font, string.Format("{0}", Position), new Vector2(32 + Position.X - cam.X, -32 + Position.Y - cam.Y), Color.White);
            }
        }

        private TimeSpan secShoot_ = TimeSpan.Zero;
        public void TryShoot(BulletSystem bs, float playerSeeAngle) {
            if (ItemGun != null) {
                if (secShoot_.TotalMilliseconds > ItemDataBase.Data[ItemGun.Id].FireRate) {
                    if ((ItemDataBase.Data[ItemGun.Id].Ammo != null && ItemAmmo != null && ItemAmmo.Id == ItemDataBase.Data[ItemGun.Id].Ammo) || ItemDataBase.Data[ItemGun.Id].Ammo == null)
                    {
                        bs.AddBullet(this, 50,
                                      playerSeeAngle +
                                      MathHelper.ToRadians((((float) Settings.rnd.NextDouble()*2f - 1)*
                                                            ItemDataBase.Data[ItemGun.Id].Accuracy/10f)));
                        secShoot_ = TimeSpan.Zero;
                        if (ItemAmmo != null) {
                            ItemAmmo.Count--;
                            if(ItemAmmo.Count <= 0) {
                                ItemAmmo = null;
                            }
                        }
                        Achievements.Stat["ammouse"].Count++;
                    } else {
                        if (secShoot_.TotalMilliseconds > 1000) {
                            EventLog.Add("No ammo", GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.NoAmmoWeapon);
                            secShoot_ = TimeSpan.Zero;
                        }
                    }
                }
            } else {
                if (secShoot_.TotalMilliseconds > 1000) {
                    EventLog.Add("No weapon", GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.NoAmmoWeapon);
                    secShoot_ = TimeSpan.Zero;
                }
            }
        }
    }
}