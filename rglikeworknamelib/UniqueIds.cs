using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib
{
    public static class UniqueIds {
        private static int iteIds, monIds;

        public static int GetNewItemId() {
            iteIds++;
            return iteIds;
        }

        public static int GetNewMonsterId()
        {
            monIds++;
            return monIds;
        }
    }
}
