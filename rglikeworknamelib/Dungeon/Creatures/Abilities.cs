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
            list = new Dictionary<string, Ability> {
                {
                    "survive", new Ability {
                        Name = "���������",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "atlet", new Ability {
                        Name = "��������",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "shoot", new Ability {
                        Name = "��������",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "martial", new Ability {
                        Name = "������� ���",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "coock", new Ability {
                        Name = "�������",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "chem", new Ability {
                        Name = "�����",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "phys", new Ability {
                        Name = "������",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "bio", new Ability {
                        Name = "��������",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "it", new Ability {
                        Name = "IT",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "tailor", new Ability {
                        Name = "�����",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "read", new Ability {
                        Name = "������",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "lockpick", new Ability {
                        Name = "�����",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "pickpocket", new Ability {
                        Name = "��������� �����",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "talk", new Ability {
                        Name = "���������",
                        NameStyle = AbilityNameStyle.mental
                    }
                }
            };

            ToShow = new List<Ability>();
            ToShow.AddRange(list.Select(x => x.Value));
            ToShow.Sort((x, y) => x.Name.CompareTo(y.Name));
        }
    }
}