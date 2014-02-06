using System;
using System.Collections.Generic;
using System.Linq;

namespace rglikeworknamelib.Dungeon.Creatures
{
    [Serializable]
    public enum AbilityType {
        Survive,
        Atlet,
        Shoot,
        Martial,
        Cook,
        Chem,
        Phys,
        It,
        Tailor,
        Read,
        Pickpocket,
        Lockpick,
        Talk,
        Bio
    }
    [Serializable]
    public class Abilities {
        public List<Ability> ToShow;
        public Dictionary<AbilityType, Ability> List;

        public Abilities() {
            List = new Dictionary<AbilityType, Ability> {
                {
                    AbilityType.Survive, new Ability {
                        Name = "Выживание",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    AbilityType.Atlet, new Ability {
                        Name = "Атлетика",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    AbilityType.Shoot, new Ability {
                        Name = "Стрельба",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    AbilityType.Martial, new Ability {
                        Name = "Ближний бой",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    AbilityType.Cook, new Ability {
                        Name = "Готовка",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Chem, new Ability {
                        Name = "Химия",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Phys, new Ability {
                        Name = "Физика",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Bio, new Ability {
                        Name = "Биология",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.It, new Ability {
                        Name = "IT",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Tailor, new Ability {
                        Name = "Шитье",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Read, new Ability {
                        Name = "Чтение",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Lockpick, new Ability {
                        Name = "Взлом",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Pickpocket, new Ability {
                        Name = "Карманная кража",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    AbilityType.Talk, new Ability {
                        Name = "Убеждение",
                        NameStyle = AbilityNameStyle.mental
                    }
                }
            };

            ToShow = new List<Ability>();
            ToShow.AddRange(List.Select(x => x.Value));
            ToShow.Sort((x, y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));
        }
    }
}