using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jarg
{
    public static class Version {
        public static int Major = 0;
        public static int Cores = 9;
        public static int Minor = 1;
        public static int Commit = 21;

        public static string GetShort() {
            return string.Format("v{0}.{1}.{2}",Major,Cores,Minor);
        }
        public static string GetLong() {
            return string.Format("v{0}.{1}.{2}-{3}", Major, Cores, Minor,Commit);
        }

        public static string GetMeaning() {
            return "v<major>.<cores count>.<minor>.<git commit number>";
        }
    }
}
