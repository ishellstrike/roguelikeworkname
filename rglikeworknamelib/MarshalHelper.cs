using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace rglikeworknamelib {
    public static class MarshalHelper
    {
        static BinaryFormatter bf = new BinaryFormatter();

        static JargPack error = new JargPack {action = "error"};
        public static JargPack DeserializeMsg(Byte[] data)
        {
            JargPack retStruct = new JargPack();
            //int objsize = Marshal.SizeOf(typeof(T));
            //IntPtr buff = Marshal.AllocHGlobal(objsize);
            //Marshal.Copy(data, 0, buff, objsize);
            //T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));
            //Marshal.FreeHGlobal(buff);
            JargPack a;
            var ms = new MemoryStream(data);
            try {
                a = (JargPack) bf.Deserialize(ms);
            }
            catch (Exception) {
                return error;
            }
            return a;
        }
        public static Byte[] SerializeMessage(JargPack msg)
        {
            //int objsize = Marshal.SizeOf(typeof(T));
            //Byte[] ret = new Byte[objsize];
            //IntPtr buff = Marshal.AllocHGlobal(objsize);
            //Marshal.StructureToPtr(msg, buff, true);
            //Marshal.Copy(buff, ret, 0, objsize);
            //Marshal.FreeHGlobal(buff);
            var ms = new MemoryStream();
            bf.Serialize(ms, msg);
            return ms.GetBuffer();
        }
    }
}