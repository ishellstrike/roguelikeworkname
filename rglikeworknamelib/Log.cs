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
    public static class EventLog
    {
        public static List<string> log = new List<string>();
        public static List<Color> cols = new List<Color>();

        public static void Add(string s, DateTime t, Color c)
        {
            log.Add(GlobalWorldLogic.GetTimeString(t) + " " + s);
            cols.Add(c);

            if (log.Count > 100) { 
                log.RemoveAt(0);
                cols.RemoveAt(0);
            }

            UpdateEvent();
        }

        public static void Add(string s, DateTime t) {
            log.Add(string.Format("{0}:{1:00}:{2:00}",t.Hour,t.Minute,t.Second)+" "+s);

            if (log.Count > 100) log.RemoveAt(0);

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
