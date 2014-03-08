using jarg;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockDoor : Block, IDoor {
        public void OpenClose(MapSector ms) {
            if (Data.IsWalkable)
            {
                EventLog.Add("Вы закрыли дверь", GlobalWorldLogic.CurrentTime, Color.Gray,
                    LogEntityType.OpenCloseDor);
            }
            else
            {
                EventLog.Add("Вы открыли дверь", GlobalWorldLogic.CurrentTime, Color.LightGray,
                    LogEntityType.OpenCloseDor);
                AchievementDataBase.Stat["dooro"].Count++;
            }
            Id = Data.AfterDeathId;
            MTex = Data.MTex;
            ms.GeomReady = false;
        }
    }
}