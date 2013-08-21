using System;
using rglikeworknamelib;

namespace jarg
{
    public static class Version {
        private static DateTime date;
        private static string name;

        public static string GetShort() {
            return string.Format("{0}", name);
        }
        public static string GetLong() {
            return string.Format("{0} от {1}", name, date);
        }

        public static void Init() {
            date = TimeFromUnix(long.Parse(AutoVersion.Time));
            name = AutoVersion.Version;
            name = name.Remove(name.LastIndexOf('-'), name.Length - name.LastIndexOf('-'));
        }

        public static DateTime TimeFromUnix(long timestamp) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static string GetMeaning() {
            return "v<release>.<major>.<cores count>.<minor>-<git commit number>";
        }
    }
}
