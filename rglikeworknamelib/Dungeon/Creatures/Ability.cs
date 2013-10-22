using System;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class Ability {
        public static readonly int[] XpNeeds = new[]
        {10, 25, 40, 60, 85, 115, 155, 200, 265, 340, 435, 555, 700, 890, 1120, 9999};

        public string Name;
        public AbilityNameStyle NameStyle;
        public int XpLevel = 2;
        private double xpCurrent_;

        public double XpCurrent {
            get { return xpCurrent_; }
            set {
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

        public event EventHandler OnLevelUp;


        public override string ToString() {
            //if (nameStyle == AbilityNameStyle.physical) {
            switch (XpLevel) {
                case 0:
                    return "������";
                case 1:
                    return "��������";
                case 2:
                    return "�������";
                case 3:
                    return "������";
                case 4:
                    return "����������";
                case 5:
                    return "������������";
                case 6:
                    return "�������";
                case 7:
                    return "���������� �������";
                case 8:
                    return "������";
                case 9:
                    return "������� ������";
                case 10:
                    return "�������";
                    // }
            }

            return XpCurrent.ToString();
        }
    }
}