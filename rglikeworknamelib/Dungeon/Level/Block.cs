using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class Block {
        public int id;
        public Color lightness;
        public bool explored;

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction) {
                case SmartAction.ActionOpenContainer:
                    return "Осмотреть содержимое";
                case SmartAction.ActionOpenClose:
                    return "Открыть/Закрыть";
                default:
                    return "Осмотреть";
            }
        }
    }
}