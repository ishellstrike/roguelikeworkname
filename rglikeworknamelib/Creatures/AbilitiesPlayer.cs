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
                Name = "���������",
                nameStyle = AbilityNameStyle.physical
            };
            Run = new Ability {
                Name = "���",
                nameStyle = AbilityNameStyle.physical
            };
            Shooting = new Ability {
                Name = "��������",
                nameStyle = AbilityNameStyle.physical
            };
            Martial = new Ability {
                Name = "������� ���",
                nameStyle = AbilityNameStyle.physical
            };
            Cooking = new Ability {
                Name = "�������",
                nameStyle = AbilityNameStyle.mental
            };
            Chemistry = new Ability {
                Name = "�����",
                nameStyle = AbilityNameStyle.mental
            };
            Physics = new Ability {
                Name = "������",
                nameStyle = AbilityNameStyle.mental
            };
            Biology = new Ability {
                Name = "��������",
                nameStyle = AbilityNameStyle.mental
            };
            Gadgets = new Ability {
                Name = "IT",
                nameStyle = AbilityNameStyle.mental
            };
            Tailoring = new Ability {
                Name = "�����",
                nameStyle = AbilityNameStyle.mental
            };
            Reading = new Ability {
                Name = "������",
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