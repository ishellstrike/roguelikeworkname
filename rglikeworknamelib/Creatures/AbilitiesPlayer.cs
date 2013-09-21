namespace rglikeworknamelib.Creatures {
    public class AbilitiesPlayer : AbilitiesBasic {
        public Ability Survive,
                       Run,
                       Shooting,
                       Martial,
                       Cooking,
                       Chemistry,
                       Physics,
                       Biology,
                       Gadgets,
                       Tailoring,
                       Reading;

        public Ability[] AllAbilities;

        public AbilitiesPlayer()
        {
            Survive = new Ability {
                Name = "Выживание",
                nameStyle = AbilityNameStyle.physical
            };
            Run = new Ability {
                Name = "Бег",
                nameStyle = AbilityNameStyle.physical
            };
            Shooting = new Ability {
                Name = "Стрельба",
                nameStyle = AbilityNameStyle.physical
            };
            Martial = new Ability {
                Name = "Ближний бой",
                nameStyle = AbilityNameStyle.physical
            };
            Cooking = new Ability {
                Name = "Готовка",
                nameStyle = AbilityNameStyle.mental
            };
            Chemistry = new Ability {
                Name = "Химия",
                nameStyle = AbilityNameStyle.mental
            };
            Physics = new Ability {
                Name = "Физика",
                nameStyle = AbilityNameStyle.mental
            };
            Biology = new Ability {
                Name = "Биология",
                nameStyle = AbilityNameStyle.mental
            };
            Gadgets = new Ability {
                Name = "IT",
                nameStyle = AbilityNameStyle.mental
            };
            Tailoring = new Ability {
                Name = "Шитье",
                nameStyle = AbilityNameStyle.mental
            };
            Reading = new Ability {
                Name = "Чтение",
                nameStyle = AbilityNameStyle.mental
            };

            AllAbilities = new[] {
                                     Survive,
                                     Run,
                                     Shooting,
                                     Martial,
                                     Cooking,
                                     Chemistry,
                                     Physics,
                                     Biology,
                                     Gadgets,
                                     Tailoring,
                                     Reading
                                 };
        }
    }
}