using System;
using NLog;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public class BlockFactory {
        private static readonly Logger logger = LogManager.GetLogger("BlockFactory");
        public static Block GetInstance(string id) {
            if (!BlockDataBase.Data.ContainsKey(id))
            {
                logger.Error(string.Format("Missing BlockData id={0}!!!", id));
                return null;
            }
            var a = Activator.CreateInstance(BlockDataBase.Data[id].TypeParsed);
            ((Block)a).Id = id;

            return (Block)a;
        }
    }
}