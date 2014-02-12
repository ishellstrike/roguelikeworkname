using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace jarg
{
    class JargBind {
        public Keys Key, AltKey;
        public string Name;
        public Keys Default;

        public static implicit operator Keys(JargBind a) {
            return a.Key;
        }
    }

    static class JargBindings {
        public static JargBind Inventory = new JargBind(), Caracter = new JargBind(), Statist = new JargBind(), Achievements = new JargBind(), Debug = new JargBind(), Wire = new JargBind(), TakeAll = new JargBind();
        public static List<JargBind> All = new List<JargBind>(); 

        static JargBindings() {
            All.AddRange(new[] {Inventory, Caracter, Statist, Achievements, Debug, Wire, TakeAll});
            Inventory.Name = "Инвентарь";
            Inventory.Default = Keys.I;

            Caracter.Name = "Окно персонажа";
            Caracter.Default = Keys.C;

            Statist.Name = "Окно статистики";
            Statist.Default = Keys.O;

            Achievements.Name = "Окно достижений";
            Achievements.Default = Keys.P;

            Debug.Name = "Отладочная информация";
            Debug.Default = Keys.F1;

            Wire.Name = "Wireframe";
            Wire.Default = Keys.F2;

            TakeAll.Name = "Взять все";
            TakeAll.Default = Keys.R;

            ResetDefault();
        }

        static void ResetDefault() {
            foreach (var jargBind in All) {
                jargBind.Key = jargBind.Default;
                jargBind.AltKey = Keys.None;
            }
        }
    }
}
