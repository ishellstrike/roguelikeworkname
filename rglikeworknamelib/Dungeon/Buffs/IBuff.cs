using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Effects {
    public interface IBuff {
        Creature Target { get; set; }
        TimeSpan Expire { get; set; }
        bool Expiring { get; set; }
        string Id { get; set; }
        bool RemoveFromTarget(Creature p);
        bool ApplyToTarget(Creature p);
        void Update(GameTime gt);
        bool Applied { get; }
    }
}