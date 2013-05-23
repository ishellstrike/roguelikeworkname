using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Dungeon
{
    public static class GlobalWorldLogic
    {
        static Random rnd = new Random();
        static DateTime currentTime = new DateTime(2020, rnd.Next(1, 13), 1);
        static float temperature = 10;

        public static float GetNormalTemp(DateTime cur)
        {
            float temp = 0;
            switch (cur.Month) {
                case 1:
                    temp = -6;
                    break;
                case 2:
                    temp = -4.2f;
                    break;
                case 3:
                    temp = -1.5f;
                    break;
                case 4:
                    temp = 10.4f;
                    break;
                case 5:
                    temp = 18.4f;
                    break;
                case 6:
                    temp = 21.7f;
                    break;
                case 7:
                    temp = 21.1f;
                    break;
                case 8:
                    temp = 21.5f;
                    break;
                case 9:
                    temp = 15.4f;
                    break;
                case 10:
                    temp = 8.2f;
                    break;
                case 11:
                    temp = 1.1f;
                    break;
                case 12:
                    temp = -3.5f;
                    break;
            }

            if (IsNight(cur)) return temp - 6;

            return temp;
        }

        public static bool IsWinter(DateTime cur)
        {
            return cur.Month >= 11 || cur.Month <= 3;
        }

        public static bool IsNight(DateTime cur)
        {
            return cur.TimeOfDay <= GetSunriseTime(cur) || cur.TimeOfDay >= GetSunsetTime(cur);
        }

        public static bool IsDay(DateTime cur)
        {
            return !IsNight(cur);
        }

        public static TimeSpan GetSunriseTime(DateTime cur)
        {
            switch (cur.Month) {
                case 1:
                    return new TimeSpan(9, 31, 0);
                case 2:     
                    return new TimeSpan(8, 23, 0);
                case 3:       
                    return new TimeSpan(7, 11, 0);
                case 4:     
                    return new TimeSpan(5, 53, 0);
                case 5:        
                    return new TimeSpan(4, 54, 0);
                case 6:        
                    return new TimeSpan(4, 47, 0);
                case 7:        
                    return new TimeSpan(5, 27, 0);
                case 8:        
                    return new TimeSpan(6, 26, 0);
                case 9:        
                    return new TimeSpan(7, 26, 0);
                case 10:       
                    return new TimeSpan(8, 27, 0);
                case 11:       
                    return new TimeSpan(9, 30, 0);
                case 12:       
                    return new TimeSpan(10, 0, 0);

            }
            return new TimeSpan();
        }


        public static TimeSpan GetSunsetTime(DateTime cur)
        {
            switch (cur.Month) {
                case 1:
                    return new TimeSpan(17, 55, 0);
                case 2:        
                    return new TimeSpan(19, 2, 0);
                case 3:        
                    return new TimeSpan(20, 0, 0);
                case 4:        
                    return new TimeSpan(21, 2, 0);
                case 5:        
                    return new TimeSpan(21, 57, 0);
                case 6:        
                    return new TimeSpan(22, 18, 0);
                case 7:        
                    return new TimeSpan(21, 44, 0);
                case 8:        
                    return new TimeSpan(20, 34, 0);
                case 9:        
                    return new TimeSpan(19, 30, 0);
                case 10:       
                    return new TimeSpan(17, 59, 0);
                case 11:       
                    return new TimeSpan(17, 5, 0);
                case 12:       
                    return new TimeSpan(17, 3, 0);
            }
            return new TimeSpan();
        }
    }
}
