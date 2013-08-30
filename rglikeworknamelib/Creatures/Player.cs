using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
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
    public class Player : Creature {
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

        public List<IBuff> buffs = new List<IBuff>(); 

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
            switch (ItemDataBase.data[i.Id].SType) {
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
            }
        }

        private void EquipExact(Item i, InventorySystem ins, ref Item ite) {
            if (i != null) {
                if (ite != null) {
                    ins.items.Add(ite);
                    foreach (var buff in ite.Buffs.Where(buff => buffs.Contains(buff))) {
                        buffs.Remove(buff);
                        buff.RemoveFromTarget(this);
                    }
                }
                ite = i;
                if (ins.items.Contains(i)) {
                    ins.items.Remove(i);
                }
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

        public void GiveDamage(float value, DamageType type, MapSector ms)
        {

            Hp = new Stat(Hp.Current-value, Hp.Max);
            EventLog.Add(string.Format("Вы получаете {0} урона", value), GlobalWorldLogic.CurrentTime, Color.Red, LogEntityType.SelfDamage);
            if (Hp.Current <= 0 && !isDead)
            {
                Kill(ms);
                EventLog.Add(string.Format("Вы умерли! GAME OVER!"), GlobalWorldLogic.CurrentTime, Color.Red, LogEntityType.Dies);
            }
            Vector2 adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));
            ms.AddDecal(new Particle(WorldPosition() + adder, 3) { Rotation = Settings.rnd.Next() % 360, Life = new TimeSpan(0, 0, 1, 0) });
        }

        public void Update(GameTime gt, GameLevel gl) {
            var time = (float) gt.ElapsedGameTime.TotalSeconds;

            var tpos = Position; 
            tpos.X += Velocity.X;
            var tpos2 = Position;
            tpos2.Y += Velocity.Y;

            int a = (int) (tpos.X/32.0);
            int b = (int) (tpos.Y/32.0);

            int c = (int)(tpos2.X / 32.0);
            int d = (int)(tpos2.Y / 32.0);

            if (tpos.X < 0) a--;
            if (tpos.Y < 0) b--;
            if (tpos2.X < 0) c--;
            if (tpos2.Y < 0) d--;

            if (!gl.IsWalkable(a, b))
            {
                Velocity.X = 0;
                if (BlockDataBase.Data[gl.GetBlock(a, b).Id].SmartAction == SmartAction.ActionOpenClose)
                {
                    gl.OpenCloseDoor(a, b);
                }
            }
            if (!gl.IsWalkable(c, d))
            {
                Velocity.Y = 0;
                if (BlockDataBase.Data[gl.GetBlock(c, d).Id].SmartAction == SmartAction.ActionOpenClose)
                {
                    gl.OpenCloseDoor(c, d);
                }
            }

            Position += Velocity * time * 20;/////////

            if (time != 0) {
                Velocity /= Settings.H() / time;
            }
        }

        public void Draw(GameTime gt, Vector2 cam) {
            var position = Position - cam;
            var origin = new Vector2(Tex.Width / 2, Tex.Height);
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
    }
}