using System;
using System.Collections.Generic;
using System.Linq;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Creatures {
    [Serializable]
    public class PerksSystem {
        private readonly Dictionary<string, Perk> Perks;
        [NonSerialized] internal Creature Owner;

        public PerksSystem(Creature owner) {
            Owner = owner;
            Perks = new Dictionary<string, Perk>();
            for (var i = 0; i < PerkDataBase.Perks.Count; i++) {
                Perks.Add(PerkDataBase.Perks.ElementAt(i).Key, new Perk());
            }
        }

        public bool IsSelected(string perkId) {
            return Perks[perkId].selected_;
        }

        /// <summary>
        ///     set perk state
        /// </summary>
        /// <param name="perkId"></param>
        /// <param name="state"></param>
        public void SetPerk(string perkId, bool state) {
            if (Perks[perkId].selected_ != state) {
                Perks[perkId].selected_ = state;
                PerkDataBase.Perks[perkId].AddRemove(state, Owner);
            }
        }

        /// <summary>
        ///     switch perk state
        /// </summary>
        /// <param name="perkId"></param>
        public void SetPerk(string perkId) {
            Perks[perkId].selected_ = !Perks[perkId].selected_;
            var addRemove = PerkDataBase.Perks[perkId].AddRemove;
            if (addRemove != null) {
                addRemove(Perks[perkId].selected_, Owner);
            }
        }
    }
}