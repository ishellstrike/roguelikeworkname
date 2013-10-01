using System.Collections.Generic;
using System.Linq;

namespace rglikeworknamelib.Creatures {
    public class AbilitiesPlayer : AbilitiesBasic {
        public Dictionary<string, Ability> Abilities;
        public List<Ability> ToShow; 

        public AbilitiesPlayer()
        {
            Abilities = new Dictionary<string, Ability>();
            Abilities.Add("survive", new Ability
            {
                Name = "Выживание",
                nameStyle = AbilityNameStyle.physical
            });
            Abilities.Add("atlet", new Ability
            {
                Name = "Атлетика",
                nameStyle = AbilityNameStyle.physical
            });
            Abilities.Add("shoot", new Ability
            {
                Name = "Стрельба",
                nameStyle = AbilityNameStyle.physical
            });
            Abilities.Add("martial", new Ability
            {
                Name = "Ближний бой",
                nameStyle = AbilityNameStyle.physical
            });
            Abilities.Add("coock", new Ability
            {
                Name = "Готовка",
                nameStyle = AbilityNameStyle.mental
            });
            Abilities.Add("chem", new Ability
            {
                Name = "Химия",
                nameStyle = AbilityNameStyle.mental
            });
            Abilities.Add("phys", new Ability
            {
                Name = "Физика",
                nameStyle = AbilityNameStyle.mental
            });
            Abilities.Add("bio", new Ability
            {
                Name = "Биология",
                nameStyle = AbilityNameStyle.mental
            });
            Abilities.Add("it", new Ability
            {
                Name = "IT",
                nameStyle = AbilityNameStyle.mental
            });
            Abilities.Add("tailor", new Ability
            {
                Name = "Шитье",
                nameStyle = AbilityNameStyle.mental
            });
            Abilities.Add("read", new Ability
            {
                Name = "Чтение",
                nameStyle = AbilityNameStyle.mental
            });

            ToShow = new List<Ability>();
            ToShow.AddRange(Abilities.Select(x=>x.Value));
            ToShow.Sort((x,y)=>x.Name.CompareTo(y.Name));
        }
    }
}