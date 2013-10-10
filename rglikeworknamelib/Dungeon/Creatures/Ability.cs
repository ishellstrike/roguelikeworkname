using System;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class Ability {
        public static readonly int[] XpNeeds = new[] { 10, 25, 40, 60, 85, 115, 155, 200, 265, 340, 435, 555, 700, 890, 1120, 9999};
        private double xpCurrent_ = 0;
        public int XpLevel = 2;
        public string Name;
        public event EventHandler OnLevelUp;
        public AbilityNameStyle NameStyle;

        public double XpCurrent
        {
            get
            {
                return xpCurrent_;
            }
            set
            {
                xpCurrent_ = value;
                if (xpCurrent_ > XpNeeds[XpLevel] && XpLevel < 10) {
                    xpCurrent_ -= XpNeeds[XpLevel];
                    XpLevel++;
                    if (OnLevelUp != null) {
                        OnLevelUp(null, null);
                    }
                }
            }
        }

        
        public override string ToString() {
            //if (nameStyle == AbilityNameStyle.physical) {
                switch (XpLevel) {
                    case 0:
                        return "ужасно";
                    case 1:
                        return "дилетант";
                    case 2:
                        return "новичок";
                    case 3:
                        return "ученик";
                    case 4:
                        return "специалист";
                    case 5:
                        return "профессионал";
                    case 6:
                        return "эксперт";
                    case 7:
                        return "выдающийся эксперт";
                    case 8:
                        return "мастер";
                    case 9:
                        return "великий мастер";
                    case 10:
                        return "легенда";
               // }
            }

            return XpCurrent.ToString();
        }
    }
}