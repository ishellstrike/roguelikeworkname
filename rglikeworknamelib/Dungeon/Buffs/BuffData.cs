using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Dungeon.Effects
{
    public class BuffData
    {
        public Type Prototype;
        public string[] Value;

        /// <summary>
        /// Duration in game minutes
        /// </summary>
        public int Duration;
        public string Name;
    }
}
