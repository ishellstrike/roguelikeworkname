from rglikeworknamelib.Dungeon.Creatures import *

def BehaviorInit(target, rnd):
	pass

def BehaviorScript(gt, ms_, hero, target, rnd):
    if target.BehaviorTag is not None and target.BehaviorTag > 0 and target.IsIddle:
        target.IssureOrder(rnd.Next(-128, 128) + hero.Position.X, rnd.Next(-128, 128) + hero.Position.Y);
        if target.BehaviorTag == 9:
            if rnd.Next(1) == 0:
                target.Say("Woof")
            else:
                target.Say("Woof-woof")
        target.BehaviorTag = target.BehaviorTag - 1
    
    if target.IsIddle:
        target.BehaviorTag = True;
        a = rnd.Next(3) 
        if a == 1:
            target.IssureOrder(Order(OrderType.Wander))
        elif a == 2:
            target.IssureOrder(Order(OrderType.Sleep, rnd.Next(1000, 3000)))
        elif Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 256:
            target.IssureOrder(rnd.Next(-128, 128) + hero.Position.X, rnd.Next(-128, 128) + hero.Position.Y)
            target.BehaviorTag = 10