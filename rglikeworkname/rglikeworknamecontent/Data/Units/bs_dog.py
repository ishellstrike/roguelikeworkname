from rglikeworknamelib.Dungeon.Creatures import *

def BehaviorScript(gt, ms_, hero, target, rnd):
    if target.behaviorTag is not None and target.behaviorTag > 0 and target.IsIddle:
        target.IssureOrder(rnd.Next(-128, 128) + hero.Position.X, rnd.Next(-128, 128) + hero.Position.Y)
        target.behaviorTag = target.behaviorTag - 1

    if target.IsIddle:
        a = rnd.Next(3) 
        if a == 1:
            target.IssureOrder(Order(OrderType.Wander))
        elif a == 2:
            target.IssureOrder(Order(OrderType.Sleep, rnd.Next(1000, 3000)))
        elif Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 256:
            target.IssureOrder(rnd.Next(-128, 128) + hero.Position.X, rnd.Next(-128, 128) + hero.Position.Y)
            target.behaviorTag = 10