using System;
using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Creatures {
    public class MoraleModifer {
        private string id_;
        public TimeSpan Time;
        public MoraleModiferData Data;

        public string Id {
            get { return id_; }
            set { id_ = value;
                Data = MoraleModiferDataBase.Data[value];
            }
        }
    }

    public class MoraleModiferData {
        public string Name, Description;
        public int Duration, Bonus;

        public override string ToString() {
            return string.Format("{0} ({1})", Name, Bonus);
        }
    }

    public class MoraleModiferDataBase
    {
        public static Dictionary<string, MoraleModiferData> Data;

        public MoraleModiferDataBase()
        {
            Data = new Dictionary<string, MoraleModiferData>();
            List<KeyValuePair<string, object>> a =
                ParsersCore.UnivarsalParseFile(
                    Directory.GetCurrentDirectory() + @"/" + Settings.GetDataDirectory()+@"/morale.txt",
                    UniversalParser.Parser<MoraleModiferData>);
            foreach (var pair in a) {
                Data.Add(pair.Key, (MoraleModiferData)pair.Value);
            }
        }


    }
}