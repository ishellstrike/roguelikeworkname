using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Effects {
    public interface IBuff {
        Creature Target { get; set; }
        TimeSpan Expire { get; set; }
        string Id { get; set; }
        bool RemoveFromTarget(Player p);
        bool ApplyToTarget(Player p);
        void Update(GameTime gt);
        bool Applied { get; }
    }
}