using System;
using System.Collections.Generic;
using System.Linq;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class Abilities {
        public Dictionary<string, Ability> list;
        public List<Ability> ToShow; 

        public Abilities()
        {
            list = new Dictionary<string, Ability>();
            list.Add("survive", new Ability
            {
                Name = "Выживание",
                nameStyle = AbilityNameStyle.physical
            });
            list.Add("atlet", new Ability
            {
                Name = "Атлетика",
                nameStyle = AbilityNameStyle.physical
            });
            list.Add("shoot", new Ability
            {
                Name = "Стрельба",
                nameStyle = AbilityNameStyle.physical
            });
            list.Add("martial", new Ability
            {
                Name = "Ближний бой",
                nameStyle = AbilityNameStyle.physical
            });
            list.Add("coock", new Ability
            {
                Name = "Готовка",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("chem", new Ability
            {
                Name = "Химия",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("phys", new Ability
            {
                Name = "Физика",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("bio", new Ability
            {
                Name = "Биология",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("it", new Ability
            {
                Name = "IT",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("tailor", new Ability
            {
                Name = "Шитье",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("read", new Ability
            {
                Name = "Чтение",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("lockpick", new Ability
            {
                Name = "Взлом",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("pickpocket", new Ability
            {
                Name = "Карманная кража",
                nameStyle = AbilityNameStyle.mental
            });
            list.Add("talk", new Ability
            {
                Name = "Убеждение",
                nameStyle = AbilityNameStyle.mental
            });

            ToShow = new List<Ability>();
            ToShow.AddRange(list.Select(x=>x.Value));
            ToShow.Sort((x,y)=>x.Name.CompareTo(y.Name));
        }
    }
}