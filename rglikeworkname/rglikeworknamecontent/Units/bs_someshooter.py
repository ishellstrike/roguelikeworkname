from rglikeworknamelib.Dungeon.Creatures import Order
from rglikeworknamelib.Dungeon.Creatures import OrderType
from rglikeworknamelib.Dungeon.Creatures import Creature
from rglikeworknamelib.Dungeon.Bullets import BulletSystem

class __innerData:
	def __init__(self, t, n="some"):
		self.time = t
		self.name = n

def BehaviorInit(target, rnd):
	target.BehaviorTag = __innerData(rnd.Next(1,1000))

def BehaviorScript(gt, ms_, hero, target, rnd):
	if target.BehaviorTag.time <= 0:
		target.BehaviorTag = __innerData(rnd.Next(1,1000))
		target.Say(target, "reset")
		target.Shoot(rnd.Next(0,628)/100.0)

	else:
		target.BehaviorTag.time -= gt.ElapsedGameTime.TotalMilliseconds


	if target.IsIddle:
		target.IssureOrder();


