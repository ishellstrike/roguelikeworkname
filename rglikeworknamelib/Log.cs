using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon;

namespace rglikeworknamelib {
    public enum LogEntityType {
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
        Console
    }

    public struct LogEntity {
        public Color col;
        public string message;
        public LogEntityType type;

        public LogEntity(string mes, Color c, LogEntityType t = LogEntityType.Default) {
            message = mes;
            col = c;
            type = t;
        }

        public LogEntity(string mes, LogEntityType entity = LogEntityType.Default) {
            message = mes;
            col = Color.White;
            type = entity;
        }
    }

    public static class EventLog {
        public static List<LogEntity> log = new List<LogEntity>();

        public static void Add(string message, Color color, LogEntityType type) {
            Add(message, GlobalWorldLogic.CurrentTime, color, type);
        }

        public static void Add(string message, DateTime globalDateTime, Color color, LogEntityType type) {
            log.Add(new LogEntity(GlobalWorldLogic.GetTimeString(globalDateTime) + " " + message, color, type));

            if (log.Count > 100) {
                log.RemoveAt(0);
            }

            UpdateEvent();
        }

        private static void UpdateEvent() {
            if (OnLogUpdate != null) {
                OnLogUpdate(null, null);
            }
        }

        public static event EventHandler OnLogUpdate;
    }
}