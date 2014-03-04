using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib
{
    public enum LogEntityType
    {
        OpenCloseDor,
        SeeSomething,
        Damage,
        Dies,
        Default,
        SelfDamage,
        NoAmmoWeapon,
        Consume,
        Buff,
        Equip,
        Console,
        Located
    }

    public class LogEntity
    {
        public Color col;
        internal string message;
        public LogEntityType type;
        internal const int LogSize = 512;
        internal int Repeat;
        internal DateTime Last;

        public LogEntity(string mes, Color c, DateTime dt, LogEntityType t = LogEntityType.Default)
        {
            message = mes;
            col = c;
            type = t;
            Repeat = 1;
            Last = dt;
        }

        public LogEntity(string mes, DateTime dt, LogEntityType entity = LogEntityType.Default)
        {
            message = mes;
            col = Color.White;
            type = entity;
            Repeat = 1;
            Last = dt;
        }

        public override string ToString() {
            if (Repeat == 1) {
                return string.Format("{0} {1}", Last.ToShortTimeString(), message);
            }
            return string.Format("{0} {1} x{2}", Last.ToShortTimeString(), message, Repeat);
        }
    }

    public static class EventLog
    {
        public static List<LogEntity> log = new List<LogEntity>(new[] {new LogEntity("Добро пожаловать в JARG", Color.LightCyan, new DateTime()), });

        public static void Add(string message, Color color, LogEntityType type)
        {
            Add(message, GlobalWorldLogic.CurrentTime, color, type);
        }

        public static void AddLocated(string message, Player p, Vector3 pos) {
            var a = p.Position/32/MapSector.Rx;
            var b = a - pos/MapSector.Rx;
            string s = "";

            if ((int)b.X == 0 && (int)b.Y == 0)
            {
                s = "Совсем рядом ";
            } else if (b.X > 0 && b.Y > 0)
            {
                s = "С юго-востока ";
            }
            else if (b.X > 0 && (int)b.Y == 0)
            {
                s = "С востока ";
            }
            else if (b.X > 0 && b.Y < 0)
            {
                s = "С северо-востока ";
            }
            else if ((int)b.X == 0 && b.Y > 0)
            {
                s = "С юга ";
            }
            else if ((int)b.X == 0 && b.Y < 0)
            {
                s = "С севера ";
            }
            else if (b.X < 0 && b.Y > 0)
            {
                s = "С юго-запада ";
            }
            else if (b.X < 0 && (int)b.Y == 0)
            {
                s = "С запада ";
            }
            else if (b.X < 0 && b.Y < 0)
            {
                s = "С северо-запада ";
            }
            Add(s+message, GlobalWorldLogic.CurrentTime, Color.White, LogEntityType.Located);
        }

        public static void Add(string message, DateTime globalDateTime, Color color, LogEntityType type)
        {
            if(log.Last().message == message) {
                log.Last().Repeat++;
                log.Last().Last = globalDateTime;
            }
            else {
                log.Add(new LogEntity(message, color, globalDateTime, type));


                if (log.Count > LogEntity.LogSize) {
                    log.RemoveAt(0);
                }
            }
            UpdateEvent();
        }

        private static void UpdateEvent()
        {
            if (OnLogUpdate != null)
            {
                OnLogUpdate(null, null);
            }
        }

        public static event EventHandler OnLogUpdate;
    }
}