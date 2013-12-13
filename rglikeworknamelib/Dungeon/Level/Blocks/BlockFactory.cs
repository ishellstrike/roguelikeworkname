using System;
using NLog;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public class BlockFactory {
        private static readonly Logger logger = LogManager.GetLogger("BlockFactory");
        public static IBlock GetInstance(string id) {
            if (!BlockDataBase.Data.ContainsKey(id))
            {
                logger.Error(string.Format("Missing BlockData id={0}!!!", id));
                return null;
            }
            var a = (IBlock)Activator.CreateInstance(BlockDataBase.Data[id].Prototype);
            a.Id = id;

            return a;
        }
    }
}