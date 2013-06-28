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
    public class GameLevel
    {
        public int rx = 100;
        public int ry = 100;
        private readonly Collection<Texture2D> atlas_, flatlas_;
        
        private readonly Block[] blocks_;
        private readonly Floor[] floors_;

        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();
        private readonly SpriteBatch spriteBatch_;
        public BlockDataBase BlockDataBase;
        public FloorDataBase FloorDataBase;
        public SchemesDataBase SchemesBase;
        private readonly Random rnd_ = new Random();

        private readonly Texture2D minimap_;

        public List<StorageBlock> GetStorageBlocks()
        {
            return (from a in blocks_ where BlockDataBase.Data[a.id].BlockPrototype == typeof (StorageBlock) select a as StorageBlock).ToList();
        }

        public void ExploreAllMap()
        {
            foreach (var b in blocks_) {
                b.explored = true;
                b.lightness = Color.White;
            }
        }

        public void CreateAllMapFromArray(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++) {
                SetBlock(i, arr[i]);
            }
        }

        public Block GetBlock(int x, int y)
        {
            return blocks_[x * ry + y];
        }

        public bool IsExplored(int x, int y)
        {
            return blocks_[x * ry + y].explored;
        }

        public bool IsExplored(int a)
        {
            return blocks_[a].explored;
        }

        public bool IsWalkable(int x, int y)
        {
            return BlockDataBase.Data[blocks_[x * ry + y].id].IsWalkable;
        }

        public void SetFloor(int x, int y, int id)
        {
            floors_[x * ry + y].ID = id;
            floors_[x * ry + y].Mtex = FloorDataBase.Data[id].RandomMtexFromAlters();
        }

        public int GetId(int x, int y)
        {
            return blocks_[x * ry + y].id;
        }

        public int GetId(int a)
        {
            return blocks_[a].id;
        }

        public void SetBlock(int a, int id)
        {
            if (BlockDataBase.Data[id].BlockPrototype == typeof(Block)) {
                blocks_[a] = new Block {
                    id = id
                };
            }
            if (BlockDataBase.Data[id].BlockPrototype == typeof(StorageBlock)) {
                blocks_[a] = new StorageBlock {
                    storedItems = new List<Item.Item>(),
                    id = id
                };
            }
        }

        public void SetBlock(int x, int y, int id) {
            SetBlock(x * ry + y, id);
        }

        public void SetBlock(Vector2 where, int id) {
            SetBlock((int)where.X, (int)where.Y, id);
        }

        public void OpenCloseDoor(Vector2 where) {
            OpenCloseDoor((int) where.X, (int) where.Y);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if(BlockDataBase.Data[blocks_[x*ry+y].id].SmartAction == SmartAction.ActionOpenClose) {
                SetBlock(x, y, BlockDataBase.Data[blocks_[x * ry + y].id].AfterDeathId);
            }
        }

        public static GameLevel CreateGameLevel(SpriteBatch spriteBatch, Collection<Texture2D> flatlas, Collection<Texture2D> atlas, BlockDataBase bdb_, FloorDataBase fdb_, SchemesDataBase sdb_, int rx_ = 100, int ry_ = 100)
        {
            return new GameLevel(spriteBatch, flatlas, atlas, bdb_, fdb_, sdb_, rx_, ry_);
        }

        private GameLevel(SpriteBatch spriteBatch, Collection<Texture2D> flatlas, Collection<Texture2D> atlas, BlockDataBase bdb_, FloorDataBase fdb_, SchemesDataBase sdb_, int rx_ = 100, int ry_ = 100) {
            rx = rx_;
            ry = ry_;
            blocks_ = new Block[rx * ry];
            floors_ = new Floor[rx * ry];

            BlockDataBase = bdb_;
            FloorDataBase = fdb_;
            SchemesBase = sdb_;

            if (spriteBatch != null) {
                minimap_ = new Texture2D(spriteBatch.GraphicsDevice, 128, 128);
            }

            int i = rx * ry;
            while (i-- != 0) {
                floors_[i] = new Floor();
                blocks_[i] = new Block();
            }

            Rebuild();

          //  MapGenerators.AddTestScheme(this, schemesBase, rx, ry);

            //for (int i = 0; i < DungDimX; i++) {

            //    _block[i * DungDimY + 0].ID = 1;
            //    _block[i * DungDimY + DungDimY-1].ID = 1;
            //}
            //for (int j = 0; j < DungDimX; j++) {
            //    _block[0 + j].ID = 1;
            //    _block[(DungDimX - 1) * DungDimY + j].ID = 1;
            //}
            atlas_ = atlas;
            flatlas_ = flatlas;
            spriteBatch_ = spriteBatch;
        }

        public void Rebuild()
        {
            MapGenerators.FillTest1(this, 1);
            MapGenerators.ClearBlocks(this);
            MapGenerators.GenerateStreetsNew(this, rnd_.Next(80,200), rnd_.Next(80,200), rnd_.Next(20,30), 2, 3);
            for (int i = 0; i < 400;i++ )
                MapGenerators.PlaceRandomSchemeByType(this, SchemesType.house, rnd_.Next(0, rx), rnd_.Next(0, ry));
        }


        public GameLevel(int rx_ = 100, int ry_ = 100)
        {
            //spriteBatch_ = new SpriteBatch(null);
            flatlas_ = new Collection<Texture2D>();
            atlas_ = new Collection<Texture2D>();
            BlockDataBase = new BlockDataBase(new Dictionary<int, BlockData>());
            FloorDataBase = new FloorDataBase(new Dictionary<int, FloorData>());
            SchemesBase = new SchemesDataBase(new List<Schemes>());
            rx = rx_;
            ry = ry_;

            blocks_ = new Block[rx_ * ry_];
            floors_ = new Floor[rx_ * ry_];

            int i = rx * ry;
            while (i-- != 0) {
                floors_[i] = new Floor();
                blocks_[i] = new Block();
            }
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
            var data = new Color[rx * ry];

            int startx = Math.Max(0, (int)pl.Position.X / 32 - 64);
            int starty = Math.Max(0, (int)pl.Position.Y / 32 - 64);
            int endx = Math.Min(rx, (int)pl.Position.X / 32 + 64);
            int endy = Math.Min(ry, (int)pl.Position.Y / 32 + 64);

            for (int i = startx; i < endx; i++) {
                for (int j = starty; j < endy; j++) {
                    if (blocks_[i * ry + j].explored) {
                        var a = blocks_[i*ry + j].id;
                        if (a == 0) {
                            data[(j - starty)*128 + (i - startx)] = FloorDataBase.Data[floors_[i*ry + j].ID].MMCol;
                        } else {
                            data[(j - starty) * 128 + (i - startx)] = BlockDataBase.Data[a].MMCol;
                        }
                    } else {
                        data[(j - starty) * 128 + (i - startx)] = Color.Black;
                    }
                }
            }
            minimap_.SetData(data);
        }

        public short[] CalcWision(Creature who, float dirAngle, float seeAngleDeg)
        {
            int dxTop = 0;
            int dyTop = 0;
            int dir;
            int dx, dy, s_x, s_y, r_x, r_y;
            int px, py;
            int a;
            int x1, x2, y1, y2;
            int los_x_null = 0;
            int los_y_null = 0;

            int pos_x = (int)who.Position.X / 32;
            int pos_y = (int)who.Position.Y / 32;

            var n = new short[rx * ry];

            if (!IsInMapBounds(pos_x, pos_y)) {
                return n;
            }

            int see = 15;


            for (int i = 0; i < rx; i++) {
                for (int j = 0; j < ry; j++) {
                    n[i * ry + j] = -1;
                }
            }

            for (int i = 0; i < blocks_.Length; i++) {
                blocks_[i].lightness = Color.Black;
            }

            try {
                for (int i = pos_x - 1; i < pos_x + 1; i++) {
                    for (int j = pos_y - 1; j < pos_y + 1; j++) {
                        blocks_[i * ry + j].lightness = Color.White;
                        blocks_[i * ry + j].explored = true;
                    }
                }
            } catch (Exception)
            {
            }


            int xTop = pos_x - see;
            int yTop = pos_y - see;


            for (dir = 0; dir < 4; dir++) {
                if (xTop < 0) {
                    xTop = 0;
                }
                if (xTop >= rx) {
                    xTop = rx - 1;
                }
                if (yTop < 0) {
                    yTop = 0;
                }
                if (yTop >= ry) {
                    yTop = ry - 1;
                }

                if (dir == 0) {
                    dxTop = +1;
                    dyTop = 0;
                }
                if (dir == 1) {
                    dxTop = 0;
                    dyTop = +1;
                }
                if (dir == 2) {
                    dxTop = -1;
                    dyTop = 0;
                }
                if (dir == 3) {
                    dxTop = 0;
                    dyTop = -1;
                }

                while (true) {

                    if (dir == 0 & ((xTop - pos_x) == see || (xTop - pos_x) >= rx - 1)) {
                        break;
                    }
                    if (dir == 1 & ((yTop - pos_y) == see || (yTop - pos_x) >= ry - 1)) {
                        break;
                    }
                    if (dir == 2 & ((pos_x - xTop) == see || (pos_x - xTop) >= rx - 1)) {
                        break;
                    }
                    if (dir == 3 & ((pos_y - yTop) == see || (pos_y - yTop) >= ry - 1)) {
                        break;
                    }

                    x1 = pos_x;
                    y1 = pos_y;
                    x2 = xTop;
                    y2 = yTop;

                    px = x2 - x1;
                    py = y2 - y1;
                    s_x = (px >= 0) ? 1 : -1;
                    s_y = (py >= 0) ? 1 : -1;
                    px = (px >= 0) ? px : -px;
                    py = (py >= 0) ? py : -py;

                    int max;
                    int min;
                    if (px >= py) {
                        max = px;
                        min = py;
                        r_x = s_x;
                        r_y = 0;
                    } else {
                        max = py;
                        min = px;
                        r_x = 0;
                        r_y = s_y;
                    }

                    int tmax = max;
                    a = max >> 1;
                    while (tmax != 0) {
                        a += min;
                        if ((a - max) < 0) {
                            dx = r_x;
                            dy = r_y;
                        } else {
                            dx = s_x;
                            dy = s_y;
                            a -= max;
                        }
                        x1 += dx;
                        y1 += dy;

                        int seen;
                        if (pos_x == xTop || pos_y == yTop) {
                            seen = see - 1;
                        } else {
                            seen = see;
                        }
                        float dist = Convert.ToInt32(Math.Sqrt(Math.Pow((pos_x - x1), 2) + Math.Pow((pos_y - y1), 2)));
                        if (dist > seen) {
                            break;
                        }

                        float curangle = (float)Math.Atan2(y1 - pos_y, x1 - pos_x);
                        float wrapedangle = MathHelper.WrapAngle(dirAngle);
                        if ((curangle - MathHelper.ToRadians(seeAngleDeg) > wrapedangle || curangle + MathHelper.ToRadians(seeAngleDeg) < wrapedangle) &&
                            (curangle - MathHelper.ToRadians(seeAngleDeg) > wrapedangle + MathHelper.TwoPi || curangle + MathHelper.ToRadians(seeAngleDeg) < wrapedangle + MathHelper.TwoPi) &&
                            (curangle - MathHelper.ToRadians(seeAngleDeg) > wrapedangle - MathHelper.TwoPi || curangle + MathHelper.ToRadians(seeAngleDeg) < wrapedangle - MathHelper.TwoPi)) {
                            break;
                        }

                        if (x1 >= 0 && x1 < rx && y1 >= 0 && y1 < ry &&
                            (n[(x1 - los_x_null) * ry + y1 - los_y_null] != 0 || n[(x1 - los_x_null) * ry + y1 - los_y_null] == -1)) {
                            if (!BlockDataBase.Data[blocks_[x1 * ry + y1].id].IsTransparent) {
                                n[(x1 - los_x_null) * ry + y1 - los_y_null] = 2;
                                blocks_[(x1 - los_x_null) * ry + y1 - los_y_null].explored = true;
                                byte temp11 = Convert.ToByte(255 - dist / seen * 205);
                                blocks_[(x1 - los_x_null) * ry + y1 - los_y_null].lightness = new Color(temp11, temp11,
                                                                                                        temp11);
                                break;
                            } else {
                                n[(x1 - los_x_null) * ry + y1 - los_y_null] = 0;
                                blocks_[(x1 - los_x_null) * ry + y1 - los_y_null].explored = true;
                                byte temp11 = Convert.ToByte(255 - dist / seen * 205);
                                blocks_[(x1 - los_x_null) * ry + y1 - los_y_null].lightness = new Color(temp11, temp11,
                                                                                                        temp11);
                            }
                        }

                        tmax--;
                    }

                    xTop += dxTop;
                    yTop += dyTop;
                }
            }
            n[pos_x * ry + pos_y] = 0;

            for (int i = 0; i < blocks_.Length; i++) {
                if (blocks_[i].explored && blocks_[i].lightness == Color.Black) {
                    blocks_[i].lightness = new Color(50, 50, 50);
                }
            }

            return n;
        }

        public void Draw(GameTime gameTime, Vector2 camera)
        {
            var min = new Vector2((camera.X) / Settings.FloorSpriteSize.X, (camera.Y) / Settings.FloorSpriteSize.Y);
            var max = new Vector2((camera.X + Settings.Resolution.X) / Settings.FloorSpriteSize.X,
                                  (camera.Y + Settings.Resolution.Y) / Settings.FloorSpriteSize.Y);

            min.X = MathHelper.Max(min.X, 0);
            min.Y = MathHelper.Max(min.Y, 0);
            max.X = MathHelper.Min(max.X + 1, rx);
            max.Y = MathHelper.Min(max.Y + 1, ry);


            for (int i = (int)min.X, ci = 0; i < (int)max.X; i++, ci++) {
                for (int j = (int)min.Y, cj = 0; j < (int)max.Y; j++, cj++) {
                    int a = i * ry + j;
                    spriteBatch_.Draw(flatlas_[floors_[a].Mtex],
                                      new Vector2(i * (Settings.FloorSpriteSize.X) - (int)camera.X,
                                                  j * (Settings.FloorSpriteSize.Y) - (int)camera.Y),
                                      null, blocks_[a].lightness);
                }
            }
        }

        public void Draw2(GameTime gameTime, Vector2 camera)
        {
            var min = new Vector2((camera.X) / Settings.FloorSpriteSize.X, (camera.Y) / Settings.FloorSpriteSize.Y);
            var max = new Vector2((camera.X + Settings.Resolution.X) / Settings.FloorSpriteSize.X,
                                  (camera.Y + Settings.Resolution.Y) / Settings.FloorSpriteSize.Y);

            min.X = MathHelper.Max(min.X, 0);
            min.Y = MathHelper.Max(min.Y, 0);
            max.X = MathHelper.Min(max.X + 1, rx);
            max.Y = MathHelper.Min(max.Y + 1, ry);


            for (int i = (int)min.X, ci = 0; i < (int)max.X; i++, ci++) {
                for (int j = (int)min.Y, cj = 0; j < (int)max.Y; j++, cj++) {
                    int a = i * ry + j;
                    spriteBatch_.Draw(atlas_[BlockDataBase.Data[blocks_[a].id].TexNo],
                                      new Vector2(i * (Settings.FloorSpriteSize.X) - (int)camera.X,
                                                  j * (Settings.FloorSpriteSize.Y) - (int)camera.Y),
                                      null, blocks_[a].lightness);
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
    }
}