using System;
using System.Collections.Generic;
using System.Linq;

namespace rglikeworknamelib.Dungeon.Creatures
{
    [Serializable]
    public class Abilities {
        public List<Ability> ToShow;
        public Dictionary<string, Ability> list;

        public Abilities() {
            list = new Dictionary<string, Ability>();
            list.Add("survive", new Ability {
                Name = "���������",
                NameStyle = AbilityNameStyle.physical
            });
            list.Add("atlet", new Ability {
                Name = "��������",
                NameStyle = AbilityNameStyle.physical
            });
            list.Add("shoot", new Ability {
                Name = "��������",
                NameStyle = AbilityNameStyle.physical
            });
            list.Add("martial", new Ability {
                Name = "������� ���",
                NameStyle = AbilityNameStyle.physical
            });
            list.Add("coock", new Ability {
                Name = "�������",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("chem", new Ability {
                Name = "�����",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("phys", new Ability {
                Name = "������",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("bio", new Ability {
                Name = "��������",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("it", new Ability {
                Name = "IT",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("tailor", new Ability {
                Name = "�����",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("read", new Ability {
                Name = "������",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("lockpick", new Ability {
                Name = "�����",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("pickpocket", new Ability {
                Name = "��������� �����",
                NameStyle = AbilityNameStyle.mental
            });
            list.Add("talk", new Ability {
                Name = "���������",
                NameStyle = AbilityNameStyle.mental
            });

            ToShow = new List<Ability>();
            ToShow.AddRange(list.Select(x => x.Value));
            ToShow.Sort((x, y) => x.Name.CompareTo(y.Name));
        }
    }
}