using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Dungeon
{
    public class GlobalWorldLogic
    {
        static Random rnd = new Random();
        static DateTime currentTime = new DateTime(2020, rnd.Next(1, 13), 1);
        static float temperature = 10;

        public static float GetNormalTemp(DateTime cur)
        {
            //switch (cur.Month) {
            //    case 1:
            //        if cur.TimeOfDay.N
            //}
            return 1;
        }

        public DateTime GetSunriseTime(DateTime cur)
        {
            switch (cur.Month) {
                case 1:
                    return new DateTime(0, 0, 0, 9, 31, 0);
                case 2:
                    return new DateTime(0, 0, 0, 8, 23, 0);
                case 3:
                    return new DateTime(0, 0, 0, 7, 11, 0);
                case 4:
                    return new DateTime(0, 0, 0, 5, 53, 0);
                case 5:
                    return new DateTime(0, 0, 0, 4, 54, 0);
                case 6:
                    return new DateTime(0, 0, 0, 4, 47, 0);
                case 7:
                    return new DateTime(0, 0, 0, 5, 27, 0);
                case 8:
                    return new DateTime(0, 0, 0, 6, 26, 0);
                case 9:
                    return new DateTime(0, 0, 0, 7, 26, 0);
                case 10:
                    return new DateTime(0, 0, 0, 8, 27, 0);
                case 11:
                    return new DateTime(0, 0, 0, 9, 30, 0);
                case 12:
                    return new DateTime(0, 0, 0, 10, 0, 0);

            }
            return new DateTime();
        }


        public DateTime GetSunsetTime(DateTime cur)
        {
            switch (cur.Month) {
                case 1:
                    return new DateTime(0, 0, 0, 17, 55, 0);
                case 2:
                    return new DateTime(0, 0, 0, 19, 2, 0);
                case 3:
                    return new DateTime(0, 0, 0, 20, 0, 0);
                case 4:
                    return new DateTime(0, 0, 0, 21, 2, 0);
                case 5:
                    return new DateTime(0, 0, 0, 21, 57, 0);
                case 6:
                    return new DateTime(0, 0, 0, 22, 18, 0);
                case 7:
                    return new DateTime(0, 0, 0, 21, 44, 0);
                case 8:
                    return new DateTime(0, 0, 0, 20, 34, 0);
                case 9:
                    return new DateTime(0, 0, 0, 19, 30, 0);
                case 10:
                    return new DateTime(0, 0, 0, 17, 59, 0);
                case 11:
                    return new DateTime(0, 0, 0, 17, 5, 0);
                case 12:
                    return new DateTime(0, 0, 0, 17, 3, 0);
            }
            return new DateTime();
        }
    }
}
