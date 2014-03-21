using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Creatures {
    class Rabbit : Creature {
        private bool a;
        public override void CreatureScript(Player hero)
        {
            var p = WorldPosition();
            if (Vector3.Distance(hero.Position, p) < 128)
            {
                IssureOrder(GetInDirection(hero.Position.X, hero.Position.Y, p.X, p.Y, 256));
            }
            else if (IsIddle)
            {
                IssureOrder(new Order(OrderType.Wander));
            }
        }
    }
}