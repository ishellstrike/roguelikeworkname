using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Creatures
{
    public enum OrderType
    {
        Iddle,
        Move,
        Attack,
        Wander,
        Sleep
    }

    public class Order
    {
        public Order()
        {
            Type = OrderType.Iddle;
        }

        public Order(OrderType orderType)
        {
            Type = orderType;
        }

        public Order(OrderType orderType, Vector2 value)
        {
            Type = orderType;
            Point = value;
        }

        public Order(OrderType orderType, float x, float y)
        {
            Type = orderType;
            Point = new Vector2(x,y);
        }

        public Order(OrderType orderType, Creature value)
        {
            Type = orderType;
            Target = value;
        }

        public Order(OrderType orderType, int p)
        {
            Type = orderType;
            Value = p;
        }

        public OrderType Type;

        public Vector2 Point;

        public Creature Target;

        public float Value;
    }
}
