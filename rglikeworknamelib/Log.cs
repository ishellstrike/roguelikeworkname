using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using Microsoft.Xna.Framework;
using NLog.Config;
using rglikeworknamelib.Dungeon;
using NLog;

namespace rglikeworknamelib
{
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
        Equip
    }

    public struct LogEntity {
        public string message;
        public Color col;
        public LogEntityType type;

        public LogEntity(string mes, Color c, LogEntityType t = LogEntityType.Default) {
            message = mes;
            col = c;
            type = t;
        }

        public LogEntity(string mes, LogEntityType t = LogEntityType.Default)
        {
            message = mes;
            col = Color.White;
            type = t;
        }
    }
    public static class EventLog
    {
        public static List<LogEntity> log = new List<LogEntity>();

        public static void Add(string s, DateTime t, Color c, LogEntityType type)
        {
            log.Add(new LogEntity(GlobalWorldLogic.GetTimeString(t) + " " + s, c, type));

            if (log.Count > 100) { 
                log.RemoveAt(0);
            }

            UpdateEvent();
        }

        private static void UpdateEvent() {
            if (onLogUpdate != null) {
                    onLogUpdate(null, null);
                }
        }

        public static event EventHandler onLogUpdate;
    }
}
