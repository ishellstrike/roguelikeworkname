using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rglikeworknamelib.Generation;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Level {

    enum SectorBiom {
        Forest,
        Field,
        AgroField,
        Bushland
    }

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

        public BlockDataBase BlockDataBase;
        public FloorDataBase FloorDataBase;
        public SchemesDataBase SchemesDataBase;
        public GameLevel Parent;
        private BackgroundWorker bw;

        public int SectorOffsetX, SectorOffsetY;

        internal Block[] Blocks;
        internal Floor[] Floors;
        internal List<Vector2> initialNodes;
        internal SectorBiom biom;

        public MapSector(int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;

            Blocks = new Block[Rx * Ry];
            Floors = new Floor[Rx * Ry];

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }

            BlockDataBase = new BlockDataBase(new Dictionary<int, BlockData>());
            FloorDataBase = new FloorDataBase(new Dictionary<int, FloorData>());
            SchemesDataBase = new SchemesDataBase(new List<Schemes>());
            initialNodes = new List<Vector2>();
        }

        public MapSector(BlockDataBase bdb, FloorDataBase fdb, SchemesDataBase sdb, GameLevel parent, int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;
            BlockDataBase = bdb;
            FloorDataBase = fdb;
            SchemesDataBase = sdb;
            Parent = parent;

            Blocks = new Block[Rx * Ry];
            Floors = new Floor[Rx * Ry];

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
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
        public MapSector(BlockDataBase bdb, FloorDataBase fdb, SchemesDataBase sdb, GameLevel parent, object sectorOffsetX, object sectorOffsetY, object blocksArray, object floorsArray, object initialn, object obiom)
        {
            SectorOffsetX = (int)sectorOffsetX;
            SectorOffsetY = (int)sectorOffsetY;
            BlockDataBase = bdb;
            FloorDataBase = fdb;
            SchemesDataBase = sdb;
            Parent = parent;

            Blocks = blocksArray as Block[];
            Floors = floorsArray as Floor[];
            initialNodes = initialn as List<Vector2>;
            biom = (SectorBiom)obiom;
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
            AsyncGeneration(mapseed);
            if(SectorOffsetX == 0 && SectorOffsetY == 0) initialNodes.Add(new Vector2(10,0));
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

            biom = (SectorBiom)rand.Next(1, 5);

            switch (biom) {
                    case SectorBiom.Bushland:
                    for (int i = 0; i < rand.Next(14, 40); i++ ) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), 16);
                        break;

                    case SectorBiom.Forest:
                        for (int i = 0; i < rand.Next(12, 40); i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), 17);
                        break;
            }

            for (int i = 0; i < 1; i++) {
                MapGenerators.PlaceRandomSchemeByType(this, SchemesType.house, rand.Next(0,Rx-1),rand.Next(0,Ry-1), rand);
            }

            Parent.generated++;
            ready = true;
        }

        public Block GetBlock(int x, int y)
        {
            return Blocks[x * Ry + y];
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

        public int GetId(int x, int y)
        {
            return Blocks[x * Ry + y].Id;
        }

        public int GetId(int a)
        {
            return Blocks[a].Id;
        }

        public void SetBlock(int oneDimCoord, int id)
        {
            if (BlockDataBase.Data[id].BlockPrototype == typeof(Block)) {
                Blocks[oneDimCoord] = new Block {
                    Id = id
                };
            }
            if (BlockDataBase.Data[id].BlockPrototype == typeof(StorageBlock)) {
                Blocks[oneDimCoord] = new StorageBlock {
                    StoredItems = new List<Item.Item>(),
                    Id = id
                };
            }
        }

        public void SetBlock(int posX, int posY, int id)
        {
            SetBlock(posX * Ry + posY, id);
        }

        public void SetBlock(Vector2 pos, int id)
        {
            SetBlock((int)pos.X, (int)pos.Y, id);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if (BlockDataBase.Data[Blocks[x * Rx + y].Id].SmartAction == SmartAction.ActionOpenClose) {
                SetBlock(x, y, BlockDataBase.Data[Blocks[x * Ry + y].Id].AfterDeathId);
            }
        }

        public void AddInitialNode(float p0, float p1) {
            initialNodes.Add(new Vector2(p0, p1));
        }

        public void CreateAllMapFromArray(int[] arr)
        {
            for (int i = 0; i < MapSector.Rx; i++)
            {
                for (int j = 0; j < MapSector.Ry; j++)
                {
                    SetBlock(i, j, arr[i]);
                }
            }
        }

        public int GetMtex(int i, int i1) {
            return Blocks[i*Rx + i1].Mtex;
        }
    }

    public class GameLevel
    {
        public int MapSeed = 23142455;

        private readonly Collection<Texture2D> atlas_, flatlas_;
        private Texture2D whitepixel;

        private List<MapSector> sectors_;
        public int generated;

        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();
        private readonly SpriteBatch spriteBatch_;
        private readonly SpriteFont font_;
        public BlockDataBase blockDataBase;
        public FloorDataBase floorDataBase;
        public SchemesDataBase schemesDataBase; 

        private readonly Texture2D minimap_;

        public MapSector[] GetNeibours(MapSector ms) {
            return GetNeibours(ms.SectorOffsetX, ms.SectorOffsetY);
        }

        public MapSector[] GetNeibours(int sectorOffsetX, int sectorOffsetY) {
            List<MapSector> arrr = new List<MapSector>();

            MapSector a = GetRightN(sectorOffsetX, sectorOffsetY), b = GetLeftN(sectorOffsetX, sectorOffsetY), c = GetUpN(sectorOffsetX, sectorOffsetY), d = GetDownN(sectorOffsetX, sectorOffsetY);

            if (a != null) arrr.Add(a);
            if (b != null) arrr.Add(b);
            if (c != null) arrr.Add(c);
            if (d != null) arrr.Add(d);

            return arrr.ToArray();
        }

         public MapSector GetDownN(int sectorOffsetX, int sectorOffsetY) {
             foreach (var sector in sectors_) {
                 if (sector != null && sector.SectorOffsetX == sectorOffsetX && sector.SectorOffsetY == sectorOffsetY + 1) {
                     return sector;
                 }
             }

             if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY + 1)))
             {
                 var temp = LoadSector(sectorOffsetX, sectorOffsetY + 1);
                 sectors_.Add(temp);
                 return temp;
             }

             return null;
         }

         public MapSector GetUpN(int sectorOffsetX, int sectorOffsetY)
         {
             foreach (var sector in sectors_)
             {
                 if (sector != null && sector.SectorOffsetX == sectorOffsetX && sector.SectorOffsetY == sectorOffsetY - 1)
                 {
                     return sector;
                 }
             }

             if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY - 1)))
             {
                 var temp = LoadSector(sectorOffsetX, sectorOffsetY - 1);
                 sectors_.Add(temp);
                 return temp;
             }

             return null;
         }

         public MapSector GetLeftN(int sectorOffsetX, int sectorOffsetY)
         {
             foreach (var sector in sectors_)
             {
                 if (sector != null && sector.SectorOffsetX == sectorOffsetX - 1 && sector.SectorOffsetY == sectorOffsetY)
                 {
                     return sector;
                 }
             }

             if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX - 1, sectorOffsetY)))
             {
                 var temp = LoadSector(sectorOffsetX - 1, sectorOffsetY);
                 sectors_.Add(temp);
                 return temp;
             }

             return null;
         }

         public MapSector GetRightN(int sectorOffsetX, int sectorOffsetY)
         {
             foreach (var sector in sectors_)
             {
                 if (sector != null && sector.SectorOffsetX == sectorOffsetX + 1 && sector.SectorOffsetY == sectorOffsetY)
                 {
                     return sector;
                 }
             }

             if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX + 1, sectorOffsetY)))
             {
                 return LoadSector(sectorOffsetX + 1, sectorOffsetY);
             }

             return null;
         }

        public MapSector GetSector(int sectorOffsetX, int sectorOffsetY) {
            for (int i = 0; i < sectors_.Count; i++) {
                var sector = sectors_[i];
                if (sector != null && sector.SectorOffsetX == sectorOffsetX && sector.SectorOffsetY == sectorOffsetY)
                {
                    return sector;
                }
            }

            MapSector last = LoadSector(sectorOffsetX, sectorOffsetY);
            if(last != null) {
                sectors_.Add(last);
                return last;}

            var temp = new MapSector(blockDataBase, floorDataBase, schemesDataBase, this, sectorOffsetX, sectorOffsetY);
            sectors_.Add(temp);
            temp.Rebuild(MapSeed);
            if (temp.ready) return temp;
            return null;
        }

        private MapSector LoadSector(int sectorOffsetX, int sectorOffsetY) {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY))) {
                BinaryFormatter binaryFormatter_ = new BinaryFormatter();
                FileStream fileStream_;
                GZipStream gZipStream_;

                fileStream_ =
                    new FileStream(
                        Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY),
                        FileMode.Open);
                gZipStream_ = new GZipStream(fileStream_, CompressionMode.Decompress);
                var q1 = binaryFormatter_.Deserialize(gZipStream_);
                var q2 = binaryFormatter_.Deserialize(gZipStream_);
                var q3 = binaryFormatter_.Deserialize(gZipStream_);
                var q4 = binaryFormatter_.Deserialize(gZipStream_);
                var q5 = binaryFormatter_.Deserialize(gZipStream_);
                var q6 = binaryFormatter_.Deserialize(gZipStream_);
                gZipStream_.Close();
                fileStream_.Close();
                return new MapSector(blockDataBase, floorDataBase, schemesDataBase, this, q1, q2, q3, q4, q5, q6);
            }
            return null;
        }

        private void SaveSector(MapSector a)
        {
            BinaryFormatter binaryFormatter_ = new BinaryFormatter();
            FileStream fileStream_;
            GZipStream gZipStream_;

            fileStream_ =
                new FileStream(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", a.SectorOffsetX, a.SectorOffsetY),
                               FileMode.Create);
            gZipStream_ = new GZipStream(fileStream_, CompressionMode.Compress);
            binaryFormatter_.Serialize(gZipStream_, a.SectorOffsetX);
            binaryFormatter_.Serialize(gZipStream_, a.SectorOffsetY);
            binaryFormatter_.Serialize(gZipStream_, a.Blocks);
            binaryFormatter_.Serialize(gZipStream_, a.Floors);
            binaryFormatter_.Serialize(gZipStream_, a.initialNodes);
            binaryFormatter_.Serialize(gZipStream_, a.biom);
            gZipStream_.Close();
            fileStream_.Close();
        }

        public void ExploreAllMap()
        {
            foreach (var mapSector in sectors_) {
                mapSector.ExploreAllSector();
            }
        }

        public List<StorageBlock> GetStorageBlocks() {
            var a =  sectors_.Select(x => x.GetStorageBlocks());
            var b = new List<StorageBlock>();
            foreach (var some in a) {
                b.AddRange(some);
            }
            return b;
        }

        //------------------

        public Block GetBlock(int x, int y)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            if (sect != null)
                return sect.GetBlock(x - divx * MapSector.Rx, y - divy * MapSector.Ry);//blocks_[x * ry + y];

            return null;
        }

        public bool IsExplored(int x, int y) {
            int divx = x < 0 ? (x + 1)/MapSector.Rx - 1 : x/MapSector.Rx;
            int divy = y < 0 ? (y + 1)/MapSector.Ry - 1 : y/MapSector.Ry;

            var sect = GetSector(divx, divy);
            return sect.Blocks[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry].Explored;
        }

        public bool IsWalkable(int x, int y) {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            return blockDataBase.Data[sect.Blocks[(x-divx*MapSector.Rx) * MapSector.Ry + y-divy*MapSector.Ry].Id].IsWalkable;
        }

        public void SetFloor(int x, int y, int id)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);

            if (sect != null) {
                sect.Floors[(x - divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry].Id = id;
                sect.Floors[(x - divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry].Mtex =
                    floorDataBase.Data[id].RandomMtexFromAlters();
            }
        }

        public int GetId(int x, int y) {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            return sect.Blocks[(x - divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry].Id;
        }

        public void SetBlock(int x, int y, int id)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            if (sect != null)
            {
                if (blockDataBase.Data[id].BlockPrototype == typeof(Block))
                {
                    sect.Blocks[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry] = new Block
                    {
                        Id = id
                    };
                }
                if (blockDataBase.Data[id].BlockPrototype == typeof(StorageBlock))
                {
                    sect.Blocks[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry] = new StorageBlock
                    {
                        StoredItems=new List<Item.Item>(),Id=id
                    };
                }
                sect.Blocks[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry].Mtex =
                    blockDataBase.Data[id].RandomMtexFromAlters();
            }
        }

        public Vector2 GetInSectorPosition(Vector2 Pos) {
            return new Vector2(Pos.X < 0 ? (Pos.X + 1) / MapSector.Rx - 1 : Pos.X / MapSector.Rx, Pos.Y < 0 ? (Pos.Y + 1) / MapSector.Ry - 1 : Pos.Y / MapSector.Ry);
        }

        public void OpenCloseDoor(int x, int y) {
            var a = GetBlock(x, y);
            if(blockDataBase.Data[a.Id].SmartAction == SmartAction.ActionOpenClose) {
                if (blockDataBase.Data[a.Id].IsWalkable) {
                    EventLog.Add("Вы закрыли дверь", GlobalWorldLogic.CurrentTime, Color.Gray);
                }
                else {
                    EventLog.Add("Вы открыли дверь", GlobalWorldLogic.CurrentTime, Color.LightGray);
                }
                SetBlock(x, y, blockDataBase.Data[GetBlock(x,y).Id].AfterDeathId);               
            }
        }

        public void KillFarSectors(Creature cara) {
            for (int i = 0; i < sectors_.Count; i++) {
                var a = sectors_[i];
                if (Math.Abs(a.SectorOffsetX*MapSector.Rx - cara.Position.X/32) > 128 || Math.Abs(a.SectorOffsetY*MapSector.Ry - cara.Position.Y/32) > 128) {
                    SaveSector(a);
                    sectors_.Remove(a);
                }
            }
        }

        public void Rebuild() {
            for (int i = 0; i < sectors_.Count; i++) {
                sectors_[i] .Rebuild(MapSeed);
            }
        }

        public GameLevel(SpriteBatch spriteBatch, Collection<Texture2D> flatlas, Collection<Texture2D> atlas, SpriteFont sf, BlockDataBase bdb, FloorDataBase fdb, SchemesDataBase sdb) {
            MapGenerators.seed = MapSeed;
            whitepixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            var data = new uint[1];
            data[0] = 0xffffffff;
            whitepixel.SetData(data);

            blockDataBase = bdb;
            floorDataBase = fdb;
            schemesDataBase = sdb;

            if (spriteBatch != null) {
                minimap_ = new Texture2D(spriteBatch.GraphicsDevice, 128, 128);
            }

            sectors_ = new List<MapSector> { new MapSector(bdb, fdb, sdb, this, 0, 0) };

            sectors_[0].Rebuild(MapSeed);

            atlas_ = atlas;
            flatlas_ = flatlas;
            spriteBatch_ = spriteBatch;
            font_ = sf;
         
        }

        public GameLevel()
        {
            flatlas_ = new Collection<Texture2D>();
            atlas_ = new Collection<Texture2D>();
            blockDataBase = new BlockDataBase(new Dictionary<int, BlockData>());
            floorDataBase = new FloorDataBase(new Dictionary<int, FloorData>());
            schemesDataBase = new SchemesDataBase(new List<Schemes>());

            sectors_ = new List<MapSector> {new MapSector(0, 0)};
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms) {

        }

        public void GenerateMinimap(GraphicsDevice gd, Creature pl)
        {
            var data = new Color[128 * 128];
            for (int i = 0; i < data.Length; i++) {
                data[i] = Color.Black;
            }

            var pos = GetInSectorPosition(pl.GetPositionInBlocks());

            foreach (var sector in sectors_)
            {
                if (sector.SectorOffsetX > pos.X - 5 && sector.SectorOffsetX < pos.X + 5 && sector.SectorOffsetY > pos.Y - 5 && sector.SectorOffsetY < pos.Y + 5)
                {
                    int x = sector.SectorOffsetX - (int)pos.X + 5;
                    int y = sector.SectorOffsetY - (int)pos.Y + 5;

                    for (int i = x * 10; i < x * 10 + 11; i++)
                    {
                        for (int j = y * 10; j < y * 10 + 11; j++)
                        {
                            data[i + j * 128] = Color.White;
                        }
                    }
                }
            }

            for (int i = 6* 10; i < 6 * 10 + 11; i++) {
                for (int j = 6*10; j < 6*10 + 11; j++) {
                    data[i + j * 128] = Color.Red;
                }
            }

            minimap_.SetData(data);
        }

        public void CalcWision(Creature who, float dirAngle, float seeAngleDeg) {
            var a = who.GetPositionInBlocks();
            for (int i = -20; i < 20; i++)
            {
                for (int j = -20; j < 20; j++)
                {
            //        var tt = 1 -
            //                 (Vector2.Distance(who.Position, new Vector2((a.X + i + 0.5f) * 32, (a.Y + j + 0.5f) * 32))) /
            //                 500.0f;
            //        tt *= 255;

            //        if (tt > 255) tt = 255;
            //        if (tt < 0) tt = 0;
            //        var bb = GetBlock((int)a.X + i, (int)a.Y + j);
            //        if (bb != null)
            //        {
            //            bb.SetLight(new Color(tt, tt, tt));
            //            bb.Explored = true;
            //        }
                    GetBlock((int)a.X + i, (int)a.Y + j).SetLight(Color.Black);
               }
            }

            //for (int i = -20; i < 20; i++)
            //{
            //    Vector2 start = who.Position;
            //    Vector2 end1 = new Vector2((a.X + i + 0.5f) * 32, (-20 + 0.5f) * 32);
            //    Vector2 end2 = new Vector2((a.X + i + 0.5f) * 32, (20 + 0.5f) * 32);
            //    Vector2 end3 = new Vector2((-20 + 0.5f) * 32, (a.Y + i + 0.5f) * 32);
            //    Vector2 end4 = new Vector2((20 + 0.5f) * 32, (a.Y + i + 0.5f) * 32);

            //    GetValue(start, end1);
            //    GetValue(start, end2);
            //    GetValue(start, end3);
            //    GetValue(start, end4);
            //}

            for (int i = -20; i < 20; i++) {
                for (int j = -20; j < 20; j++) {
                    if(PathClear(a, new Vector2(a.X + i, a.Y + j))) {
                        var temp2 = GetBlock((int)a.X + i, (int)a.Y + j);
                        temp2.Lightness = Color.White;
                        temp2.Explored = true;
                    }
                }
            }

            var temp = GetBlock((int)a.X, (int)a.Y);
            temp.Lightness = Color.White;
            temp.Explored = true;
        }

        bool PathClear(Vector2 start, Vector2 end)
        {
            if (Vector2.Distance(start, end) > 1) {
                float xDelta = (end.X - start.X);
                float yDelta = (end.Y - start.Y);
                float unitX;
                float uintY;

                Vector2 checkPoint = start;

                if (Math.Abs(xDelta) > Math.Abs(yDelta)) {
                    if (end.X == 30 && end.Y == 37)
                        end.X = 30;
                    unitX = xDelta/Math.Abs(xDelta);
                    uintY = yDelta/Math.Abs(xDelta);
                    for (int x = 1; x <= Math.Abs(xDelta); x++) {
                        checkPoint.X = start.X + (int) Math.Round(x*unitX, 2);
                        checkPoint.Y = start.Y + (int) Math.Round(x*uintY, 2);
                        if (!blockDataBase.Data[GetBlock((int) checkPoint.X, (int) checkPoint.Y).Id].IsTransparent) {
                            GetBlock((int) checkPoint.X, (int) checkPoint.Y).Lightness = Color.White;
                            return false;
                        }

                        //var temp = GetBlock((int) checkPoint.X, (int) checkPoint.Y);
                        //temp.Lightness = Color.White;
                        //temp.Explored = true;
                    }
                }
                else {
                    unitX = xDelta/Math.Abs(yDelta);
                    uintY = yDelta/Math.Abs(yDelta);

                    for (int x = 1; x <= Math.Abs(yDelta); x++) {
                        checkPoint.X = start.X + (int) Math.Round(x*unitX, 2);
                        checkPoint.Y = start.Y + (int) Math.Round(x*uintY, 2);

                        if (!blockDataBase.Data[GetBlock((int) checkPoint.X, (int) checkPoint.Y).Id].IsTransparent) {
                            GetBlock((int) checkPoint.X, (int) checkPoint.Y).Lightness = Color.White;
                            return false;
                        }

                        //var temp = GetBlock((int) checkPoint.X, (int) checkPoint.Y);
                        //temp.Lightness = Color.White;
                        //temp.Explored = true;
                    }
                }
            }
            return true;
        }

        private Vector2 min, max;
        public void Draw(GameTime gameTime, Vector2 camera) {
            GetBlock((int)(camera.X / 32), (int)(camera.Y / 32));
            GetBlock((int)((camera.X + Settings.Resolution.X) / 32), (int)((camera.Y + Settings.Resolution.Y) / 32));
            GetBlock((int)((camera.X + Settings.Resolution.X) / 32), (int)((camera.Y) / 32));
            GetBlock((int)((camera.X) / 32), (int)((camera.Y + Settings.Resolution.Y) / 32));

            min = new Vector2((camera.X) / Settings.FloorSpriteSize.X - 1, (camera.Y) / Settings.FloorSpriteSize.Y - 1);
            max = new Vector2((camera.X + Settings.Resolution.X) / Settings.FloorSpriteSize.X,
                                  (camera.Y + Settings.Resolution.Y) / Settings.FloorSpriteSize.Y);

            foreach (MapSector sector in sectors_) {
                if (sector.SectorOffsetX * MapSector.Rx + MapSector.Rx < min.X && sector.SectorOffsetY * MapSector.Ry + MapSector.Ry < min.Y) continue;
                if (sector.SectorOffsetX * MapSector.Rx > max.X && sector.SectorOffsetY * MapSector.Ry > max.Y) continue;
                for (int i = 0; i < MapSector.Rx; i++) {
                    for (int j = 0; j < MapSector.Ry; j++)
                        if (sector.SectorOffsetX * MapSector.Rx + i > min.X && sector.SectorOffsetY * MapSector.Ry + j > min.Y && sector.SectorOffsetX * MapSector.Rx + i < max.X && sector.SectorOffsetY * MapSector.Ry + j < max.Y)    
                    {
                        int a = i*MapSector.Ry + j;
                        spriteBatch_.Draw(flatlas_[sector.Floors[a].Mtex],
                                          new Vector2(
                                              i*Settings.FloorSpriteSize.X - (int) camera.X +
                                              MapSector.Rx*Settings.FloorSpriteSize.X*sector.SectorOffsetX,
                                              j * Settings.FloorSpriteSize.Y - (int)camera.Y +
                                              MapSector.Ry * Settings.FloorSpriteSize.Y * sector.SectorOffsetY),
                                          null, sector.Blocks[a].Lightness);
                    }
                }
            }
        }

        public void Draw2(GameTime gameTime, Vector2 camera)
        {
            foreach (MapSector sector in sectors_) {
                if (sector.SectorOffsetX * MapSector.Rx + MapSector.Rx < min.X && sector.SectorOffsetY * MapSector.Ry + MapSector.Ry < min.Y) continue;
                if (sector.SectorOffsetX * MapSector.Rx > max.X && sector.SectorOffsetY * MapSector.Ry > max.Y) continue;
                for (int i = 0; i < MapSector.Rx; i++) {
                    for (int j = 0; j < MapSector.Ry; j++)
                        if (sector.SectorOffsetX*MapSector.Rx + i > min.X &&
                            sector.SectorOffsetY*MapSector.Ry + j > min.Y &&
                            sector.SectorOffsetX*MapSector.Rx + i < max.X &&
                            sector.SectorOffsetY*MapSector.Ry + j < max.Y) {
                            int a = i*MapSector.Ry + j;
                            spriteBatch_.Draw(atlas_[sector.Blocks[a].Mtex],
                                              new Vector2(
                                                  i*(Settings.FloorSpriteSize.X) - (int) camera.X +
                                                  MapSector.Rx*Settings.FloorSpriteSize.X*sector.SectorOffsetX,
                                                  j*(Settings.FloorSpriteSize.Y) - (int) camera.Y +
                                                  MapSector.Ry*Settings.FloorSpriteSize.Y*sector.SectorOffsetY),
                                              null, sector.Blocks[a].Lightness);
                        }
                }

                if (Settings.DebugInfo) {
                    Vector2 ff = new Vector2(-(int) camera.X +
                                         MapSector.Rx*Settings.FloorSpriteSize.X*
                                         sector.SectorOffsetX, - (int) camera.Y +
                                                               MapSector.Ry*Settings.FloorSpriteSize.Y*
                                                               sector.SectorOffsetY);

                    spriteBatch_.Draw(whitepixel, ff, null, Color.White, 0, Vector2.Zero, new Vector2(1,1024), SpriteEffects.None, 0);
                    spriteBatch_.Draw(whitepixel, ff, null, Color.White, 0, Vector2.Zero, new Vector2(1024,1), SpriteEffects.None, 0);
                    spriteBatch_.DrawString(font_, sector.biom.ToString(), new Vector2(20,20)+ff, Color.White);
                }
            }
        
        }

        public bool IsCreatureMeele(int nx, int ny, Player player) {
            return (Settings.GetMeeleActionRange() >=
                    Vector2.Distance(new Vector2((nx + 0.5f)*32, (ny + 0.5f)*32), new Vector2(player.Position.X, player.Position.Y)));
        }

        public Texture2D GetMinimap() {
            return minimap_;
        }

        public int SectorCount() {
            return sectors_.Count;
        }

        public void SaveAll() {
            foreach (var sector in sectors_) {
                SaveSector(sector);
            }
        }
    }
}