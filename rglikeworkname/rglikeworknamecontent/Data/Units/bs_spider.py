from rglikeworknamelib.Dungeon.Creatures import Order
from rglikeworknamelib.Dungeon.Creatures import OrderType
from rglikeworknamelib.Dungeon.Creatures import Creature

def BehaviorInit(target, rnd):
	pass

def BehaviorScript(gt, ms_, hero, target, rnd):
	if Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 600 and target.behaviorTag == 1:
		ta = target.WorldPosition()
		b = Creature.GetInDirection(hero.Position.X, hero.Position.Y, ta.X, ta.Y, -300)
		target.IssureOrder(b.X, b.Y)
		target.BehaviorTag = 2
		target.Say("Sssss")
		
	if target.IsIddle and target.BehaviorTag == 2:
		target.IssureOrder(Order(OrderType.Sleep, 5000))
		target.BehaviorTag = 3

	if target.IsIddle and target.BehaviorTag == 3:
		target.IssureOrder(hero)
		target.BehaviorTag = 4
		target.Say("Shhhhhhhh")

	if target.IsIddle:
		target.BehaviorTag = 1
		if rnd.Next(3) == 1:
			target.IssureOrder(Order(OrderType.Wander))
		else:
			target.IssureOrder(Order(OrderType.Sleep, rnd.Next(1000,2000)))