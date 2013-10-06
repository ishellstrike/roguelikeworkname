using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Generation;

namespace rglikeworknamelib.Dungeon.Level {
    public class GameLevel
    {
        public int MapSeed = 23142455;

        private Texture2D whitepixel;
        private Texture2D transparentpixel;

        private Dictionary<Point, MapSector> sectors_;
        public int generated;

        private readonly List<StreetOld__> streets_ = new List<StreetOld__>();
        private readonly SpriteBatch spriteBatch_;
        private readonly GraphicsDevice gd_;
        private readonly SpriteFont font_;
        public LevelWorker lw_;

        public bool MapJustUpdated;

        private RenderTarget2D minimap_;
        private RenderTarget2D map_;
        private Dictionary<Tuple<int, int>, SectorBiom> globalMap = new Dictionary<Tuple<int, int>, SectorBiom>();
        public HashSet<Tuple<int, int>> RoadSectors = new HashSet<Tuple<int, int>>();

            #region Constructor
        Effect be_;
        public GameLevel(SpriteBatch spriteBatch, SpriteFont sf, GraphicsDevice gd, LevelWorker lw)
        {
            MapGenerators.seed = MapSeed;
            whitepixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            var data = new uint[1];
            data[0] = 0xffffffff;
            whitepixel.SetData(data);

            transparentpixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            data[0] = 0x0;
            transparentpixel.SetData(data);

            minimap_ = new RenderTarget2D(spriteBatch.GraphicsDevice, 121, 121);
            map_ = new RenderTarget2D(spriteBatch.GraphicsDevice, 671, 671);

            sectors_ = new Dictionary<Point, MapSector>();
            //{
            //    new KeyValuePair<Point, MapSector>(Point.Zero, new MapSector(this, 0, 0));
            //}

            //sectors_.Last().Value.Rebuild(MapSeed);

            spriteBatch_ = spriteBatch;
            font_ = sf;
            gd_ = gd;
            be_ = new BasicEffect(gd_);
            (be_ as BasicEffect).DiffuseColor = Color.Black.ToVector3();

            lw_ = lw;

            LoadRoadmap();
            LoadMap();
        }

        public GameLevel()
        {
            sectors_ = new Dictionary<Point, MapSector>();
        }
        #endregion

        /// <summary>
        /// Explore all active sector
        /// </summary>
        [Obsolete]
        public void ExploreAllMap()
        {
            foreach (var mapSector in sectors_) {
                mapSector.Value.ExploreAllSector();
            }
        }

        public List<StorageBlock> GetStorageBlocks() {
            var a =  sectors_.Select(x => x.Value.GetStorageBlocks());
            var b = new List<StorageBlock>();
            foreach (var some in a) {
                b.AddRange(some);
            }
            return b;
        }

        //------------------

        public MapSector[] GetNeibours(MapSector ms)
        {
            return GetNeibours(ms.SectorOffsetX, ms.SectorOffsetY);
        }

        public MapSector[] GetNeibours(int sectorOffsetX, int sectorOffsetY)
        {
            List<MapSector> arrr = new List<MapSector>();

            MapSector a = GetRightN(sectorOffsetX, sectorOffsetY), b = GetLeftN(sectorOffsetX, sectorOffsetY), c = GetUpN(sectorOffsetX, sectorOffsetY), d = GetDownN(sectorOffsetX, sectorOffsetY);

            if (a != null) arrr.Add(a);
            if (b != null) arrr.Add(b);
            if (c != null) arrr.Add(c);
            if (d != null) arrr.Add(d);

            return arrr.ToArray();
        }

        public MapSector GetDownN(int sectorOffsetX, int sectorOffsetY) {
            MapSector a;
            sectors_.TryGetValue(new Point(sectorOffsetX, sectorOffsetY+1), out a);
            return a;
        }

        public MapSector GetUpN(int sectorOffsetX, int sectorOffsetY)
        {
            MapSector a;
            sectors_.TryGetValue(new Point(sectorOffsetX, sectorOffsetY-1), out a);
            return a;
        }

        public MapSector GetLeftN(int sectorOffsetX, int sectorOffsetY)
        {
            MapSector a;
            sectors_.TryGetValue(new Point(sectorOffsetX-1, sectorOffsetY), out a);
            return a;
        }

