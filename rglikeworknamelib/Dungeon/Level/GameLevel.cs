using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Block[] blocks_;
        public Floor[] floors_;
        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();
        private readonly SpriteBatch spriteBatch_;
        private readonly BlockDataBase blockDataBase;
        private readonly FloorDataBase floorDataBase;
        private readonly SchemesDataBase schemesBase;
        private Random rnd = new Random();

        private Texture2D minimap;

        public List<StorageBlock> GetStorageBlocks() {
            List<StorageBlock> temp = new List<StorageBlock>();
            foreach(var a in blocks_) {
                if (blockDataBase.Data[a.id].blockPrototype == typeof(StorageBlock))
                    temp.Add(a as StorageBlock);
            }
            return temp;
        }

        public void CreateBlock(int x, int y, int id) {
            if (blockDataBase.Data[id].blockPrototype == typeof (Block)) {
                blocks_[x*ry+y] = new Block{ id = id };
            }
            if (blockDataBase.Data[id].blockPrototype == typeof(StorageBlock)) {
                blocks_[x * ry + y] = new StorageBlock {
                                                           storedItems = new List<Item.Item>(),
                                                           id = id
                                                       };
            }
        }

        public void CreateBlock(Vector2 where, int id) {
            CreateBlock((int)where.X, (int)where.Y, id);
        }

        public void OpenCloseDoor(Vector2 where) {
            OpenCloseDoor((int) where.X, (int) where.Y);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if(blockDataBase.Data[blocks_[x*ry+y].id].smartAction == SmartAction.ActionOpenClose) {
                CreateBlock(x, y, blockDataBase.Data[blocks_[x * ry + y].id].afterdeathId);
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

            blockDataBase = bdb_;
            floorDataBase = fdb_;
            schemesBase = sdb_;

            minimap = new Texture2D(spriteBatch.GraphicsDevice, rx, ry);

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
            MapGenerators.FillTest1(ref blocks_, ref floors_, rx, ry, 1);
            MapGenerators.GenerateStreetsNew(ref floors_, rx, ry, rnd.Next(80,200), rnd.Next(80,200), rnd.Next(20,30), 2, 3);
        }

        public GameLevel(int rx_ = 100, int ry_ = 100)
        {
            //spriteBatch_ = new SpriteBatch(null);
            flatlas_ = new Collection<Texture2D>();
            atlas_ = new Collection<Texture2D>();
            blockDataBase = new BlockDataBase(new Dictionary<int, BlockData>());
            floorDataBase = new FloorDataBase(new Dictionary<int, FloorData>());
            schemesBase = new SchemesDataBase(new List<Schemes>());
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

        public void GenerateMinimap(GraphicsDevice gd)
        {
            var data = new Color[rx * ry];
            for (int i = 0; i < floors_.Length; i++) {
                if (blocks_[i].explored) {
                    data[i] = floorDataBase.Data[floors_[i].ID].MMCol;
                } else {
                    data[i] = Color.Black;
                }
            }
            minimap.SetData(data);
        }

        public short[] CalcWision(Creature who, float dirAngle, float seeAngleDeg)
        {
            int x_top;
            int y_top;
            int dx_top = 0;
            int dy_top = 0;
            int dir;
            float dist;
            int seen;
            int max, min, tmax;
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
            } catch (Exception) {
            }


            x_top = pos_x - see;
            y_top = pos_y - see;


            for (dir = 0; dir < 4; dir++) {
                if (x_top < 0) {
                    x_top = 0;
                }
                if (x_top >= rx) {
                    x_top = rx - 1;
                }
                if (y_top < 0) {
                    y_top = 0;
                }
                if (y_top >= ry) {
                    y_top = ry - 1;
                }

                if (dir == 0) {
                    dx_top = +1;
                    dy_top = 0;
                }
                if (dir == 1) {
                    dx_top = 0;
                    dy_top = +1;
                }
                if (dir == 2) {
                    dx_top = -1;
                    dy_top = 0;
                }
                if (dir == 3) {
                    dx_top = 0;
                    dy_top = -1;
                }

                while (true) {

                    if (dir == 0 & ((x_top - pos_x) == see || (x_top - pos_x) >= rx - 1)) {
                        break;
                    }
                    if (dir == 1 & ((y_top - pos_y) == see || (y_top - pos_x) >= ry - 1)) {
                        break;
                    }
                    if (dir == 2 & ((pos_x - x_top) == see || (pos_x - x_top) >= rx - 1)) {
                        break;
                    }
                    if (dir == 3 & ((pos_y - y_top) == see || (pos_y - y_top) >= ry - 1)) {
                        break;
                    }

                    x1 = pos_x;
                    y1 = pos_y;
                    x2 = x_top;
                    y2 = y_top;

                    px = x2 - x1;
                    py = y2 - y1;
                    s_x = (px >= 0) ? 1 : -1;
                    s_y = (py >= 0) ? 1 : -1;
                    px = (px >= 0) ? px : -px;
                    py = (py >= 0) ? py : -py;

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

                    tmax = max;
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

                        if (pos_x == x_top || pos_y == y_top) {
                            seen = see - 1;
                        } else {
                            seen = see;
                        }
                        dist = Convert.ToInt32(Math.Sqrt(Math.Pow((pos_x - x1), 2) + Math.Pow((pos_y - y1), 2)));
                        if (dist > seen) {
                            break;
                        }

                        float curangle;
                        curangle = (float)Math.Atan2(y1 - pos_y, x1 - pos_x);
                        float wrapedangle = MathHelper.WrapAngle(dirAngle);
                        if ((curangle - MathHelper.ToRadians(seeAngleDeg) > wrapedangle || curangle + MathHelper.ToRadians(seeAngleDeg) < wrapedangle) &&
                            (curangle - MathHelper.ToRadians(seeAngleDeg) > wrapedangle + MathHelper.TwoPi || curangle + MathHelper.ToRadians(seeAngleDeg) < wrapedangle + MathHelper.TwoPi) &&
                            (curangle - MathHelper.ToRadians(seeAngleDeg) > wrapedangle - MathHelper.TwoPi || curangle + MathHelper.ToRadians(seeAngleDeg) < wrapedangle - MathHelper.TwoPi)) {
                            break;
                        }

                        if (x1 >= 0 && x1 < rx && y1 >= 0 && y1 < ry &&
                            (n[(x1 - los_x_null) * ry + y1 - los_y_null] != 0 || n[(x1 - los_x_null) * ry + y1 - los_y_null] == -1)) {
                            if (!blockDataBase.Data[blocks_[x1 * ry + y1].id].isTransparent) {
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

                    x_top += dx_top;
                    y_top += dy_top;
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

        public void Draw(GameTime gameTime, Vector2 camera_)
        {
            var min = new Vector2((camera_.X) / Settings.FloorSpriteSize.X, (camera_.Y) / Settings.FloorSpriteSize.Y);
            var max = new Vector2((camera_.X + Settings.Resolution.X) / Settings.FloorSpriteSize.X,
                                  (camera_.Y + Settings.Resolution.Y) / Settings.FloorSpriteSize.Y);

            min.X = MathHelper.Max(min.X, 0);
            min.Y = MathHelper.Max(min.Y, 0);
            max.X = MathHelper.Min(max.X + 1, rx);
            max.Y = MathHelper.Min(max.Y + 1, ry);


            for (int i = (int)min.X, ci = 0; i < (int)max.X; i++, ci++) {
                for (int j = (int)min.Y, cj = 0; j < (int)max.Y; j++, cj++) {
                    int a = i * ry + j;
                    spriteBatch_.Draw(flatlas_[floorDataBase.Data[floors_[a].ID].Mtex],
                                      new Vector2(i * (Settings.FloorSpriteSize.X) - (int)camera_.X,
                                                  j * (Settings.FloorSpriteSize.Y) - (int)camera_.Y),
                                      null, blocks_[a].lightness);
                }
            }
        }

        public void Draw2(GameTime gameTime, Vector2 camera_)
        {
            var min = new Vector2((camera_.X) / Settings.FloorSpriteSize.X, (camera_.Y) / Settings.FloorSpriteSize.Y);
            var max = new Vector2((camera_.X + Settings.Resolution.X) / Settings.FloorSpriteSize.X,
                                  (camera_.Y + Settings.Resolution.Y) / Settings.FloorSpriteSize.Y);

            min.X = MathHelper.Max(min.X, 0);
            min.Y = MathHelper.Max(min.Y, 0);
            max.X = MathHelper.Min(max.X + 1, rx);
            max.Y = MathHelper.Min(max.Y + 1, ry);


            for (int i = (int)min.X, ci = 0; i < (int)max.X; i++, ci++) {
                for (int j = (int)min.Y, cj = 0; j < (int)max.Y; j++, cj++) {
                    int a = i * ry + j;
                    spriteBatch_.Draw(atlas_[blockDataBase.Data[blocks_[a].id].texNo],
                                      new Vector2(i * (Settings.FloorSpriteSize.X) - (int)camera_.X,
                                                  j * (Settings.FloorSpriteSize.Y) - (int)camera_.Y),
                                      null, blocks_[a].lightness);
                }
            }
        }

        public bool IsCreatureMeele(int nx, int ny, Player player) {
            return (Settings.GetMeeleActionRange() >=
                    Vector2.Distance(new Vector2((nx + 0.5f)*32, (ny + 0.5f)*32), new Vector2(player.Position.X, player.Position.Y)));
        }

        public Texture2D GetMinimap() {
            return minimap;
        }
    }
}