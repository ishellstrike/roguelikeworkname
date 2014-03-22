namespace rglikeworknamelib.Dungeon.Creatures {
    class Shooter : Creature
    {
        public override void CreatureScript(Player hero)
        {
            if (IsIddle)
            {
                IssureOrder(new Order(OrderType.Wander));
            }
        }
    }
}