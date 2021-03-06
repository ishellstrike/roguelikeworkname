from rglikeworknamelib.Dungeon.Creatures import Order
from rglikeworknamelib.Dungeon.Creatures import OrderType
from rglikeworknamelib.Dungeon.Creatures import Creature

def BehaviorInit(target, rnd):
	pass

def BehaviorScript(gt, ms_, hero, target, rnd):
	if Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 128:
		ta = target.WorldPosition()
		b = Creature.GetInDirection(hero.Position.X, hero.Position.Y, ta.X, ta.Y, 256)
		target.IssureOrder(b.X, b.Y)
		if target.BehaviorTag == True:
			target.Say("Eeeeee!")
			target.BehaviorTag = False

	if target.IsIddle:
		target.BehaviorTag = True;
		if rnd.Next(3) == 1:
			target.IssureOrder(Order(OrderType.Wander))
		else:
			target.IssureOrder(Order(OrderType.Sleep, rnd.Next(1000,10000)))