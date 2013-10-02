using System;
using System.Collections.Generic;
using System.Linq;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class PerksSystem {
        private Dictionary<string, Perk> Perks;
        private ICreature Owner;

        public bool IsSelected(string s) {
            return Perks[s].selected_;
        }

        public void SetPerk(string id, bool state) {
            if(Perks[id].selected_ != state) {
                Perks[id].selected_ = state;
                PerkDataBase.Perks[id].AddRemove(state, Owner);
            }
        }

        public void SetPerk(string id)
        {
            Perks[id].selected_ = !Perks[id].selected_;
            var addRemove = PerkDataBase.Perks[id].AddRemove;
            if (addRemove != null) {
                addRemove(Perks[id].selected_, Owner);
            }
        }

        public PerksSystem(ICreature owner) {
            Owner = owner;
            Perks = new Dictionary<string, Perk>();
            for (int i = 0; i < PerkDataBase.Perks.Count; i++) {
                Perks.Add(PerkDataBase.Perks.ElementAt(i).Key, new Perk());
            }
        }
    }
}