using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon;

namespace rglikeworknamelib
{
    public static class Log
    {
        public static List<string> log = new List<string>();

        public static void Init() {
        }

        public static void AddString(string s) {
            log.Add(s);
        }

        public static void LogError(string s) {
            Directory.CreateDirectory("Errors\\");
            StreamWriter sw = new StreamWriter(string.Format("Errors\\{0}-{1}-{2}_{3}-{4}-{5}--error.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            sw.WriteLine(s);
            sw.Close();
            throw new Exception("logged error");
        }

        public static void SaveLog() {
            StreamWriter sw = new StreamWriter(string.Format("{0}-{1}-{2}_{3}-{4}-{5}--log.txt",DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year,DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second));
            foreach (var a in log) {
                if(a != "")
                sw.WriteLine(a);
            }
            sw.Close();
        }
    }

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
