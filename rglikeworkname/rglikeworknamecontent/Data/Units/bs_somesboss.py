from rglikeworknamelib.Dungeon.Creatures import Order
from rglikeworknamelib.Dungeon.Creatures import OrderType
from rglikeworknamelib.Dungeon.Creatures import Creature
from rglikeworknamelib.Dungeon.Bullets import BulletSystem

class __innerData:
	def __init__(self):
		self.ultimate = 10000
		self.phase = 1
		self.phasetime = 5000

def BehaviorInit(target, rnd):
	target.BehaviorTag = __innerData()

def BehaviorScript(gt, ms_, hero, target, rnd):
	target.BehaviorTag.phasetime -= gt.ElapsedGameTime.TotalMilliseconds

	if target.BehaviorTag.phasetime <= 0:
		target.BehaviorTag.phasetime = 5000
		target.BehaviorTag.phase += 1
		target.IssureOrder()
	
	if target.BehaviorTag.phase == 1 and target.IsIddle:
		target.Say("phase 1 move")
		target.IssureOrder(target.WorldPosition().X + rnd.Next(-200,200), target.WorldPosition().Y + rnd.Next(-200,200))

	if target.BehaviorTag.phase == 2 and target.IsIddle:
		target.Say("phase 2 stand")
		target.IssureOrder(1000)

	if target.BehaviorTag.phase >= 3:
		target.BehaviorTag.ultimate -= gt.ElapsedGameTime.TotalMilliseconds
	if target.BehaviorTag.phase >= 3 and target.IsIddle:
		target.Say("phase 3 fire ult:" + str(target.BehaviorTag.ultimate))
		
		if target.BehaviorTag.ultimate <= 0:
			target.Say("phase 3 ULTIMATE")
			target.BehaviorTag.ultimate = 10000
			target.Shoot(rnd.Next(0,628)/100.0)
			target.Shoot(rnd.Next(0,628)/100.0)
			target.Shoot(rnd.Next(0,628)/100.0)
			target.Shoot(rnd.Next(0,628)/100.0)
			target.Shoot(rnd.Next(0,628)/100.0)
			target.Shoot(rnd.Next(0,628)/100.0)
		target.Shoot(rnd.Next(0,628)/100.0)
		target.Shoot(rnd.Next(0,628)/100.0)
		target.IssureOrder(300)