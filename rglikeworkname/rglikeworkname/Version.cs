using rglikeworknamelib;

namespace jarg
{
    public static class Version {
        public static string GetShort() {
            return string.Format("v{0}", AutoVersion.Version);
        }
        public static string GetLong() {
            return string.Format("v{0}", AutoVersion.Version);
        }

        public static void Init() {
            
        }

        public static string GetMeaning() {
            return "v<release>.<major>.<cores count>.<minor>-<git commit number>";
        }
    }
}
