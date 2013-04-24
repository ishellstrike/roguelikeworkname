using System;
using System.IO;

namespace rglikeworknamelib
{
    public static class Log
    {
        public static string[] log = new string[100];

        public static void Init() {
            for (int i = 0; i < log.Length; i++) {
                log[i] = "";
            }
        }

        public static void AddString(string s) {
            for (int i = 1; i < log.Length; i++) {
                log[i-1] = log[i];
            }
            log[99] = s;
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
}
