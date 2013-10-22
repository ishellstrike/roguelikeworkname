using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items {
    public class CraftData {
        public string[] Input1;
        public string[] Input1Count;

        public string[] Input2;
        public string[] Input2Count;

        public string[] Input3;
        public string[] Input3Count;

        public string[] Input4;
        public string[] Input4Count;

        public string[] Output;
        public string[] OutputCount;

        public CraftType SType = CraftType.Other;

        public override string ToString() {
            var sb = new StringBuilder();

            sb.AppendLine(" Создать:");

            for (int i = 0; i < Output.Length; i++) {
                sb.AppendLine(string.Format("{1}{0}", ItemDataBase.Data[Output[i]].Name, OutputCount[i] != "1" ? OutputCount[i] != "0" ? OutputCount[i] + " " : "(и) " : string.Empty));
            }

            sb.AppendLine(" Необходимо:");

            GetValue(sb, Input1, Input1Count, 1);
            GetValue(sb, Input2, Input2Count, 2);
            GetValue(sb, Input3, Input3Count, 3);
            GetValue(sb, Input4, Input4Count, 4);

            return sb.ToString();
        }

        private void GetValue(StringBuilder sb, string[] input, string[] inputcount, int count)
        {
            if (input != null) {
                sb.Append(string.Format("{0}) ", count));
                sb.Append(string.Format("{1}{0}", ItemDataBase.Data[input[0]].Name, inputcount[0] != "1" ? inputcount[0] != "0" ? inputcount[0] + " " : "(и) " : string.Empty));
                for (int i = 1; i < input.Length; i++)
                {
                    sb.Append(string.Format(" или {1}{0}", ItemDataBase.Data[input[i]].Name, inputcount[i] != "1" ? inputcount[i] != "0" ? inputcount[i] + " " : "(и) " : string.Empty));
                }
                sb.Append(Environment.NewLine);
            }
        }
    }

    public enum CraftType {
        Other,
        Misc,
        Food,
        Medicine,
        Weapon,
        Disassemble
    }

    public class CraftDataBase {
        public static Collection<CraftData> Data;

        public CraftDataBase() {
            Data = new Collection<CraftData>();
            List<object> t = ParsersCore.UniversalParseDirectory(Settings.GetCraftsDirectory(),
                                                                 UniversalParser.NoIdParser<CraftData>);
            foreach (object value in t) {
                Data.Add((CraftData) value);
            }
        }
    }
}