        public MapSector GetRightN(int sectorOffsetX, int sectorOffsetY)
        {
            MapSector a;
            sectors_.TryGetValue(new Point(sectorOffsetX+1, sectorOffsetY), out a);
            return a;
        }

        public MapSector GetSector(int sectorOffsetX, int sectorOffsetY, bool noLoading = false)
        {
            MapSector a;
            if (sectors_.TryGetValue(new Point(sectorOffsetX, sectorOffsetY), out a)) {
                return a;
            }

            if(noLoading) {
                return null;
            }

            var temp = lw_.TryGet(new Point(sectorOffsetX, sectorOffsetY), this);
            if (temp != null) {
                GlobalMapAdd(temp);
                sectors_.Add(new Point(temp.SectorOffsetX, temp.SectorOffsetY), temp);
            }
            return null;
        }

        private void GlobalMapAdd(MapSector temp) {
            var a = new Tuple<int, int>(temp.SectorOffsetX, temp.SectorOffsetY);
            if (!globalMap.ContainsKey(a)) {
                globalMap.Add(a, temp.biom);
            }
        }

        public IBlock GetBlock(int x, int y, bool noLoading = false)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy, noLoading);
            if (sect != null)
                return sect.GetBlock(x - divx * MapSector.Rx, y - divy * MapSector.Ry);//blocks_[x * ry + y];

