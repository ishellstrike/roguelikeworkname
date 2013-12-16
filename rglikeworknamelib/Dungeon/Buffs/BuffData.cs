using System;

namespace rglikeworknamelib.Dungeon.Effects {
    public class BuffData {
        /// <summary>
        ///     Duration in game minutes
        /// </summary>
        public int Duration;

        public string Name, Description;
        public Type Prototype;
        public string[] Value;

        public override string ToString() {
            return string.Format("{0} ({1})", Name, Prototype);
        }
    }
}