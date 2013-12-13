using System;

namespace jarg {
    public class AchievementData {
        public DateTime Date;
        public string Description;
        public string Name;
        public AcievementType Type;

        public override string ToString() {
            return string.Format("{0} ({1})", Name, Date != DateTime.MinValue ? Date.Date.ToString() : "не получено");
        }
    }
}