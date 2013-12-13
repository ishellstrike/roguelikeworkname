using System.Collections.Generic;
using System.IO;
using rglikeworknamelib;
using rglikeworknamelib.Parser;

namespace jarg {
    public class AchievementDataBase {
        public static Dictionary<string, AchievementData> Data = new Dictionary<string, AchievementData>();
        public static Dictionary<string, StatistData> Stat = new Dictionary<string, StatistData>();

        public AchievementDataBase() {
            Data = new Dictionary<string, AchievementData>();
            List<KeyValuePair<string, object>> a =
                ParsersCore.UnivarsalParseFile(
                    Directory.GetCurrentDirectory() + @"/" + Settings.GetDataDirectory() + @"/achievements.txt",
                    UniversalParser.Parser<AchievementData>);
            foreach (var pair in a)
            {
                Data.Add(pair.Key, (AchievementData)pair.Value);
            }

            Stat = new Dictionary<string, StatistData>();
            a =
                ParsersCore.UnivarsalParseFile(
                    Directory.GetCurrentDirectory() + @"/" + Settings.GetDataDirectory() + @"/statist.txt",
                    UniversalParser.Parser<StatistData>);
            foreach (var pair in a)
            {
                Stat.Add(pair.Key, (StatistData)pair.Value);
            }
        }
    }
}