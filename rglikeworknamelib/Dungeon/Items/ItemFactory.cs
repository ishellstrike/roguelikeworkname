using System;
using NLog;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemFactory {
        private static readonly Logger logger = LogManager.GetLogger("ItemFactory");
        public static Item GetInstance(string id, int count) {
            if (!ItemDataBase.Instance.Data.ContainsKey(id)) {
                logger.Error(string.Format("Missing ItemData id={0}!!!", id));
                return null;
            }
            var a = Activator.CreateInstance(ItemDataBase.Instance.Data[id].TypeParsed);
            ((Item) a).Id = id;
            ((Item) a).Count = count;
            return (Item)a;
        }
    }
}