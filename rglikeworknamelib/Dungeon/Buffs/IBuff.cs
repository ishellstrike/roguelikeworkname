using System;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Effects {
    interface IBuff {
        Creature Target { get; set; }
        TimeSpan Expire { get; set; }
        string Id { get; set; }
        bool RemoveFromTarget();
        bool ApplyToTarget();
        bool Applied { get; }
    }
}