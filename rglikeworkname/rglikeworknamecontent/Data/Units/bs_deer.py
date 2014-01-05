from rglikeworknamelib.Dungeon.Creatures import Order
from rglikeworknamelib.Dungeon.Creatures import OrderType
from rglikeworknamelib.Dungeon.Creatures import Creature

def BehaviorScript(gt, ms_, hero, target, rnd):
	if Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 256:
		ta = target.WorldPosition()
		b = Creature.GetInDirection(hero.Position.X, hero.Position.Y, ta.X, ta.Y, 1000)
		target.IssureOrder(b.X, b.Y)

	if target.IsIddle:
		if rnd.Next(3) == 1:
			target.IssureOrder(Order(OrderType.Wander))
		else:
			target.IssureOrder(Order(OrderType.Sleep, rnd.Next(1000,10000)))