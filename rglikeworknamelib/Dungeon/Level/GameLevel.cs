using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public BlockDataBase BlockDataBase;
        public FloorDataBase FloorDataBase;
        public SchemesDataBase SchemesDataBase;

        public int SectorOffsetX, SectorOffsetY;

        internal Block[] Blocks;
        internal Floor[] Floors;

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
        }

        public MapSector(BlockDataBase bdb, FloorDataBase fdb, SchemesDataBase sdb, int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;
            BlockDataBase = bdb;
            FloorDataBase = fdb;
            SchemesDataBase = sdb;

            Blocks = new Block[Rx * Ry];
            Floors = new Floor[Rx * Ry];

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }
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
        public MapSector(BlockDataBase bdb, FloorDataBase fdb, SchemesDataBase sdb, object sectorOffsetX, object sectorOffsetY, object blocksArray, object floorsArray) {
            SectorOffsetX = (int)sectorOffsetX;
            SectorOffsetY = (int)sectorOffsetY;
            BlockDataBase = bdb;
            FloorDataBase = fdb;
            SchemesDataBase = sdb;

            Blocks = blocksArray as Block[];
            Floors = floorsArray as Floor[];
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
        public void Rebuild(int mapseed)
        {
            Random rand = new Random(SectorOffsetX|SectorOffsetY|mapseed);
            MapGenerators.FillTest1(this, 1);
            MapGenerators.ClearBlocks(this);
            MapGenerators.FloorPerlin(this);
            MapGenerators.GenerateStreetsNew(this, rand.Next(1, 3), rand.Next(5, 10), rand.Next(2, 3), 2, 3, rand);
            for (int i = 0; i < 3; i++)
                MapGenerators.PlaceRandomSchemeByType(this, SchemesType.house, rand.Next(0, Rx), rand.Next(0, Ry), rand);
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
        
    }

    public class GameLevel
    {
        public int Rx = 100;
        public int Ry = 100;
        private readonly Collection<Texture2D> atlas_, flatlas_;
        private Texture2D whitepixel;

        private List<MapSector> sectors_;

        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();
        private readonly SpriteBatch spriteBatch_;
        public BlockDataBase blockDataBase;
        public FloorDataBase floorDataBase;
        public SchemesDataBase schemesDataBase;

        private readonly BinaryFormatter binaryFormatter_ = new BinaryFormatter();
        private FileStream fileStream_;
        private GZipStream gZipStream_;

        public int MapSeed = 12345;

        private readonly Texture2D minimap_;

        public MapSector GetSector(int sectorOffsetX, int sectorOffsetY) {

            foreach (var sector in sectors_) {
                if (sector.SectorOffsetX == sectorOffsetX && sector.SectorOffsetY == sectorOffsetY) return sector;
            }

            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY)))
            {
                fileStream_ = new FileStream(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY), FileMode.Open);
                gZipStream_ = new GZipStream(fileStream_, CompressionMode.Decompress);
                var q1 = binaryFormatter_.Deserialize(gZipStream_);
                var q2 = binaryFormatter_.Deserialize(gZipStream_);
                var q3 = binaryFormatter_.Deserialize(gZipStream_);
                var q4 = binaryFormatter_.Deserialize(gZipStream_);
                gZipStream_.Close();
                fileStream_.Close();
                sectors_.Add(new MapSector(blockDataBase, floorDataBase, schemesDataBase, q1, q2, q3, q4));
                return sectors_.Last();
            }
            sectors_.Add(new MapSector(blockDataBase, floorDataBase, schemesDataBase, sectorOffsetX, sectorOffsetY));
            sectors_.Last().Rebuild(MapSeed);
            return sectors_.Last();
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

        public void CreateAllMapFromArray(int[] arr)
        {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    SetBlock(i, j, arr[i]);
                }
            }
        }

        public Block GetBlock(int x, int y)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            return sect.GetBlock(x-divx*MapSector.Rx, y-divy*MapSector.Ry);//blocks_[x * ry + y];
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
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);

            sect.Floors[(x - divx *MapSector.Rx) * MapSector.Ry + y - divy*MapSector.Ry].Id = id;
            sect.Floors[(x - divx*MapSector.Rx) * MapSector.Ry + y - divy*MapSector.Ry].Mtex = floorDataBase.Data[id].RandomMtexFromAlters();
        }

        public int GetId(int x, int y) {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            return sect.Blocks[(x - divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry].Id;
        }

        public void SetBlock(int x, int y, int id)
        {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);

            if (blockDataBase.Data[id].BlockPrototype == typeof(Block)) {
                sect.Blocks[(x-divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry] = new Block {
                    Id = id
                };
            }
            if (blockDataBase.Data[id].BlockPrototype == typeof(StorageBlock)) {
                sect.Blocks[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry] = new StorageBlock {
                    StoredItems = new List<Item.Item>(),
                    Id = id
                };
            }
        }

        public void OpenCloseDoor(int x, int y)
        {
            if(blockDataBase.Data[GetBlock(x,y).Id].SmartAction == SmartAction.ActionOpenClose) {
                SetBlock(x, y, blockDataBase.Data[GetBlock(x,y).Id].AfterDeathId);
            }
        }

        public void KillFarSectors(Creature cara) {
            for (int i = 0; i < sectors_.Count; i++) {
                var a = sectors_[i];
                if (Math.Abs(a.SectorOffsetX*MapSector.Rx - cara.Position.X/32) > 64 || Math.Abs(a.SectorOffsetY*MapSector.Ry - cara.Position.Y/32) > 64) {
                    fileStream_ = new FileStream(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", a.SectorOffsetX, a.SectorOffsetY), FileMode.Create);
                    gZipStream_ = new GZipStream(fileStream_, CompressionMode.Compress);
                    binaryFormatter_.Serialize(gZipStream_, a.SectorOffsetX);
                    binaryFormatter_.Serialize(gZipStream_, a.SectorOffsetY);
                    binaryFormatter_.Serialize(gZipStream_, a.Blocks);
                    binaryFormatter_.Serialize(gZipStream_, a.Floors);
                    gZipStream_.Close();
                    fileStream_.Close();
                    sectors_.Remove(a);
                }
            }
        }

        public void Rebuild()
        {
            foreach (var sector in sectors_) {
                sector.Rebuild(MapSeed);
            }
        }

        public GameLevel(SpriteBatch spriteBatch, Collection<Texture2D> flatlas, Collection<Texture2D> atlas, BlockDataBase bdb, FloorDataBase fdb, SchemesDataBase sdb)
        {

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

            sectors_ = new List<MapSector> {new MapSector(bdb, fdb, sdb, 0, 0)};

            sectors_[0].Rebuild(MapSeed);

            atlas_ = atlas;
            flatlas_ = flatlas;
            spriteBatch_ = spriteBatch;
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

        public bool IsInMapBounds(Vector2 t)
        {
            return IsInMapBounds((int)t.X, (int)t.Y);
        }
        public bool IsInMapBounds(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < Rx && y < Ry);
        }

        public void GenerateMinimap(GraphicsDevice gd, Creature pl)
        {
            //var data = new Color[rx * ry];

            //int startx = Math.Max(0, (int)pl.Position.X / 32 - 64);
            //int starty = Math.Max(0, (int)pl.Position.Y / 32 - 64);
            //int endx = Math.Min(rx, (int)pl.Position.X / 32 + 64);
            //int endy = Math.Min(ry, (int)pl.Position.Y / 32 + 64);

            //for (int i = startx; i < endx; i++) {
            //    for (int j = starty; j < endy; j++) {
            //        if (blocks_[i * ry + j].explored) {
            //            var a = blocks_[i*ry + j].id;
            //            if (a == 0) {
            //                data[(j - starty)*128 + (i - startx)] = floorDataBase.Data[floors_[i*ry + j].ID].MMCol;
            //            } else {
            //                data[(j - starty) * 128 + (i - startx)] = blockDataBase.Data[a].MMCol;
            //            }
            //        } else {
            //            data[(j - starty) * 128 + (i - startx)] = Color.Black;
            //        }
            //    }
            //}
            //minimap_.SetData(data);
        }

        public short[] CalcWision(Creature who, float dirAngle, float seeAngleDeg)
        {
            return new short[1];
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
                                          null, Color.White); //sector.blocks_[a].lightness);
                    }
                }
            }
        }

        public void Draw2(GameTime gameTime, Vector2 camera)
        {
            foreach (MapSector sector in sectors_) {
                for (int i = 0; i < MapSector.Rx; i++) {
                    for (int j = 0; j < MapSector.Ry; j++)
                        if (sector.SectorOffsetX*MapSector.Rx + i > min.X &&
                            sector.SectorOffsetY*MapSector.Ry + j > min.Y &&
                            sector.SectorOffsetX*MapSector.Rx + i < max.X &&
                            sector.SectorOffsetY*MapSector.Ry + j < max.Y) {
                            int a = i*MapSector.Ry + j;
                            spriteBatch_.Draw(atlas_[blockDataBase.Data[sector.Blocks[a].Id].MTex],
                                              new Vector2(
                                                  i*(Settings.FloorSpriteSize.X) - (int) camera.X +
                                                  MapSector.Rx*Settings.FloorSpriteSize.X*sector.SectorOffsetX,
                                                  j*(Settings.FloorSpriteSize.Y) - (int) camera.Y +
                                                  MapSector.Ry*Settings.FloorSpriteSize.Y*sector.SectorOffsetY),
                                              null, Color.White); //sector.blocks_[a].lightness);
                        }
                }

                if (Settings.DebugInfo) {
                    spriteBatch_.Draw(whitepixel, new Vector2(-(int) camera.X +
                                                              MapSector.Rx*Settings.FloorSpriteSize.X*
                                                              sector.SectorOffsetX, - (int) camera.Y +
                                                  MapSector.Ry*Settings.FloorSpriteSize.Y*sector.SectorOffsetY), null, Color.White, 0, Vector2.Zero, new Vector2(1,1024), SpriteEffects.None, 0);
                    spriteBatch_.Draw(whitepixel, new Vector2(-(int) camera.X +
                                                              MapSector.Rx*Settings.FloorSpriteSize.X*
                                                              sector.SectorOffsetX, - (int) camera.Y +
                                                  MapSector.Ry*Settings.FloorSpriteSize.Y*sector.SectorOffsetY), null, Color.White, 0, Vector2.Zero, new Vector2(1024,1), SpriteEffects.None, 0);
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
    }
}