using System;
using System.Collections.Generic;
using System.Linq;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class PerksSystem {
        private Dictionary<string, Perk> Perks;
        private ICreature Owner;

        public bool IsSelected(string perkId) {
            return Perks[perkId].selected_;
        }

        public void SetPerk(string perkId, bool state) {
            if(Perks[perkId].selected_ != state) {
                Perks[perkId].selected_ = state;
                PerkDataBase.Perks[perkId].AddRemove(state, Owner);
            }
        }

        public void SetPerk(string perkId)
        {
            Perks[perkId].selected_ = !Perks[perkId].selected_;
            var addRemove = PerkDataBase.Perks[perkId].AddRemove;
            if (addRemove != null) {
                addRemove(Perks[perkId].selected_, Owner);
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