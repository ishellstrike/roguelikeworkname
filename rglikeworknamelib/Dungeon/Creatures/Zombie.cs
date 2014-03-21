using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Creatures {
    class Zombie : Creature {
        public override void CreatureScript(Player hero) {
            if (IsIddle)
            {
                IssureOrder(new Order(OrderType.Wander));
            }
        }
    }
}