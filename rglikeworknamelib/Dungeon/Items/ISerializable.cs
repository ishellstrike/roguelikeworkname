using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Dungeon.Items
{
    interface ISerializable {
        string Serialize();
        object Desesialize(string s);
    }

}
