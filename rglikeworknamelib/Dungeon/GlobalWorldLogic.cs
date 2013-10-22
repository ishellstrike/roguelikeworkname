using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon {
    internal enum Seasons {
        Winter,
        Summer,
        Vesna,
        Osen
    }

    public enum DayPart {
        Day,
        Night
    }

    public static class GlobalWorldLogic {
        public static DateTime CurrentTime = new DateTime(2020, 6, 1, 12, 0, 0);
        public static TimeSpan Elapse;
        public static float Temperature = 10;
        private static Seasons currentSeason_ = Seasons.Summer;
        public static DayPart DayPart = DayPart.Day;
        private static readonly long Hour3 = new TimeSpan(0, 3, 0, 0).Ticks;
        private static float ler_ = 7;

        public static float GetCurrentSlen() {
            return ler_;
        }

        public static void Update(GameTime gt) {
            Elapse = TimeSpan.FromMinutes(gt.ElapsedGameTime.TotalSeconds);
            CurrentTime += Elapse;
            Temperature -= Temperature*(float) gt.ElapsedGameTime.TotalMinutes;
            Temperature += GetNormalTemp(CurrentTime)*(float) gt.ElapsedGameTime.TotalMinutes;

            DayPart = CurrentTime.Hour > 22 || CurrentTime.Hour < 7 ? DayPart.Night : DayPart.Day;

            float a = 3;
            switch (DayPart) {
                case DayPart.Day:
                    a = 12f;
                    break;
                case DayPart.Night:
                    a = 2f;
                    break;
            }
            ler_ = Vector2.Lerp(new Vector2(ler_, 0), new Vector2(a, 0), (float) Elapse.TotalHours).X;


            switch (currentSeason_) {
                case Seasons.Osen:
                    if (CurrentTime.Month >= 11) {
                        currentSeason_ = Seasons.Winter;
                        if (onWinterBegins != null) {
                            onWinterBegins(null, null);
                        }
                    }
                    break;
                case Seasons.Winter:
                    if (CurrentTime.Month > 3) {
                        currentSeason_ = Seasons.Vesna;
                        if (onVesnaBegins != null) {
                            onVesnaBegins(null, null);
                        }
                    }
                    break;
                case Seasons.Vesna:
                    if (CurrentTime.Month >= 11) {
                        currentSeason_ = Seasons.Summer;
                        if (onSummerBegins != null) {
                            onSummerBegins(null, null);
                        }
                    }
                    break;
                case Seasons.Summer:
                    if (CurrentTime.Month > 3) {
                        currentSeason_ = Seasons.Osen;
                        if (onOsenBegins != null) {
                            onOsenBegins(null, null);
                        }
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

        public static float GetNormalTemp(DateTime cur) {
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

            if (IsNight(cur)) {
                return temp - 6;
            }

            return temp;
        }

        public static bool IsWinter(DateTime cur) {
            return currentSeason_ == Seasons.Winter;
        }

        public static bool IsNight(DateTime cur) {
            return DayPart == DayPart.Night;
        }

        public static bool IsDay(DateTime cur) {
            return DayPart == DayPart.Day;
        }

        public static string GetTimeString(DateTime time) {
            if (Settings.IsAMDM) {
                if (time.Hour == 0) {
                    return string.Format("PM {0}:{1:00}:{2:00}", 12, time.Minute, time.Second);
                }
                return time.Hour <= 12
                           ? string.Format("AM {0}:{1:00}:{2:00}", time.Hour, time.Minute, time.Second)
                           : string.Format("PM {0}:{1:00}:{2:00}", time.Hour - 12, time.Minute, time.Second);
            }
            return string.Format("{0}:{1:00}:{2:00}", time.Hour, time.Minute, time.Second);
        }
    }
}