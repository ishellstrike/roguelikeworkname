using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NLog;
using jarg;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items {
    
    public class ItemDataBase {
        public static Dictionary<string, ItemData> Data;
        public static Dictionary<string, ItemData> DataMedicineItems;
        public static Dictionary<string, ItemData> DataFoodItems;
        public static Dictionary<string, ItemAction> ItemScripts;

        public static Dictionary<string, List<DropGroup>> SpawnLists; 

        private static Logger logger_ = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public ItemDataBase() {
            //texatlas = texatlas_;
            Data = new Dictionary<string, ItemData>();
            DataFoodItems = new Dictionary<string, ItemData>();
            DataMedicineItems = new Dictionary<string, ItemData>();
            List<KeyValuePair<string, object>> a = ParsersCore.UniversalParseDirectory(Settings.GetItemDataDirectory(),
                                                                                       UniversalParser.Parser<ItemData>);
            foreach (var pair in a) {
                Data.Add(pair.Key, (ItemData) pair.Value);
                if (((ItemData) pair.Value).SortType == ItemType.Medicine) {
                    DataMedicineItems.Add(pair.Key, (ItemData) pair.Value);
                }
                if (((ItemData) pair.Value).SortType == ItemType.Food) {
                    DataFoodItems.Add(pair.Key, (ItemData) pair.Value);
                }
            }

            SpawnLists = new Dictionary<string, List<DropGroup>>();

            var b = ParsersCore.ParseDirectory(Settings.GetSpawnlistsDataDirectory(), SpawnlistParser.Parser);

            foreach (var pair in b) {
                if(SpawnLists.ContainsKey(pair.Key)) {
                    SpawnLists[pair.Key].Add(pair.Value);
                }
                else {
                    SpawnLists.Add(pair.Key, new List<DropGroup>{ pair.Value });
                }
            }

            ItemScripts = new Dictionary<string, ItemAction>();
            ItemScripts.Add("opencan", new ItemAction(OpenCan, "Открыть банку"));
            ItemScripts.Add("disass", new ItemAction(Disass, "Разобрать радио"));
            ItemScripts.Add("turnradio", new ItemAction(RadioOnOff, "Включить радио"));
            ItemScripts.Add("openbottle", new ItemAction(OpenBottle, "Открыть бутылку"));
            ItemScripts.Add("destroycloth", new ItemAction(DestroyCloth, "Разорать на тряпки"));
            ItemScripts.Add("smoke", new ItemAction(Smoke, "Выкурить сигарету"));
        }

        public static Dictionary<string, ItemData> GetItemByItemDataSortType(ItemType it) {
            switch (it) {
                case ItemType.Nothing:
                    return Data;
                case ItemType.Medicine:
                    return DataMedicineItems;
                case ItemType.Food:
                    return DataFoodItems;
                default:
                    return null;
            }
        }

        public static string GetItemDescription(IItem i) {
            return i.Data.Description;
        }

        public static List<KeyValuePair<string,ItemData>> GetBySpawnGroup(string group) {
            return Data.Where(x => x.Value.SpawnGroup == group).ToList();
        }

        public static string GetItemFullDescription(IItem i) {
            ItemData item = Data[i.Id];
            var sb = new StringBuilder();
            sb.Append(item.Name);
            if (item.Description != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + item.Description);
            }
            sb.Append(Environment.NewLine + string.Format("{0} г", item.Weight));
            sb.Append(Environment.NewLine + string.Format("{0} места", item.Volume));
            if (Settings.DebugInfo) {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, 0));
            }

            if (item.AfteruseId != null) {
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", Data[item.AfteruseId].Name));
            }

            if (item.Buff != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Эффекты :"));

                foreach (string buff in item.Buff) {
                    sb.Append(Environment.NewLine + string.Format("{0}", BuffDataBase.Data[buff].Name));
                }
            }
            if (item.Ammo != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Калибр :"));

                sb.Append(Environment.NewLine + Data[item.Ammo].Name);
            }
            //switch (item.SortType) {
            //    case ItemType.Medicine:
            //        sb.Append(item.)
            //        break;
            //}
            return sb.ToString();
        }

        #region ItemScripts
        public void OpenCan(Player p, IItem target) {
            if (p.Inventory.ContainsId("knife") || p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
            }
            else {
                EventLog.Add("Чтобы открывать банки вам нужен нож или отвертка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
        private void Disass(Player p, IItem target)
        {
            if (p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", Settings.rnd.Next(5) + 10));

                EventLog.Add(string.Format("Вы успешно разбираете {0}", target.Data.Name), Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
            else
            {
                EventLog.Add("Чтобы разбирать электронику вам нужна отвертка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }

        }
        private void RadioOnOff(Player p, IItem target)
        {
            EventLog.Add("Радио включается", Color.White, LogEntityType.Default);
        }
        public void OpenBottle(Player p, IItem target)
        {
            p.Inventory.TryRemoveItem(target.Id, 1);
            p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
        }
        public void DestroyCloth(Player p, IItem target)
        {
            var weight = target.Data.Weight;
            if (weight > 10000)
            {
                EventLog.Add("Предмет слишком большой", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            if (weight < 100)
            {
                EventLog.Add("Предмет слишком маленький", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            p.Inventory.TryRemoveItem(target.Id, 1);
            int smallparts = 0;
            int bigparts = 0;
            var rnd = Settings.rnd;
            while (weight >= 50)
            {
                var part = rnd.Next(2);
                switch (part)
                {
                    case 0:
                        weight -= 50;
                        p.Inventory.AddItem(ItemFactory.GetInstance("brcloth", 1));
                        smallparts++;
                        break;
                    case 1:
                        weight -= 100;
                        p.Inventory.AddItem(ItemFactory.GetInstance("partcloth", 1));
                        bigparts++;
                        break;
                }
            }
            string mess = string.Format("Вы успешно разорвали {0} на", target.Data.Name);
            if (bigparts > 0)
            {
                mess += string.Format(" {0} {1}", bigparts, Data["brcloth"].Name);
            }
            if (smallparts > 0)
            {
                mess += string.Format(" {0} {1}", smallparts, Data["partcloth"].Name);
            }
            EventLog.Add(mess, Color.Yellow, LogEntityType.NoAmmoWeapon);
        }
        public void Smoke(Player p, IItem target)
        {
            if (p.Inventory.ContainsId("lighter1") || p.Inventory.ContainsId("lighter2"))
            {
                EventLog.Add(
                    string.Format("Вы выкурили сигарету "),
                    GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                foreach (IBuff buff in target.Buffs)
                {
                    buff.ApplyToTarget(p);
                }

                AchievementDataBase.Stat["sigause"].Count++;

                target.Doses--;
                if (target.Doses <= 0)
                {
                    p.Inventory.RemoveItem(target);
                }
            }
            else
            {
                EventLog.Add("Чтобы курить вам нужна зажигалка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
#endregion
    }
}