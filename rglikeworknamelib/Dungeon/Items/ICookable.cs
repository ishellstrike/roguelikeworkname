using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    public interface ICookable {
        double CookProgress { get; }
        void GiveHeat(double value, Player p, Vector3 secpos);
    }
}