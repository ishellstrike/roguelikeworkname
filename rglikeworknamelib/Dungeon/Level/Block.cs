using System;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Block {

        public string Id {
            get { return Data.Id; }
            set { Data = Registry.Instance.Blocks[value]; }
        }

        public BlockData Data {
            get { return data_; }
            private set { data_ = value; }
        }

        public Vector2 Source {
            get { return source_; }
        }
        
        [NonSerialized]
        private BlockData data_;

        public virtual void Update(TimeSpan ts, MapSector ms, Player p)
        {
        }

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction)
            {
                case SmartAction.ActionOpenContainer:
                    return "Îñìîòðåòü ñîäåðæèìîå";
                case SmartAction.ActionOpenClose:
                    return "Îòêðûòü/Çàêðûòü";
                default:
                    return "Îñìîòðåòü";
            }
        }
    }
}
