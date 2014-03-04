using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Dungeon.Level
{
    public class NewBlockData : Singleton<NewBlockData> {
        public string BaseName;
        public string[] AlterMtex;

        public virtual string SerializeTag(object tag) {
            return string.Empty;
        }

        public virtual object DeserializeTag(string s) {
            return null;
        }
    }
}
