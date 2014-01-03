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
        Attack
    }

    public class Order
    {
        public Order()
        {
            Type = OrderType.Iddle;
        }

        public Order(OrderType orderType, Vector2 value)
        {
            Type = orderType;
            Point = value;
        }

        public Order(OrderType orderType, Creature value)
        {
            Type = orderType;
            Target = value;
        }

        public OrderType Type;

        public Vector2 Point;

        public Creature Target;
    }
}
