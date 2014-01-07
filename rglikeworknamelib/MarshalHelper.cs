using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace rglikeworknamelib {
    public static class MarshalHelper
    {
        static BinaryFormatter bf = new BinaryFormatter();

        static JargPack error = new JargPack {action = "error"};
        public static JargPack DeserializeMsg(Byte[] data)
        {
            JargPack a = new JargPack();
            try {
                //a = (JargPack) bf.Deserialize(ms);
                string geted = Encoding.ASCII.GetString(data);
                var parts = geted.Split('@');
                a.action = parts[0];
                a.name = parts[1];
                a.x = float.Parse(parts[2]);
                a.y = float.Parse(parts[3]);
                a.angle = float.Parse(parts[4]);
                a.mapsector = parts[5];
            }
            catch (Exception) {
                return error;
            }
            return a;
        }
        public static Byte[] SerializeMessage(JargPack msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}@{1}@{2}@{3}@{4}@{5}", msg.action, msg.name, msg.x, msg.y, msg.angle, msg.mapsector);
            var data = Encoding.ASCII.GetBytes(sb.ToString());
            
            return data;
        }
    }
}