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
                        Name = "Выживание",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "atlet", new Ability {
                        Name = "Атлетика",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "shoot", new Ability {
                        Name = "Стрельба",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "martial", new Ability {
                        Name = "Ближний бой",
                        NameStyle = AbilityNameStyle.physical
                    }
                }, {
                    "coock", new Ability {
                        Name = "Готовка",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "chem", new Ability {
                        Name = "Химия",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "phys", new Ability {
                        Name = "Физика",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "bio", new Ability {
                        Name = "Биология",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "it", new Ability {
                        Name = "IT",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "tailor", new Ability {
                        Name = "Шитье",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "read", new Ability {
                        Name = "Чтение",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "lockpick", new Ability {
                        Name = "Взлом",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "pickpocket", new Ability {
                        Name = "Карманная кража",
                        NameStyle = AbilityNameStyle.mental
                    }
                }, {
                    "talk", new Ability {
                        Name = "Убеждение",
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