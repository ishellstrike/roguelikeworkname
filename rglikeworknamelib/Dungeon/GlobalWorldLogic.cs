using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon
{
    enum Seasons {
        Winter,
        Summer,
        Vesna,
        Osen
    }

    public enum DayPart {
        Day,
        Morning,
        Vecher,
        Night
    }

    public static class GlobalWorldLogic
    {
        public static DateTime CurrentTime = new DateTime(2020, 6, 1, 12, 0, 0);
        private static TimeSpan elapse;
        public static float Temperature = 10;
        private static Seasons currentSeason_ = Seasons.Summer;
        public static DayPart dayPart_  = DayPart.Day;
        private static readonly long Hour3 = new TimeSpan(0,3,0,0).Ticks;
        private static float ler = 3;
        public static float GetCurrentSlen() {
            return ler;
        }

        public static void Update(GameTime gt) {
            elapse = TimeSpan.FromMinutes(gt.ElapsedGameTime.TotalSeconds*50);
            CurrentTime += elapse;
            Temperature -= Temperature * (float)gt.ElapsedGameTime.TotalMinutes;
            Temperature += GetNormalTemp(CurrentTime) * (float)gt.ElapsedGameTime.TotalMinutes;

            switch (dayPart_) {
                case DayPart.Day:
                    if (CurrentTime.TimeOfDay.Ticks > GetSunsetTime(CurrentTime).Ticks - Hour3) {
                        dayPart_ = DayPart.Vecher;
                        if(onVecherBegins != null) {
                            onVecherBegins(null, null);
                        }
                    }
                    break;
                case DayPart.Morning:
                    if (CurrentTime.TimeOfDay.Ticks > GetSunriseTime(CurrentTime).Ticks + Hour3) {
                        dayPart_ = DayPart.Day;
                        if(onDayBegins != null) {
                            onDayBegins(null, null);
                        }
                    }
                    break;
                case DayPart.Night:
                    if(CurrentTime.TimeOfDay.Ticks > GetSunriseTime(CurrentTime).Ticks) {
                        dayPart_ = DayPart.Morning;
                        if(onMorningBegins != null) {
                            onMorningBegins(null, null);
                        }
                    }
                    break;
                case DayPart.Vecher:
                    if(CurrentTime.TimeOfDay.Ticks > GetSunsetTime(CurrentTime).Ticks) {
                        dayPart_ = DayPart.Night;
                        if(onNightBegins != null) {
                            onNightBegins(null, null);
                        }
                    }
                    break;
            }

            float a = 3;
            switch (dayPart_) {
                    case DayPart.Day:
                    a = 0.1f;
                    break;
                    case DayPart.Morning:
                    a = 1;
                    break;
                    case DayPart.Vecher:
                    a = 4;
                    break;
                    case DayPart.Night:
                    a = 7;
                    break;
            }
            ler = Vector2.Lerp(new Vector2(ler, 0), new Vector2(a, 0), (float) elapse.TotalHours).X;
             

            switch (currentSeason_) {
                case Seasons.Osen:
                    if (CurrentTime.Month >= 11) {
                        currentSeason_ = Seasons.Winter;
                        if (onWinterBegins != null) onWinterBegins(null, null);
                    }
                    break;
                case Seasons.Winter:
                    if (CurrentTime.Month > 3) {
                        currentSeason_ = Seasons.Vesna;
                        if (onVesnaBegins != null) onVesnaBegins(null, null);
                    }
                    break;
                case Seasons.Vesna:
                    if (CurrentTime.Month >= 11) {
                        currentSeason_ = Seasons.Summer;
                        if (onSummerBegins != null) onSummerBegins(null, null);
                    }
                    break;
                case Seasons.Summer:
                    if (CurrentTime.Month > 3) {
                        currentSeason_ = Seasons.Osen;
                        if (onOsenBegins != null) onOsenBegins(null, null);
                    }
                    break;
            }
        }

        private static event EventHandler onNightBegins;
        private static event EventHandler onDayBegins;
        private static event EventHandler onMorningBegins;
        private static event EventHandler onVecherBegins;

        private static event EventHandler onWinterBegins;
        private static event EventHandler onSummerBegins;
        private static event EventHandler onOsenBegins;
        private static event EventHandler onVesnaBegins;

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
            return currentSeason_ == Seasons.Winter;
        }

        public static bool IsNight(DateTime cur)
        {
            return dayPart_ == DayPart.Night;
        }

        public static bool IsDay(DateTime cur)
        {
            return dayPart_ == DayPart.Day ;
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

        public static string GetTimeString(DateTime t) {
            if (Settings.IsAMDM)
            {
                if (t.Hour == 0) return string.Format("PM {0}:{1:00}:{2:00}", 12, t.Minute, t.Second);
                return t.Hour <= 12 ? string.Format("AM {0}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second) : string.Format("PM {0}:{1:00}:{2:00}", t.Hour - 12, t.Minute, t.Second);
            }
            else {
                return string.Format("{0}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);
            }
        }
    }
}
