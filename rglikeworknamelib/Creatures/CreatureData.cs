using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Creatures
{
    public class CreatureData {
        public string MTex;
        public string Name;
        public Type CreaturePrototype;

        public CreatureData(string t) {
            MTex = t;
        }
    }
}
