using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Effects {
    public interface IBuff {
        Creature Target { get; set; }
        TimeSpan Expire { get; set; }
        bool Expiring { get; set; }
        string Id { get; set; }
        bool Applied { get; }
        bool RemoveFromTarget(Creature target);
        bool ApplyToTarget(Creature target);
        void Update(GameTime gt);
    }
}