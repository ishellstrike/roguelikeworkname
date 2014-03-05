using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Block {
        private string id_;

        public object ScriptTag;

        public string Id {
            get { return id_; }
            set {
                id_ = value;
                Data = Registry.Instance.Blocks[value];
            }
        }

        public BlockData Data {
            get { return data_; }
            private set { data_ = value; }
        }

        public Color Lightness { get; set; }

        public Vector2 Source { get; private set; }

        public bool IsVisible()
        {
            return Lightness == Color.White;
        }

        private string mTex_;
        [NonSerialized]
        private BlockData data_;

        public string MTex
        {
            get { return mTex_; }
            set
            {
                Source = Atlases.GetSource(value);
                mTex_ = value;
            }
        }

        public virtual void Update(TimeSpan ts, MapSector ms, Player p)
        {
        }

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction)
            {
                case SmartAction.ActionOpenContainer:
                    return "Осмотреть содержимое";
                case SmartAction.ActionOpenClose:
                    return "Открыть/Закрыть";
                default:
                    return "Осмотреть";
            }
        }
    }

    class BlockChest : Block, IItemStorage {
        private List<Item> itemList_ = new List<Item>();
        public List<Item> ItemList {
            get { return itemList_; }
            set { itemList_ = value; }
        }

        public bool RemoveItem(Item i) {
            if (ItemList.Contains(i))
            {
                ItemList.Remove(i);
                return true;
            }
            return false;
        }

        public bool AddItem(Item i) {
            ItemList.Add(i);
            return true;
        }
    }

    public interface IItemStorage {
        List<Item> ItemList { get; set; }
        bool RemoveItem(Item i);
        bool AddItem(Item i);
    }
}