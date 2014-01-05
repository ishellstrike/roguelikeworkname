from rglikeworknamelib.Dungeon.Creatures import Order
from rglikeworknamelib.Dungeon.Creatures import OrderType
from rglikeworknamelib.Dungeon.Creatures import Creature

def BehaviorScript(gt, ms_, hero, target, rnd):
	if Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 600 and target.behaviorTag == 1:
		ta = target.WorldPosition()
		b = Creature.GetInDirection(hero.Position.X, hero.Position.Y, ta.X, ta.Y, -300)
		target.IssureOrder(b.X, b.Y)
		target.behaviorTag = 2
		Creature.Say(target, "Sssss")
		
	if target.IsIddle and target.behaviorTag == 2:
		target.IssureOrder(Order(OrderType.Sleep, 5000))
		target.behaviorTag = 3

	if target.IsIddle and target.behaviorTag == 3:
		target.IssureOrder(hero)
		target.behaviorTag = 4
		Creature.Say(target, "Shhhhhhhh")

	if target.IsIddle:
		target.behaviorTag = 1
		if rnd.Next(3) == 1:
			target.IssureOrder(Order(OrderType.Wander))
		else:
			target.IssureOrder(Order(OrderType.Sleep, rnd.Next(1000,2000)))