using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jarg
{
    public class Achievement {
        public string Name;
        public string Description;
        public DateTime Date;
        public AcievementType Type;
    }
    public enum AcievementType {
        
    }
    public class Statist {
        public string Name;
        public string Description;
        public float Count;
        public StatistType Type;
    }
    public enum StatistType {
        
    }
    public class Achievements
    {
        public static Dictionary<string, Achievement> Data = new Dictionary<string, Achievement>();
        public static Dictionary<string, Statist> Stat = new Dictionary<string, Statist>(); 
        public Achievements() {
            Data.Add("test1", new Achievement{Name = "Test 1", Description = "Desc 1"});
            Data.Add("test2", new Achievement { Name = "Test 2", Description = "Desc 2" });

            Stat.Add("ammototal", new Statist{Name ="Vsego patronov sobrano"});
            Stat.Add("ammouse", new Statist { Name = "Vsego patronov rasstrelyano" });
            Stat.Add("guntotal", new Statist { Name = "Vsego pushek sobrano" });
            Stat.Add("foodtotal", new Statist { Name = "Vsego edi sobrano" });
            Stat.Add("zombiekill", new Statist { Name = "Ubito zombi" });
            Stat.Add("takedmg", new Statist { Name = "Polucheno urona" });
            Stat.Add("makedmg", new Statist { Name = "Naneseno urona" });
            Stat.Add("maxdmg", new Statist { Name = "Maksimalnii uron" });
            Stat.Add("dooro", new Statist { Name = "Otkrito dverei" });
            Stat.Add("walk", new Statist { Name = "Vsego proideno" });
            Stat.Add("drive", new Statist { Name = "Vsego proehano" });
        }
    }
}
