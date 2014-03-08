using jarg;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockDoor : Block, IDoor {
        public void OpenClose(MapSector ms) {
            if (Data.IsWalkable)
            {
                EventLog.Add("�� ������� �����", LogEntityType.OpenCloseDor);
            }
            else
            {
                EventLog.Add("�� ������� �����", LogEntityType.OpenCloseDor);
                AchievementDataBase.Stat["dooro"].Count++;
            }
            Id = Data.AfterDeathId;
            MTex = Data.MTex;
            ms.GeomReady = false;
        }
    }
}