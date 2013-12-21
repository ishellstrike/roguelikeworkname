using System;
using NLog;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemFactory {
        private static readonly Logger logger = LogManager.GetLogger("ItemFactory");
        public static IItem GetInstance(string id, int count) {
            if (!ItemDataBase.Data.ContainsKey(id)) {
                logger.Error(string.Format("Missing ItemData id={0}!!!", id));
                return null;
            }
            var a = (IItem)new Item();
            a.Id = id;
            a.Count = count;
            a.OnLoad();
            return a;
        }
    }
}