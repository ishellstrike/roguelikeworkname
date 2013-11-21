using System.Collections.Generic;

namespace jarg {
    public class Achievements {
        public static Dictionary<string, Achievement> Data = new Dictionary<string, Achievement>();
        public static Dictionary<string, Statist> Stat = new Dictionary<string, Statist>();

        public Achievements() {
            Data.Add("test1", new Achievement {Name = "Test 1", Description = "Desc 1"});
            Data.Add("test2", new Achievement {Name = "Test 2", Description = "Desc 2"});

            Stat.Add("ammototal", new Statist {Name = "Собрано патронов"});
            Stat.Add("ammouse", new Statist {Name = "Израсходовано патронов"});
            Stat.Add("guntotal", new Statist {Name = "Огнестрельного оружия"});
            Stat.Add("foodtotal", new Statist {Name = "Еды"});
            Stat.Add("medtotal", new Statist { Name = "Медикаментов" });
            Stat.Add("zombiekill", new Statist {Name = "Зомби"});
            Stat.Add("takedmg", new Statist {Name = "получено урона"});
            Stat.Add("makedmg", new Statist {Name = "Нанесено урона"});
            Stat.Add("maxdmg", new Statist {Name = "Максимальный урон"});
            Stat.Add("dooro", new Statist {Name = "Открыто дверей"});
            Stat.Add("walk", new Statist {Name = "Пройдено"});
            Stat.Add("drive", new Statist {Name = "На транспортном средстве"});
            Stat.Add("fooduse", new Statist { Name = "Еды" });
            Stat.Add("meduse", new Statist { Name = "Медикаментов" });
        }
    }
}