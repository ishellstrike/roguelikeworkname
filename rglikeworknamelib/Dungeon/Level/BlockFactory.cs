using System;
using NLog;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public class BlockFactory {
        private static readonly Logger logger = LogManager.GetLogger("BlockFactory");
        public static Block GetInstance(string id) {
            if (!Registry.Instance.Blocks.ContainsKey(id))
            {
                logger.Error(string.Format("Missing BlockData id={0}!!!", id));
                return null;
            }
            var a = Activator.CreateInstance(Registry.Instance.Blocks[id].TypeParsed);
            ((Block)a).Id = id;

            return (Block)a;
        }
    }
}