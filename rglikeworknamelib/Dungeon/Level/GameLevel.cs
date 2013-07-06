using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rglikeworknamelib.Generation;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Level {

    public class MapSector {
        public const int Rx = 32;
        public const int Ry = 32;

        public BlockDataBase blockDataBase;
        public FloorDataBase floorDataBase;
        public SchemesDataBase schemesDataBase;

        Random rnd_ = new Random();

        public int GLx, GLy;

        internal Block[] blocks_;
        internal Floor[] floors_;

        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();

        public MapSector(int x, int y) {
            GLx = x;
            GLy = y;

            blocks_ = new Block[Rx * Ry];
            floors_ = new Floor[Rx * Ry];

            int i = Rx * Ry;
            while (i-- != 0) {
                floors_[i] = new Floor();
                blocks_[i] = new Block();
            }

            blockDataBase = new BlockDataBase(new Dictionary<int, BlockData>());
            floorDataBase = new FloorDataBase(new Dictionary<int, FloorData>());
            schemesDataBase = new SchemesDataBase(new List<Schemes>());
        }

        public MapSector(BlockDataBase bdb_, FloorDataBase fdb_, SchemesDataBase sdb_, int x, int y) {
            GLx = x;
            GLy = y;
            blockDataBase = bdb_;
            floorDataBase = fdb_;
            schemesDataBase = sdb_;

            blocks_ = new Block[Rx * Ry];
            floors_ = new Floor[Rx * Ry];

            int i = Rx * Ry;
            while (i-- != 0) {
                floors_[i] = new Floor();
                blocks_[i] = new Block();
            }
        }

        public List<StorageBlock> GetStorageBlocks()
        {
            return (from a in blocks_
                    where blockDataBase.Data[a.id].BlockPrototype == typeof(StorageBlock)
                    select a as StorageBlock).ToList();
        }

        public void ExploreAllSector()
        {
            foreach (var b in blocks_) {
                b.explored = true;
                b.lightness = Color.White;
            }
        }

        public void Rebuild()
        {
            MapGenerators.FillTest1(this, 1);
            MapGenerators.ClearBlocks(this);
            MapGenerators.FloorPerlin(this);
            MapGenerators.GenerateStreetsNew(this, rnd_.Next(1, 3), rnd_.Next(5, 10), rnd_.Next(2, 3), 2, 3);
            for (int i = 0; i < 3; i++)
                MapGenerators.PlaceRandomSchemeByType(this, SchemesType.house, rnd_.Next(0, MapSector.Rx), rnd_.Next(0, MapSector.Ry));
        }

        public Block GetBlock(int x, int y)
        {
            return blocks_[x * Ry + y];
        }

        public bool IsExplored(int x, int y)
        {
            return blocks_[x * Ry + y].explored;
        }

        public bool IsExplored(int a)
        {
            return blocks_[a].explored;
        }

        public bool IsWalkable(int x, int y)
        {
            return blockDataBase.Data[blocks_[x * Ry + y].id].IsWalkable;
        }

        public void SetFloor(int x, int y, int id)
        {
            floors_[x * Ry + y].ID = id;
            floors_[x * Ry + y].Mtex = floorDataBase.Data[id].RandomMtexFromAlters();
        }

        public int GetId(int x, int y)
        {
            return blocks_[x * Ry + y].id;
        }

        public int GetId(int a)
        {
            return blocks_[a].id;
        }

        public void SetBlock(int a, int id)
        {
            if (blockDataBase.Data[id].BlockPrototype == typeof(Block)) {
                blocks_[a] = new Block {
                    id = id
                };
            }
            if (blockDataBase.Data[id].BlockPrototype == typeof(StorageBlock)) {
                blocks_[a] = new StorageBlock {
                    storedItems = new List<Item.Item>(),
                    id = id
                };
            }
        }

        public void SetBlock(int x, int y, int id)
        {
            SetBlock(x * Ry + y, id);
        }

        public void SetBlock(Vector2 where, int id)
        {
            SetBlock((int)where.X, (int)where.Y, id);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if (blockDataBase.Data[blocks_[x * Rx + y].id].SmartAction == SmartAction.ActionOpenClose) {
                SetBlock(x, y, blockDataBase.Data[blocks_[x * Ry + y].id].AfterDeathId);
            }
        }
        
    }

    public class GameLevel
    {
        public int rx = 100;
        public int ry = 100;
        private readonly Collection<Texture2D> atlas_, flatlas_;

        private List<MapSector> sectors_;

        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();
        private readonly SpriteBatch spriteBatch_;
        public BlockDataBase blockDataBase;
        public FloorDataBase floorDataBase;
        public SchemesDataBase schemesDataBase;
        private readonly Random rnd_ = new Random();

        private readonly Texture2D minimap_;

        public MapSector GetSector(int x, int y) {

            foreach (var sector in sectors_) {
                if (sector.GLx == x && sector.GLy == y) return sector;
            }
            sectors_.Add(new MapSector(blockDataBase, floorDataBase, schemesDataBase, x, y));
            sectors_.Last().Rebuild();
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
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x/MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);
            return sect.GetBlock(x-divx*MapSector.Rx, y-divy*MapSector.Ry);//blocks_[x * ry + y];
        }

        public bool IsExplored(int x, int y) {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);
            return sect.blocks_[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry].explored;
        }

        public bool IsWalkable(int x, int y) {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);
            return blockDataBase.Data[sect.blocks_[(x-divx*MapSector.Rx) * MapSector.Ry + y-divy*MapSector.Ry].id].IsWalkable;
        }

        public void SetFloor(int x, int y, int id)
        {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);

            sect.floors_[(x - divx *MapSector.Rx) * MapSector.Ry + y - divy*MapSector.Ry].ID = id;
            sect.floors_[(x - divx*MapSector.Rx) * MapSector.Ry + y - divy*MapSector.Ry].Mtex = floorDataBase.Data[id].RandomMtexFromAlters();
        }

        public int GetId(int x, int y) {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            return sect.blocks_[(x - divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry].id;
        }

        public void SetBlock(int x, int y, int id)
        {
            int divx = x / MapSector.Rx, divy = y / MapSector.Ry;
            if (x < 0) divx = x / MapSector.Rx - 1;
            if (y < 0) divy = y / MapSector.Ry - 1;
            var sect = GetSector(divx, divy);

            if (blockDataBase.Data[id].BlockPrototype == typeof(Block)) {
                sect.blocks_[(x-divx*MapSector.Rx)*MapSector.Ry + y - divy*MapSector.Ry] = new Block {
                    id = id
                };
            }
            if (blockDataBase.Data[id].BlockPrototype == typeof(StorageBlock)) {
                sect.blocks_[(x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry] = new StorageBlock {
                    storedItems = new List<Item.Item>(),
                    id = id
                };
            }
        }

        public void OpenCloseDoor(int x, int y)
        {
            if(blockDataBase.Data[GetBlock(x,y).id].SmartAction == SmartAction.ActionOpenClose) {
                SetBlock(x, y, blockDataBase.Data[GetBlock(x,y).id].AfterDeathId);
            }
        }

        public void KillFarSectors(Creature cara) {
            for (int i = 0; i < sectors_.Count; i++) {
                var a = sectors_[i];
                if (Math.Abs(a.GLx*MapSector.Rx - cara.Position.X/32) > 64 || Math.Abs(a.GLy*MapSector.Ry - cara.Position.Y/32) > 64) {
                    sectors_.Remove(a);
                }
            }
        }

        public void Rebuild()
        {
            foreach (var sector in sectors_) {
                sector.Rebuild();
            }
        }

        public GameLevel(SpriteBatch spriteBatch, Collection<Texture2D> flatlas, Collection<Texture2D> atlas, BlockDataBase bdb_, FloorDataBase fdb_, SchemesDataBase sdb_)
        {

            blockDataBase = bdb_;
            floorDataBase = fdb_;
            schemesDataBase = sdb_;

            if (spriteBatch != null) {
                minimap_ = new Texture2D(spriteBatch.GraphicsDevice, 128, 128);
            }

            sectors_ = new List<MapSector> {new MapSector(bdb_, fdb_, sdb_, 0, 0)};

            sectors_[0].Rebuild();

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
            return (x >= 0 && y >= 0 && x < rx && y < ry);
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
                        if (sector.GLx * MapSector.Rx + i > min.X && sector.GLy * MapSector.Ry + j > min.Y && sector.GLx * MapSector.Rx + i < max.X && sector.GLy * MapSector.Ry + j < max.Y)    
                    {
                        int a = i*MapSector.Ry + j;
                        spriteBatch_.Draw(flatlas_[sector.floors_[a].Mtex],
                                          new Vector2(
                                              i*Settings.FloorSpriteSize.X - (int) camera.X +
                                              MapSector.Rx*Settings.FloorSpriteSize.X*sector.GLx,
                                              j * Settings.FloorSpriteSize.Y - (int)camera.Y +
                                              MapSector.Ry * Settings.FloorSpriteSize.Y * sector.GLy),
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
                        if (sector.GLx * MapSector.Rx + i > min.X && sector.GLy * MapSector.Ry + j > min.Y && sector.GLx * MapSector.Rx + i < max.X && sector.GLy * MapSector.Ry + j < max.Y)   
                    {
                        int a = i * MapSector.Ry + j;
                        spriteBatch_.Draw(atlas_[blockDataBase.Data[sector.blocks_[a].id].MTex],
                                          new Vector2(
                                              i * (Settings.FloorSpriteSize.X) - (int)camera.X +
                                              MapSector.Rx * Settings.FloorSpriteSize.X * sector.GLx,
                                              j * (Settings.FloorSpriteSize.Y) - (int)camera.Y +
                                              MapSector.Ry * Settings.FloorSpriteSize.Y * sector.GLy),
                                          null, Color.White);//sector.blocks_[a].lightness);
                    }
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