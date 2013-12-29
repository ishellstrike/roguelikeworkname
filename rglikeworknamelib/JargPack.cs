using System.Runtime.InteropServices;

namespace rglikeworknamelib {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct JargPack
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string action;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string name;

        public float x;
        public float y;
    }
}