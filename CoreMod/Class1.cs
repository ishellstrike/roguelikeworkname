using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Level;

namespace CoreMod
{
    public class CoreMod : JargMod
    {
        public override void Init() {
            
        }
    }

    public class BrickBlockData : NewBlockData {
        public BrickBlockData() {
            AlterMtex = new[] {"brick"};
        }
    }
}
