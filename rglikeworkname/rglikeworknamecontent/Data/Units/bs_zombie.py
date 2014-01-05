from rglikeworknamelib.Dungeon.Creatures import *

def BehaviorScript(gt, ms_, hero, target, rnd):
    if target.IsIddle:
        target.IssureOrder(Order(OrderType.Wander));
    
    worldPositionInBlocks = target.GetWorldPositionInBlocks();
    block = ms_.Parent.GetBlock(worldPositionInBlocks.X, worldPositionInBlocks.Y)
    if target.IsIddleOrWander and block is not None and block.IsVisible and target.reactionT_.TotalMilliseconds > target.Data.ReactionTime and Creature.GetLength(hero.Position.X - target.WorldPosition().X, hero.Position.Y - target.WorldPosition().Y) < 640:
        target.IssureOrder(hero)