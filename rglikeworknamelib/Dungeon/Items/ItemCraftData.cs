using System.Collections.Generic;
using System.Linq;
using System.Text;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemCraftRequire {
        public AbilityType Ability;
        public int Level;
    }

    public class ItemCraftData
    {
        //using output[0] name if null
        public string Name;
        public List<ItemCraftWithAlters> Output, Input;
        //in game minutes
        public int Time;
        public ItemCraftType Type;
        public List<ItemCraftRequire> Require; 

        public override string ToString() {
            StringBuilder s = new StringBuilder();

            s.AppendLine("Необходимо:");
            foreach (var alterse in Input) {
                s.Append("* ");
                foreach (var alter in alterse.Alters) {
                    s.AppendFormat("{0}{1} {2}", ItemDataBase.Instance.Data[alter.Id].Name, alter.Count > 1 ? " x" + alter.Count : string.Empty, alter.IsTool ? "(Tool)" : string.Empty);
                    if (alter != alterse.Alters.Last()) {
                        s.Append(", ");
                    }
                }
                s.AppendLine();
            }
            s.AppendLine();
            s.AppendLine("Результат:");
            foreach (var alterse in Output) {
                s.Append("* ");
                foreach (var alter in alterse.Alters)
                {
                    s.AppendFormat("{0}{1} {2}", ItemDataBase.Instance.Data[alter.Id].Name, alter.Count > 1 ? " x" + alter.Count : string.Empty, alter.IsTool ? "(Tool)" : string.Empty);
                    if (alter != alterse.Alters.Last())
                    {
                        s.Append(", ");
                    }
                }
                s.AppendLine();
            }

            if (Require != null) {
                s.AppendLine();
                s.AppendLine("Навыки:");
                foreach (var req in Require) {
                    s.AppendFormat("{0} - {1}", req.Ability.ToString(), Ability.GetLevelName(req.Level));
                    if (req != Require.Last())
                    {
                        s.Append(", ");
                    }
                }
            }

            return s.ToString();
        }
    }
}