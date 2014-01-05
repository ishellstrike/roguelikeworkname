using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IronPython.Hosting;
using jarg;
using Microsoft.Scripting.Hosting;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items
{
    public sealed class ItemDataBase
    {
        private static readonly Logger logger = LogManager.GetLogger("ItemDataBase");

        private static Logger logger_ = LogManager.GetCurrentClassLogger();

        private static volatile ItemDataBase instance_;
        private readonly ScriptRuntime ipy = Python.CreateRuntime();
        public Dictionary<string, ItemData> Data;
        public Dictionary<string, ItemAction> ItemScripts;

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        private ItemDataBase()
        {
            //texatlas = texatlas_;
            Data = new Dictionary<string, ItemData>();

            Data = UniversalParser.JsonDataLoader<ItemData>(Settings.GetItemDataDirectory());

            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Item assembly loading";

            ipy.LoadAssembly(Assembly.GetAssembly(typeof (Item)));

            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Item base script loading";

            dynamic isNothing = ipy.UseFile(Settings.GetItemDataDirectory() + "\\is_nothing.py");

            string[] files = Directory.GetFiles(Settings.GetItemDataDirectory(), "*.py");
            ItemScripts = new Dictionary<string, ItemAction>();
            int i = 0;
            foreach (string f in files)
            {
                var r = new FileInfo(f);
                Settings.NeedToShowInfoWindow = true;
                Settings.NTS1 = "IScripts: ";
                Settings.NTS2 = string.Format("{0}/{1} ({2})", i + 1, files.Length, r.Name);
                i++;
                string name = r.Name.Replace(r.Extension, string.Empty);
                dynamic temp;
                string disc = string.Empty;
                try
                {
                    temp = ipy.UseFile(f);
                }
                catch (Exception ex)
                {
                    ItemScripts.Add(name, new ItemAction(isNothing, "ошибка"));
                    logger.Error(ex);
#if DEBUG
                    throw;
#else
                    continue;
#endif
                }
                var ia = new ItemAction(temp, disc);
                ItemScripts.Add(name, ia);
                temp.Name(ia);
            }

            Settings.NeedToShowInfoWindow = false;

            //ItemScripts.Add("opencan", new ItemAction(OpenCan, "Открыть банку"));
            //ItemScripts.Add("disass", new ItemAction(Disass, "Разобрать радио"));
            //ItemScripts.Add("turnradio", new ItemAction(RadioOnOff, "Включить радио"));
            //ItemScripts.Add("openbottle", new ItemAction(OpenBottle, "Открыть бутылку"));
            //ItemScripts.Add("destroycloth", new ItemAction(DestroyCloth, "Разорать на тряпки"));
            //ItemScripts.Add("smoke", new ItemAction(Smoke, "Выкурить сигарету"));
        }

        /// <summary>
        ///     Первое обращение порождает первичную загрузку
        /// </summary>
        public static ItemDataBase Instance
        {
            get { return instance_ ?? (instance_ = new ItemDataBase()); }
        }

        public string GetItemDescription(Item i)
        {
            return i.Data.Description;
        }

        public List<KeyValuePair<string, ItemData>> GetBySpawnGroup(string group)
        {
            return Data.Where(x => x.Value.SpawnGroup == group).ToList();
        }

        public string GetItemFullDescription(Item i)
        {
            ItemData item = Data[i.Id];
            var sb = new StringBuilder();
            sb.Append(item.Name);
            if (item.Description != null)
            {
                sb.Append(Environment.NewLine + item.Description);
            }
            sb.Append(Environment.NewLine + string.Format("{0} г", item.Weight));
            sb.Append(Environment.NewLine + string.Format("{0} места", item.Volume));
            if (Settings.DebugInfo)
            {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, 0));
            }

            if (item.AfteruseId != null)
            {
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", Data[item.AfteruseId].Name));
            }

            if (item.Buff != null)
            {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Эффекты :"));

                foreach (string buff in item.Buff)
                {
                    sb.Append(Environment.NewLine + string.Format("{0}", BuffDataBase.Data[buff].Name));
                }
            }
            if (item.Ammo != null)
            {
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

        public void OpenCan(Player p, Item target)
        {
            if (p.Inventory.ContainsId("knife") || p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
            }
            else
            {
                EventLog.Add("Чтобы открывать банки вам нужен нож или отвертка", Color.Yellow,
                    LogEntityType.NoAmmoWeapon);
            }
        }

        private void Disass(Player p, Item target)
        {
            if (p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", Settings.rnd.Next(5) + 10));

                EventLog.Add(string.Format("Вы успешно разбираете {0}", target.Data.Name), Color.Yellow,
                    LogEntityType.NoAmmoWeapon);
            }
            else
            {
                EventLog.Add("Чтобы разбирать электронику вам нужна отвертка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }

        private void RadioOnOff(Player p, Item target)
        {
            EventLog.Add("Радио включается", Color.White, LogEntityType.Default);
        }

        public void OpenBottle(Player p, Item target)
        {
            p.Inventory.TryRemoveItem(target.Id, 1);
            p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
        }

        public void DestroyCloth(Player p, Item target)
        {
            int weight = target.Data.Weight;
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
            Random rnd = Settings.rnd;
            while (weight >= 50)
            {
                int part = rnd.Next(2);
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

        public void Smoke(Player p, Item target)
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