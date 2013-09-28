using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockData 
    {
        public string AfterDeathId;

        public float Damage;
        public string Description;
        public bool IsDestructable;

        public Type Prototype;
        public SmartAction SmartAction;

        public float Hp;
        public string MTex;
        public string Name;

        public Color MMCol;

        public bool IsWalkable;
        public bool IsTransparent;

        public int swide = 32;

        public int StorageSlots;

        public string[] AlterMtex;

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

        public Rectangle RandomMtexFromAlters(ref string texstring)
        {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1) {
                texstring = AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            } else {
                texstring = MTex;
            }
            return GetSource(texstring);
        }

        public static Rectangle GetSource(string s)
        {
            int index = Atlases.BlockIndexes[s];
            return new Rectangle(index % 32 * 32, index / 32 * 32, 32, 32);
        }
    }
}