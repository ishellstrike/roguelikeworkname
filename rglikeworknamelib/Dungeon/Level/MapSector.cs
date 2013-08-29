using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Generation;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class MapSector {
        /// <summary>
        /// Sector ox size
        /// </summary>
        public const int Rx = 32;

        /// <summary>
        /// Sectro oy size
        /// </summary>
        public const int Ry = 32;

        public bool ready;

        public GameLevel Parent;
        private BackgroundWorker bw;

        public int SectorOffsetX, SectorOffsetY;

        internal List<IBlock> Blocks;
        internal Floor[] Floors;
        internal List<ICreature> creatures;
        internal List<Particle> decals;
        internal List<Vector2> initialNodes;
        internal SectorBiom biom;

        public MapSector(int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;

            Blocks = new List<IBlock>(Rx * Ry);
            Floors = new Floor[Rx * Ry];
            creatures = new List<ICreature>();

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }

            initialNodes = new List<Vector2>();
        }

        public MapSector(GameLevel parent, int sectorOffsetX, int sectorOffsetY)
        {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;
            Parent = parent;

            Blocks = new List<IBlock>(Rx * Ry);
            Floors = new Floor[Rx * Ry];
            creatures = new List<ICreature>();
            decals = new List<Particle>();

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks.Add(new Block());
            }
            initialNodes = new List<Vector2>();
        }

        /// <summary>
        /// Map from just serialized data
        /// </summary>
        /// <param name="bdb"></param>
        /// <param name="fdb"></param>
        /// <param name="sdb"></param>
        /// <param name="sectorOffsetX"></param>
        /// <param name="sectorOffsetY"></param>
        /// <param name="blocksArray"></param>
        /// <param name="floorsArray"></param>
        public MapSector(GameLevel parent, object sectorOffsetX, object sectorOffsetY, object blocksArray, object floorsArray, object initialn, object obiom, object creat, object decal)
        {
            SectorOffsetX = (int)sectorOffsetX;
            SectorOffsetY = (int)sectorOffsetY;
            Parent = parent;

            Blocks = blocksArray as List<IBlock>;
            Floors = floorsArray as Floor[];
            initialNodes = initialn as List<Vector2>;
            biom = (SectorBiom)obiom;
            creatures = (List<ICreature>)creat;
            decals = (List<Particle>)decal;
        }

        public List<StorageBlock> GetStorageBlocks()
        {
            return (from a in Blocks
                    where BlockDataBase.Data[a.Id].BlockPrototype == typeof(StorageBlock)
                    select a as StorageBlock).ToList();
        }

        public void ExploreAllSector()
        {
            foreach (var b in Blocks) {
                b.Explored = true;
                b.Lightness = Color.White;
            }
        }

        /// <summary>
        /// Main sector generation proc
        /// </summary>
        /// <param name="mapseed">глобальный сид карты</param>
        public void Rebuild(int mapseed) {
            Action<int> a = AsyncGeneration;
            //a.BeginInvoke(mapseed, null, null);
            a(mapseed);
        }

        /// <summary>
        /// ###---### Main generation proc ###---###
        /// </summary>
        /// <param name="mapseed">Global map seed</param>
        private void AsyncGeneration(int mapseed) {
            int s = (int)(MapGenerators.Noise2D(SectorOffsetX,SectorOffsetY)*int.MaxValue);
            Random rand = new Random(s);
            MapGenerators.FillTest1(this, 1);
            MapGenerators.ClearBlocks(this);
            MapGenerators.FloorPerlin(this);

            var tt = Parent.GetDownN(SectorOffsetX, SectorOffsetY);
            if(tt != null)
                foreach (var ino in tt.initialNodes) {
                    if(ino.Y == 0) AddInitialNode(ino.X, Ry - 1);
                }

            
            tt = Parent.GetUpN(SectorOffsetX, SectorOffsetY);
            if (tt != null)
                foreach (var ino in tt.initialNodes)
                {
                    if (ino.Y == Ry - 1) AddInitialNode(ino.X, 0);
                }

            
            tt = Parent.GetLeftN(SectorOffsetX, SectorOffsetY);
            if (tt != null)
                foreach (var ino in tt.initialNodes)
                {
                    if (ino.X == 0) AddInitialNode(Rx - 1, ino.Y);
                }

            
            tt = Parent.GetRightN(SectorOffsetX, SectorOffsetY);
            if (tt != null)
                foreach (var ino in tt.initialNodes)
                {
                    if (ino.X == Rx - 1) AddInitialNode(Rx - 1, ino.Y);
                }

            MapGenerators.GenerateStreetGrid(this, initialNodes.ToArray(), rand);

            biom = (SectorBiom)rand.Next(0, 5);

            switch (biom) {
                case SectorBiom.Bushland:
                    for (int i = 0; i < rand.Next(14, 40); i++ ) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "bbochka");
                    break;

                case SectorBiom.Forest:
                    for (int i = 0; i < rand.Next(12, 40); i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < rand.Next(12, 40); i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < rand.Next(12, 40); i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;
            }

            for (int i = 0; i < 1; i++) {
                MapGenerators.PlaceRandomSchemeByType(this, SchemesType.house, rand.Next(0,Rx-1),rand.Next(0,Ry-1), rand);
            }

            var sb = GetStorageBlocks();

            foreach (var block in sb) {
                for (int i = 0; i < rand.Next(0,3); i++) {
                    block.StoredItems.Add(new Item.Item(ItemDataBase.data.ElementAt(rand.Next(0, ItemDataBase.data.Count)).Key, rand.Next(1, 2)));
                }
            }

            for (int i = 1; i< rand.Next(0, 4); i++ ) {
                Spawn("zombie1", rand);
            }

            Parent.generated++;
            ready = true;
        }

        private void Spawn(string i, Random rnd) {
            Spawn(i, rnd.Next(0, Rx), rnd.Next(0, Ry));
        }

        private void Spawn(string i, int x, int y) {
            var n = (ICreature) Activator.CreateInstance(MonsterDataBase.Data[i].CreaturePrototype);
            n.Position = new Vector2(x*32, y*32);
            n.Id = i;
            creatures.Add(n);
        }

        public IBlock GetBlock(int x, int y)
        {
            return Blocks[x * Ry + y];
        }

        public IBlock GetBlock(int a)
        {
            return Blocks[a];
        }

        public bool IsExplored(int x, int y)
        {
            return Blocks[x * Ry + y].Explored;
        }

        public bool IsExplored(int a)
        {
            return Blocks[a].Explored;
        }

        public bool IsWalkable(int x, int y)
        {
            return BlockDataBase.Data[Blocks[x * Ry + y].Id].IsWalkable;
        }

        public void SetFloor(int x, int y, int id)
        {
            Floors[x * Ry + y].Id = id;
            Floors[x * Ry + y].Mtex = FloorDataBase.Data[id].RandomMtexFromAlters();
        }

        public string GetId(int x, int y)
        {
            return Blocks[x * Ry + y].Id;
        }

        public string GetId(int a)
        {
            return Blocks[a].Id;
        }

        /// <summary>
        /// Base standart block setter
        /// </summary>
        /// <param name="oneDimCoord"></param>
        /// <param name="id"></param>
        public void SetBlock(int oneDimCoord, string id) {
            Blocks[oneDimCoord] = (IBlock)Activator.CreateInstance(BlockDataBase.Data[id].BlockPrototype);
            Blocks[oneDimCoord].Id = id;
            Blocks[oneDimCoord].Mtex = BlockDataBase.Data[id].RandomMtexFromAlters();
        }

        /// <summary>
        /// Base advansed block setter
        /// </summary>
        /// <param name="oneDimCoord"></param>
        /// <param name="id"></param>
        public void SetBlock(int oneDimCoord, IBlock bl)
        {
            Blocks[oneDimCoord] = bl;
           
            Blocks[oneDimCoord].Mtex = BlockDataBase.Data[bl.Id].RandomMtexFromAlters();
        }

        public void SetBlock(int posX, int posY, string id)
        {
            SetBlock(posX * Ry + posY, id);
        }

        public void SetBlock(Vector2 pos, string id)
        {
            SetBlock((int)pos.X, (int)pos.Y, id);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if (BlockDataBase.Data[Blocks[x * Rx + y].Id].SmartAction == SmartAction.ActionOpenClose)
            {
                SetBlock(x, y, BlockDataBase.Data[Blocks[x * Ry + y].Id].AfterDeathId);
            }
        }

        public void AddInitialNode(float p0, float p1) {
            initialNodes.Add(new Vector2(p0, p1));
        }

        public void CreateAllMapFromArray(string[] arr)
        {
            for (int i = 0; i < Rx; i++)
            {
                for (int j = 0; j < Ry; j++)
                {
                    SetBlock(i, j, arr[i]);
                }
            }
        }

        public string GetMtex(int i, int i1) {
            return Blocks[i*Rx + i1].Mtex;
        }

        public void AddDecal(Particle particle) {
            decals.Add(particle);
            if(decals.Count > 256) {
                decals.RemoveAt(0);
            }
        }
    }
}