            return null;
        }

        public ICreature GetCreatureAtCoord(Vector2 pos, Vector2 start) {
            var p = GetInSectorPosition(GetPositionInBlocks(pos));
            MapSector sect;

            sectors_.TryGetValue(new Point((int)p.X, (int)p.Y), out sect);

            if (sect == null) return null;

            var adder = new Vector2(sect.SectorOffsetX * MapSector.Rx * 32, sect.SectorOffsetY * MapSector.Ry * 32);

            foreach (var crea in sect.creatures) {
                if (Intersects(start, pos, new Vector2(crea.Position.X - 16, crea.Position.Y - 32) + adder, new Vector2(crea.Position.X - 16, crea.Position.Y) + adder))
                    return crea;
                if (Intersects(start, pos, new Vector2(crea.Position.X - 16, crea.Position.Y - 32) + adder, new Vector2(crea.Position.X + 16, crea.Position.Y-32) + adder))
                    return crea;
                if (Intersects(start, pos, new Vector2(crea.Position.X + 16, crea.Position.Y) + adder, new Vector2(crea.Position.X + 16, crea.Position.Y-32) + adder))
                    return crea;
                if (Intersects(start, pos, new Vector2(crea.Position.X + 16, crea.Position.Y) + adder, new Vector2(crea.Position.X - 16, crea.Position.Y) + adder))
                    return crea;
            }
            //return sect.creatures.FirstOrDefault(crea => crea.Position.X >= pos.X - 16 && crea.Position.Y >= pos.Y - 32 && crea.Position.X <= pos.X + 16 && crea.Position.Y <= pos.Y);
            return null;
        }

        static bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            return true;
        }

        public bool IsExplored(int x, int y) {
            int divx = x < 0 ? (x + 1)/MapSector.Rx - 1 : x/MapSector.Rx;
            int divy = y < 0 ? (y + 1)/MapSector.Ry - 1 : y/MapSector.Ry;

            var sect = GetSector(divx, divy);
            return sect.GetBlock(x - divx * MapSector.Rx, y - divy * MapSector.Ry).Explored;
        }

        public bool IsWalkable(int x, int y) {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            if(sect == null) return false;
            return sect.GetBlock(x-divx*MapSector.Rx, y-divy*MapSector.Ry).Data.IsWalkable;
        }

        public string GetId(int x, int y) {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            return sect.GetBlock(x - divx*MapSector.Rx, y - divy*MapSector.Ry).Id;
        }

        public void SetFloor(int x, int y, string floorId)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);

            if (sect != null) {
                sect.SetFloor(x - divx*MapSector.Rx, y - divy*MapSector.Ry, floorId);
            }
        }

        public void SetBlock(int x, int y, string blockId)
        {
            int divx = x < 0 ? (x + 1) / MapSector.Rx - 1 : x / MapSector.Rx;
            int divy = y < 0 ? (y + 1) / MapSector.Ry - 1 : y / MapSector.Ry;
            var sect = GetSector(divx, divy);
            if (sect != null)
            {
                var braw = (x - divx * MapSector.Rx) * MapSector.Ry + y - divy * MapSector.Ry;
                if (BlockDataBase.Data[blockId].Prototype == typeof(Block))
                {

                    sect.SetBlock(braw, new Block {
                        Id = blockId,
                        data = BlockDataBase.Data[blockId]
                    });
                }
                if (BlockDataBase.Data[blockId].Prototype == typeof(StorageBlock))
                {
                    sect.SetBlock(braw, new StorageBlock
                    {
                        StoredItems=new List<Item.Item>(),
                        Id=blockId,
                        data = BlockDataBase.Data[blockId]
                    });
                }
                MapJustUpdated = true;
            }
        }

        /// <summary>
        /// Sector position from block position
        /// </summary>
        /// <param name="Pos">position on blocks</param>
        /// <returns>position in sectors</returns>
        public Vector2 GetInSectorPosition(Vector2 Pos) {
            return new Vector2(Pos.X < 0 ? (Pos.X + 1) / MapSector.Rx - 1 : Pos.X / MapSector.Rx, Pos.Y < 0 ? (Pos.Y + 1) / MapSector.Ry - 1 : Pos.Y / MapSector.Ry);
        }
        /// <summary>
        /// Get position in blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks(Vector2 po)
        {
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        public void OpenCloseDoor(int x, int y) {
            var a = GetBlock(x, y);
            if(a.Data.SmartAction == SmartAction.ActionOpenClose) {
                if (a.Data.IsWalkable) {
                    EventLog.Add("Вы закрыли дверь", GlobalWorldLogic.CurrentTime, Color.Gray, LogEntityType.OpenCloseDor);
                }
                else {
                    EventLog.Add("Вы открыли дверь", GlobalWorldLogic.CurrentTime, Color.LightGray, LogEntityType.OpenCloseDor);
                    Achievements.Stat["dooro"].Count++;
                }
                SetBlock(x, y, GetBlock(x,y).Data.AfterDeathId);
                MapJustUpdated = true;
            }
        }

        private TimeSpan sec;
        public void KillFarSectors(Creature cara, GameTime gt, bool ignore = false) {
            sec += gt.ElapsedGameTime;
            if (sec.TotalMilliseconds >= 500 || ignore) {
                sec = TimeSpan.Zero;
                for (int i = 0; i < sectors_.Count; i++) {
                    var a = sectors_.ElementAt(i);
                    if (Math.Abs((a.Value.SectorOffsetX + 0.5) * MapSector.Rx - cara.GetWorldPositionInBlocks().X) > 32 ||
                        Math.Abs((a.Value.SectorOffsetY + 0.5) * MapSector.Ry - cara.GetPositionInBlocks().Y) > 32)
                    {
                        sectors_.Remove(sectors_.ElementAt(i).Key);
                        lw_.Save(a.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Rebuild all active sectors
        /// </summary>
        [Obsolete]
        public void Rebuild() {
            for (int i = 0; i < sectors_.Count; i++) {
                sectors_.ElementAt(i).Value.Rebuild(MapSeed);
            }
        }

        public void GenerateMinimap(GraphicsDevice gd, SpriteBatch sb, Creature pl)
        {
            gd.SetRenderTarget(minimap_);
            gd.Clear(Color.Transparent);
            sb.Begin();
            var pos = GetInSectorPosition(pl.GetPositionInBlocks());
            for (int i = (int)pos.X - 5; i < pos.X + 6; i++)
            {
                for (int j = (int)pos.Y - 5; j < pos.Y + 6; j++)
                {
                    var a = new Tuple<int, int>(i, j);
                    if (globalMap.ContainsKey(a))
                    {
                        int x = i - (int)pos.X + 5;
                        int y = j - (int)pos.Y + 5;
                        var t = GetMinimapData(globalMap[a]);
                        sb.Draw(t.Item1, new Vector2(x * 11, y * 11), t.Item2);
                    }
                }
            }
            sb.Draw(Atlases.MinimapAtlas["cross1"], new Vector2(5 * 11, 5 * 11), Color.Red);
            sb.End();
            gd.SetRenderTarget(null);
        }

        public void GenerateMap(GraphicsDevice gd, SpriteBatch sb, Creature pl)
        {
            gd.SetRenderTarget(map_);
            gd.Clear(Color.Transparent);
            sb.Begin();
            var pos = GetInSectorPosition(pl.GetPositionInBlocks());
            for (int i = (int)pos.X - 30; i < pos.X + 31; i++)
            {
                for (int j = (int)pos.Y - 30; j < pos.Y + 31; j++)
                {
                    var a = new Tuple<int, int>(i, j);
                    if (globalMap.ContainsKey(a))
                    {
                        int x = i - (int)pos.X + 30;
                        int y = j - (int)pos.Y + 30;
                        var t = GetMinimapData(globalMap[a]);
                        sb.Draw(t.Item1, new Vector2(x * 11, y * 11), t.Item2);
                    }
                }
            }
            sb.Draw(Atlases.MinimapAtlas["cross1"], new Vector2(30 * 11, 30 * 11), Color.Red);
            sb.End();
            gd.SetRenderTarget(null);
        }

        private Tuple<Texture2D, Color> GetMinimapData(SectorBiom bi) {
            Tuple<Texture2D, Color> a;

            switch (bi) {
                case SectorBiom.Forest:
                case SectorBiom.WildForest:
                case SectorBiom.SuperWildForest:
                    a = new Tuple<Texture2D, Color>(Atlases.MinimapAtlas["forest1"], Color.Green);
                    break;
                case SectorBiom.House:
                    a = new Tuple<Texture2D, Color>(Atlases.MinimapAtlas["house1"], Color.White);
                    break;
                case SectorBiom.RoadCross:
                case SectorBiom.RoadHevt:
                case SectorBiom.RoadHor:
                    a = new Tuple<Texture2D, Color>(Atlases.MinimapAtlas["cross1"], Color.Gray);
                    break;
                default:
                    a = new Tuple<Texture2D, Color>(Atlases.MinimapAtlas["nothing1"], Color.White);
                    break;
            }

            return a;
        }


        private Vector2 pre_pos_vis;
        public void CalcWision(Creature who, float dirAngle, float seeAngleDeg) {
            var a = who.GetPositionInBlocks();
            var colb = Color.Black;

            if (Vector2.Distance(pre_pos_vis, a) > 0 || MapJustUpdated) {

                for (int i = -20; i < 20; i++) {
                    for (int j = -20; j < 20; j++) {
                        var t = GetBlock((int) a.X + i, (int) a.Y + j);
                        if (t != null) {
                            t.Lightness = colb;
                        }
                    }
                }

                IBlock temp2;
                var lightness = Color.White;
                for (int i = -20; i < 20; i++) {
                    for (int j = -20; j < 20; j++) {
                        temp2 = GetBlock((int) a.X + i, (int) a.Y + j);
                        if (temp2 != null && temp2.Id != "0" && PathClear(a, new Vector2(a.X + i, a.Y + j))) {
                            temp2.Lightness = lightness;
                            temp2.Explored = true;
                        }
                    }
                }

                var temp = GetBlock((int) a.X, (int) a.Y);
                if (temp != null) {
                    temp.Lightness = lightness;
                    temp.Explored = true;
                }

                for (int i = 0; i < sectors_.Count; i++) {
                    var sector = sectors_.ElementAt(i).Value;
                    foreach (var crea in sector.creatures) {
                        if (Vector2.Distance(who.Position, crea.WorldPosition()) < 1000 && PathClear(a,
                                      new Vector2(crea.GetWorldPositionInBlocks().X, crea.GetWorldPositionInBlocks().Y)))
                        {
                            temp2 = GetBlock((int)crea.GetWorldPositionInBlocks().X,
                                             (int)crea.GetWorldPositionInBlocks().Y);
                            if (temp2 != null) {
                                temp2.Lightness = lightness;
                                temp2.Explored = true;
                            }
                        }
                    }
                }
            }
            pre_pos_vis = a;
        }

        public bool PathClear(Vector2 start, Vector2 end)
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
                        var key = GetBlock((int) checkPoint.X, (int) checkPoint.Y);
                        if (key != null && !key.Data.IsTransparent) {
                            GetBlock((int) checkPoint.X, (int) checkPoint.Y).Lightness = Color.White;
                            return false;
                        }
                        if(key == null) {
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

                        var t = GetBlock((int) checkPoint.X, (int) checkPoint.Y);

                        if (t!= null && t.Id != null && !t.Data.IsTransparent) {
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

        /// <summary>
        /// Main loop creature update
        /// </summary>
        /// <param name="gt"></param>
        public void UpdateCreatures(GameTime gt, Player hero) {
            for (int k = 0; k < sectors_.Count; k++) {
                var sector = sectors_.ElementAt(k).Value;
                for (int m = 0; m < sector.creatures.Count; m++) {
                    sector.creatures[m].Skipp = false;
                }
            }

            for (int k = 0; k < sectors_.Count; k++) {
                var sector = sectors_.ElementAt(k).Value;
                for (int m = 0; m < sector.creatures.Count; m++)
                {
                    var crea = sector.creatures[m];

                    if (crea.isDead)
                    {
                        sector.creatures.Remove(crea);
                        continue; 
                    }

                    crea.Update(gt, sector, hero);
                }
            }
        }

        /// <summary>
        /// Main loop blocks update
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="camera"></param>
        public void UpdateBlocks(GameTime gt, Vector2 camera) {
        }

        private void AddShadowpointForBlock(Vector2 camera, Creature per, float xpos, float ypos, int wide) {
            var po = GetAtBlockPoints(xpos, ypos, wide, wide);

            for (int k = 0; k < po.Length; k++) {
                po[k] = XyToVector3(po[k]);
            }
            Vector3 car = new Vector3(per.Position.X - camera.X, per.Position.Y - camera.Y, 0);
            car = XyToVector3(car);

            //лучи ко всем вершинам блока
            var r1 = new Ray(car, Vector3.Normalize(po[0] - car));
            var r2 = new Ray(car, Vector3.Normalize(po[1] - car));
            var r3 = new Ray(car, Vector3.Normalize(po[2] - car));
            var r4 = new Ray(car, Vector3.Normalize(po[3] - car));

            Vector3[] po2 = new[] {
                                      GetBorderIntersection(r1), GetBorderIntersection(r2), GetBorderIntersection(r3),
                                      GetBorderIntersection(r4)
                                  };

            int[] spoints;

            float playerPosX = per.Position.X - camera.X;
            float plauerPosY = per.Position.Y - camera.Y;


            //Выбор крайних точек блока 
            if (playerPosX <= xpos) {
                //left column
                spoints = plauerPosY <= ypos
                              ? new[] {1, 3}
                              : (plauerPosY <= ypos + 32 ? new[] {0, 3} : new[] {0, 2});
            }
            else if (playerPosX <= xpos + 32) {
                //middle column
                spoints = plauerPosY <= ypos
                              ? new[] {1, 0}
                              : (plauerPosY <= ypos + 32 ? new[] {0, 0} : new[] {3, 2});
            }
            else {
                //right column
                spoints = plauerPosY <= ypos
                              ? new[] {2, 0}
                              : (plauerPosY <= ypos + 32 ? new[] {2, 1} : new[] {3, 1});
            }

            //Making shadow polys
            points.Add(new VertexPositionColor(po2[spoints[0]], Color.Black));
            points.Add(new VertexPositionColor(po[spoints[0]], Color.Black));
            points.Add(new VertexPositionColor(po2[spoints[1]], Color.Black));
            points.Add(new VertexPositionColor(po[spoints[0]], Color.Black));
            points.Add(new VertexPositionColor(po[spoints[1]], Color.Black));
            points.Add(new VertexPositionColor(po2[spoints[1]], Color.Black));
        }

        /// <summary>
        /// Использует неприведенные координаты
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="resx">Размер квадрата, кастующего тень</param>
        /// <param name="resy">Размер квадрата, кастующего тень</param>
        /// <returns></returns>
        private static Vector3[] GetAtBlockPoints(float xpos, float ypos, int resx, int resy) {

            var vix = (32 - resx)/2.0f;
            var viy = (32 - resy)/2.0f;

            Vector3 p1 = new Vector3(xpos+vix, ypos+vix, 0);
            Vector3 p2 = new Vector3(xpos + resx + vix, ypos + viy, 0);
            Vector3 p3 = new Vector3(xpos + resx + vix, ypos + resy + viy, 0);
            Vector3 p4 = new Vector3(xpos+vix, ypos + resy+viy, 0);

            return new[] {p1, p2, p3, p4};
        }

        /// <summary>
        /// Использует приведенные координаты
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Vector3 GetBorderIntersection(Ray r)
        {
            //return GetSlosestIntersectionPoint(r, upPlane, downPlane, leftPlane, rightPlane);
            return r.Position + r.Direction*5;
        }

        List<VertexPositionColor> points = new List<VertexPositionColor>();

        public void ShadowRender() {

            gd_.RasterizerState = new RasterizerState() {CullMode = CullMode.None, FillMode = FillMode.Solid};
            gd_.DepthStencilState = DepthStencilState.Default;
            gd_.BlendState = BlendState.AlphaBlend;
            ((BasicEffect) be_).DiffuseColor = Color.Black.ToVector3();

            foreach (EffectPass pass in be_.CurrentTechnique.Passes) {
                pass.Apply();

                if (points.Count != 0) {
                    gd_.DrawUserPrimitives(PrimitiveType.TriangleList, points.ToArray(), 0, points.Count/3);
                }
            }

            if(Settings.DebugWire) {
                gd_.RasterizerState = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
                gd_.DepthStencilState = DepthStencilState.Default;
                gd_.BlendState = BlendState.AlphaBlend;
                (be_ as BasicEffect).DiffuseColor = Color.White.ToVector3();

                foreach (EffectPass pass in be_.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (points.Count != 0)
                    {
                        gd_.DrawUserPrimitives(PrimitiveType.TriangleList, points.ToArray(), 0, points.Count / 3);
                    }
                }
            }
        }

        /// <summary>
        /// Приводит координаты из [0,resolution] к [-1,1] представлению
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        private Vector3 XyToVector3(Vector3 vec)
        {
            var nx = (vec.X / Settings.Resolution.X) * 2.0 - 1;
            var ny = -(vec.Y / Settings.Resolution.Y) * 2.0 + 1;
            return new Vector3((float)nx, (float)ny, 0.0f);
        }

        public bool IsCreatureMeele(int nx, int ny, Player player) {
            return (Settings.GetMeeleActionRange() >=
                    Vector2.Distance(new Vector2((nx + 0.5f)*32, (ny + 0.5f)*32), new Vector2(player.Position.X, player.Position.Y)));
        }

        public Texture2D GetMinimap() {
            return minimap_;
        }

        public Texture2D GetMap() {
            return map_;
        }

        public int SectorCount() {
            return sectors_.Count;
        }

        public void SaveAll() {
            Action a = SaveAllAsync;
            a.BeginInvoke(null, null);
        }

        private bool all_saved;
        private void SaveAllAsync() {
            Settings.NTS1 = "Saving Map";
            Settings.NeedToShowInfoWindow = true;
            SaveMap();
            Settings.NeedToShowInfoWindow = true;
            Settings.NTS1 = "Saving : ";
            Settings.NTS2 = "";
            if(sectors_.Count > 0) {
                for(int i=0; i<sectors_.Count; i++) {
                    Settings.NTS2 = i+"/"+sectors_.Count;
                    Settings.NeedToShowInfoWindow = true;
                    lw_.SaveSector(sectors_.ElementAt(i).Value); 
                }
            }
            Settings.NeedExit = true;
        }

        private void SaveMap() {
            BinaryFormatter binaryFormatter_ = new BinaryFormatter();
            FileStream fileStream_;
            GZipStream gZipStream_;

            fileStream_ =
                new FileStream(Settings.GetWorldsDirectory() + string.Format("map.rlm"),
                               FileMode.Create);
            gZipStream_ = new GZipStream(fileStream_, CompressionMode.Compress);
            binaryFormatter_.Serialize(gZipStream_, globalMap);
            gZipStream_.Close();
            fileStream_.Close();
        }

        private void LoadMap()
        {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("map.rlm"))) {
                BinaryFormatter binaryFormatter_ = new BinaryFormatter();
                FileStream fileStream_;
                GZipStream gZipStream_;

                fileStream_ =
                    new FileStream(Settings.GetWorldsDirectory() + string.Format("map.rlm"),
                                   FileMode.Open);
                gZipStream_ = new GZipStream(fileStream_, CompressionMode.Decompress);
                globalMap = (Dictionary<Tuple<int, int>, SectorBiom>) binaryFormatter_.Deserialize(gZipStream_);
                gZipStream_.Close();
                fileStream_.Close();
            }
        }

        private void LoadRoadmap()
        {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("roadmap.rlm")))
            {
                var binaryFormatter = new BinaryFormatter();

                var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("roadmap.rlm"),
                                                        FileMode.Open);
                GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                RoadSectors = (HashSet<Tuple<int, int>>)binaryFormatter.Deserialize(gZipStream);
                gZipStream.Close();
                fileStream.Close();
            } else {
                RoadSectors = MapGenerators.GenerateRoadmap(MapSeed);
                var binaryFormatter = new BinaryFormatter();

                var fileStream_ = new FileStream(Settings.GetWorldsDirectory() + string.Format("roadmap.rlm"),
                                                        FileMode.Create);
                var gZipStream_ = new GZipStream(fileStream_, CompressionMode.Compress);
                    binaryFormatter.Serialize(gZipStream_, RoadSectors);
                gZipStream_.Close();
                fileStream_.Close();
            }
        }

        public int GetShadowrenderCount() {
            return points.Count;
        }

        // Shadow casting region

        #region Drawes
        private Vector2 min, max;
        public void DrawFloors(GameTime gameTime, Vector2 camera)
        {
            var fatlas = Atlases.FloorAtlas; // Make field's non-static
            var fdb = FloorDataBase.Data;
            var rx = MapSector.Rx;
            var ry = MapSector.Ry;
            var ssx = Settings.FloorSpriteSize.X;
            var ssy = Settings.FloorSpriteSize.Y;

            GetBlock((int)(camera.X / 32), (int)(camera.Y / 32));
            GetBlock((int)((camera.X + Settings.Resolution.X) / 32), (int)((camera.Y + Settings.Resolution.Y) / 32));
            GetBlock((int)((camera.X + Settings.Resolution.X) / 32), (int)((camera.Y) / 32));
            GetBlock((int)((camera.X) / 32), (int)((camera.Y + Settings.Resolution.Y) / 32));

            min = new Vector2((camera.X) / ssx - 1, (camera.Y) / ssy - 1);
            max = new Vector2((camera.X + Settings.Resolution.X) / ssx,
                                  (camera.Y + Settings.Resolution.Y) / ssy);

            for (int k = 0; k < sectors_.Count; k++)
            {
                MapSector sector = sectors_.ElementAt(k).Value;
                if (sector.SectorOffsetX * rx + rx < min.X &&
                    sector.SectorOffsetY * ry + ry < min.Y)
                {
                    continue;
                }
                if (sector.SectorOffsetX * rx > max.X && sector.SectorOffsetY * ry > max.Y)
                {
                    continue;
                }
                for (int i = 0; i < rx; i++)
                {
                    for (int j = 0; j < ry; j++)
                    {
                        if (sector.SectorOffsetX * rx + i > min.X &&
                            sector.SectorOffsetY * ry + j > min.Y &&
                            sector.SectorOffsetX * rx + i < max.X &&
                            sector.SectorOffsetY * ry + j < max.Y)
                        {
                            int a = i * ry + j;
                            spriteBatch_.Draw(fatlas,
                                              new Vector2(
                                                  i * ssx - (int)camera.X +
                                                  rx * ssx * sector.SectorOffsetX,
                                                  j * ssy - (int)camera.Y +
                                                  ry * ssy * sector.SectorOffsetY), 
                                                  sector.Floors[a].Source, 
                                                  Color.White);
                        }
                    }
                }
            }
        }

        public void DrawDecals(GameTime gameTime, Vector2 camera) {
            var atl = Atlases.ParticleAtlas;
            var rx = MapSector.Rx;
            var ry = MapSector.Ry;

            for (int k = 0; k < sectors_.Count; k++)
            {
                MapSector sector = sectors_.ElementAt(k).Value;
                if (sector.SectorOffsetX * rx + rx < min.X &&
                    sector.SectorOffsetY * ry + ry < min.Y)
                {
                    continue;
                }
                if (sector.SectorOffsetX * rx > max.X && sector.SectorOffsetY * ry > max.Y)
                {
                    continue;
                }

                for (int index = 0; index < sector.decals.Count; index++) {
                    var dec = sector.decals[index];
                    spriteBatch_.Draw(Atlases.ParticleAtlas[dec.MTex],
                                      dec.Pos - camera,
                                      null, dec.Color, dec.Rotation,
                                      new Vector2(atl[dec.MTex].Height/2f, atl[dec.MTex].Width/2f), dec.Scale,
                                      SpriteEffects.None, 0);
                }
            }
        }

        private Vector2 perPrew_;
        public void DrawBlocks(GameTime gameTime, Vector2 camera, Creature per)
        {
            var batlas = Atlases.BlockAtlas; // Make field's non-static
            var rx = MapSector.Rx;
            var ry = MapSector.Ry;
            var ssx = Settings.FloorSpriteSize.X;
            var ssy = Settings.FloorSpriteSize.Y;

            bool shad = Vector2.Distance(camera, perPrew_) > 2;

            if (shad || MapJustUpdated)
            {
                points.Clear();
            }

            for (int k = 0; k < sectors_.Count; k++)
            {
                MapSector sector = sectors_.ElementAt(k).Value;
                if (sector.SectorOffsetX * rx + rx < min.X &&
                    sector.SectorOffsetY * ry + ry < min.Y)
                {
                    continue;
                }
                if (sector.SectorOffsetX * rx > max.X && sector.SectorOffsetY * ry > max.Y)
                {
                    continue;
                }
                for (int i = 0; i < rx; i++)
                {
                    for (int j = 0; j < ry; j++)
                    {

                        var xpos = i * (ssx) - (int)camera.X +
                                       rx * ssx * sector.SectorOffsetX;

                        var ypos = j * (ssy) - (int)camera.Y +
                                   ry * ssy * sector.SectorOffsetY;

                        int a = i * ry + j;
                        var block = sector.GetBlock(a);

                        if (sector.SectorOffsetX * rx + i > min.X &&
                            sector.SectorOffsetY * ry + j > min.Y &&
                            sector.SectorOffsetX * rx + i < max.X &&
                            sector.SectorOffsetY * ry + j < max.Y)
                        {

                            if(block.Id != "0") {
                                spriteBatch_.Draw(batlas,new Vector2(xpos, ypos), block.Source, block.Lightness);
                            }


                            if ((shad || MapJustUpdated) && !sector.GetBlock(a).Data.IsTransparent)
                            {
                                AddShadowpointForBlock(camera, per, xpos, ypos, block.Data.swide);
                            }
                        }

                        block.Update(gameTime.ElapsedGameTime, new Vector2(xpos + camera.X, ypos + camera.Y));
                    }
                }

                perPrew_ = per.Position;

                if (Settings.DebugInfo)
                {
                    Vector2 ff = new Vector2(-(int)camera.X +
                                             MapSector.Rx * ssx *
                                             sector.SectorOffsetX, -(int)camera.Y +
                                                                   MapSector.Ry * ssy *
                                                                   sector.SectorOffsetY);

                    spriteBatch_.Draw(whitepixel, ff, null, Color.White, 0, Vector2.Zero, new Vector2(1, 1024),
                                      SpriteEffects.None, 0);
                    spriteBatch_.Draw(whitepixel, ff, null, Color.White, 0, Vector2.Zero, new Vector2(1024, 1),
                                      SpriteEffects.None, 0);

                    spriteBatch_.DrawString(font_, string.Format("({0},{1}) {2}",sector.SectorOffsetX, sector.SectorOffsetY,  sector.biom), new Vector2(20, 20) + ff, Color.White);
                }
            }
        }

        public void DrawCreatures(GameTime gameTime, Vector2 camera)
        {
            bool b = Settings.DebugInfo;
            for (int k = 0; k < sectors_.Count; k++)
            {
                var sector = sectors_.ElementAt(k).Value;
                if (sector.SectorOffsetX * MapSector.Rx + MapSector.Rx < min.X && sector.SectorOffsetY * MapSector.Ry + MapSector.Ry < min.Y) continue;
                if (sector.SectorOffsetX * MapSector.Rx > max.X && sector.SectorOffsetY * MapSector.Ry > max.Y) continue;

                for (int m = 0; m < sector.creatures.Count; m++)
                {
                    var crea = sector.creatures[m];
                    crea.Draw(spriteBatch_, camera, sector);
                }
            }
        }
#endregion

        public MapSector GetCreatureSector(Vector2 pos, Vector2 start) {
            var p = GetInSectorPosition(GetPositionInBlocks(pos));
            MapSector a;
            sectors_.TryGetValue(new Point((int)p.X, (int)p.Y), out a);
            return a;
        }

        public bool IsCreatureMeele(Creature hero, Creature ny) {
            return (Settings.GetMeeleActionRange() >=
                    Vector2.Distance(ny.WorldPosition(), hero.Position));
        }

        public IEnumerable<Light> GetLights() {
            var a = new List<Light>();
            if(sectors_.Count > 0) {
                for(int i = 0; i< sectors_.Count; i++) {
                    var sec = sectors_.ElementAt(i);
                    if(sec.Value.lights != null) {
                        a.AddRange(sec.Value.lights);
                    }
                }
            }
            return a;
        }
    }
}