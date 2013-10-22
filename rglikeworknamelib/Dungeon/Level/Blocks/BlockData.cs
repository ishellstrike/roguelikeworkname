using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockData {
        public string AfterDeathId;
        public string[] AlterMtex;

        public float Damage;
        public string Description;

        public float Hp;
        public bool IsDestructable;
        public bool IsTransparent;
        public bool IsWalkable;
        public Color MMCol;
        public string MTex;
        public string Name;
        public Type Prototype;
        public SmartAction SmartAction;

        public int StorageSlots;
        public int swide = 32;

        //public override bool Equals(object obj) {
        //    if (obj == null || GetType() != obj.GetType()) {
        //        return false;
        //    }
        //    var a = obj as BlockData;
        //    if (a == null) return false;

        //    var fields = typeof (BlockData).GetFields();

        //    foreach (FieldInfo field in fields) {
        //        var value = field.GetValue(a);
        //        var o = field.GetValue(this);
        //        if(o == null && value == null) continue;
        //        if (o == null || value == null) return false;
        //        if (value.Equals(o)) {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public string RandomMtexFromAlters() {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1) {
                return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            return MTex;
        }

        public static Rectangle GetSource(string s) {
            if (s == null) {
                return new Rectangle(0, 0, 0, 0);
            }
            int index = Atlases.BlockIndexes[s];
            return new Rectangle(index%32*32, index/32*32, 32, 32);
        }
    }
}