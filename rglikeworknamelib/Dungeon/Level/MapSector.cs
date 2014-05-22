using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Dungeon.Vehicles;
using rglikeworknamelib.Generation;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class MapSector {
        /// <summary>
        ///     Sector ox size
        /// </summary>
        public const int Rx = 25;

        /// <summary>
        ///     Sectro oy size
        /// </summary>
        public const int Ry = 25;

        internal SectorBiom Biom;

        
        internal List<Creature> Creatures;
        internal List<Particle> Decals;
        internal Block[] Blocks;
        internal Floor[] Floors;
        internal List<Light> Lights;

        [NonSerialized]public GameLevel Parent;
        public bool Ready;
        public int SectorOffsetX, SectorOffsetY;
        internal List<Vehicle> Vehicles;

        public MapSector(int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;

            Blocks = new Block[Rx*Ry];
            Floors = new Floor[Rx*Ry];
            Creatures = new List<Creature>();

            int i = Rx*Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }
        }

        public MapSector(GameLevel parent, int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;
            Parent = parent;

            Blocks = new Block[Rx*Ry];
            Floors = new Floor[Rx*Ry];
            Creatures = new List<Creature>();
            Decals = new List<Particle>();
            Vehicles = new List<Vehicle>();

            int i = Rx*Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }
            Lights = new List<Light>();
        }

        /// <summary>
        ///     Map from just serialized data
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sectorOffsetX"></param>
        /// <param name="sectorOffsetY"></param>
        /// <param name="blocksArray"></param>
        /// <param name="floorsArray"></param>
        /// <param name="obiom"></param>
        /// <param name="creat"></param>
        /// <param name="decal"></param>
        public MapSector(GameLevel parent, object sectorOffsetX, object sectorOffsetY, object blocksArray,
                         object floorsArray, object obiom, object creat, object decal) {
            SectorOffsetX = (int) sectorOffsetX;
            SectorOffsetY = (int) sectorOffsetY;
            Parent = parent;

            Blocks = (Block[]) blocksArray;
            Floors = (Floor[]) floorsArray;
            Biom = (SectorBiom) obiom;
            Creatures = (List<Creature>) creat;
            Decals = (List<Particle>) decal;
            Lights = new List<Light>();
            Vehicles = new List<Vehicle>();
            ResetLightingSources();

            //foreach (IBlock block in Blocks) {
            //    block.OnLoad();
            //}
            //foreach (Floor floor in Floors) {
            //    floor.Source = FloorData.GetSource(floor.MTex);
            //}
        }

        public bool GeomReady;

        private List<Block> activeBlocks; 
        public List<Block> ActiveBlocks {
            get { return GeomReady ? activeBlocks ?? (activeBlocks = GetActive()) : (activeBlocks = GetActive()); }
        }

        private List<Block> GetActive() {
            var a = new List<Block>();
            foreach (var block in Blocks) {
                if (block.Data.Type != null) {
                    a.Add(block);
                }
            }
            return a;
        }

        public void ResetLightingSources() {
            Lights.Clear();
            //int i = 0;
            //for (int index = 0; index < Blocks.Length; index++) {
            //    IBlock block = Blocks[index];
            //    if (block is ILightSource) {
            //        Lights.Add(GetLights(block, i/Ry*32 + SectorOffsetX*Rx*32, i%Ry*32 + SectorOffsetY*Ry*32));
            //    }
            //    i++;
            //}
        }

        private Light GetLights(Block block, int x, int y) {
            //var a1 = block as ILightSource;
            //var t = new Light {
            //    Color = a1.LightColor,
            //    LightRadius = a1.LightRange,
            //    Power = a1.LightPower,
            //    Position = new Vector3(x + 16, y, 1 + 32)
            //};
            //return t;
            return new Light();
        }

        /// <summary>
        ///     Main sector generation proc
        /// </summary>
        /// <param name="mapseed">глобальный сид карты</param>
        public void Rebuild(int mapseed) {
            Action<int> a = AsyncGeneration;
            a(mapseed);
        }

        /// <summary>
        ///     ###---### Main generation proc ###---###
        /// </summary>
        /// <param name="mapseed">Global map seed</param>
        private void AsyncGeneration(int mapseed) {
            var s = (int) (MapGenerators.Noise2D(SectorOffsetX, SectorOffsetY)*int.MaxValue);
            var rand = new Random(s);

            Biom = GetBiom(SectorOffsetX, SectorOffsetY, Parent);

            MapGenerators.ClearBlocks(this);
            MapGenerators.FloorPerlin(this);

            switch (Biom) {
                case SectorBiom.Bushland:
                    int aa = rand.Next(3, 7);
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    }
                    break;

                case SectorBiom.Forest:
                    aa = rand.Next(2, 5);
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    }
                    break;

                case SectorBiom.WildForest:
                    aa = rand.Next(10, 20);
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    }
                    break;

                case SectorBiom.SuperWildForest:
                    aa = rand.Next(30, 40);
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    }
                    for (int i = 0; i < aa; i++) {
                        SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    }
                    break;

                    //case Se
            }


            //int rnd1 = rand.Next(0, 5);
            //int rnd2 = rand.Next(0, 3);
            if(rand.Next(5) == 1){
                    Spawn("rabbit", rand);
            }
            if (rand.Next(10) == 1)
            {
                Spawn("dog", rand);
            }
            //for (int i = 1; i < rnd2; i++) {
            //    Spawn("hdzombie", rand);
            //}

            ResetLightingSources();

            Parent.generated++;
            Ready = true;
        }

        public static SectorBiom GetBiom(int offX, int offY, GameLevel gl) {
            SectorBiom biom;
            var s = (int) (MapGenerators.Noise2D(offX, offY)*int.MaxValue);
            var rand = new Random(s);
            string most = MapGenerators.GetMost(offX, offY);

            //biom = (SectorBiom) rand.Next(0, Enum.GetNames(typeof (SectorBiom)).Length - 3);

            if(most == "1") {
                biom = SectorBiom.Field;
                if(rand.Next(5) == 0) {
                    biom = SectorBiom.AgroField;
                }
            } else {
                var fo = new[] {SectorBiom.Forest, SectorBiom.WildForest, SectorBiom.SuperWildForest, SectorBiom.Bushland, };
                biom = fo[rand.Next(fo.Length)];
                if(rand.Next(30) == 0) {
                    biom = SectorBiom.ForestSpider;
                }
            }

            return biom;
        }

        public void Spawn(string creatureId, Random rnd) {
            Spawn(creatureId, rnd.Next(0, Rx), rnd.Next(0, Ry));
        }

        public void Spawn(string creatureId, int x, int y) {
            Creatures.Add(CreatureFactory.GetInstance(creatureId, new Vector3(x * 32, y * 32, 0)));
        }

        public Block GetBlock(int x, int y) {
            return Blocks[x*Ry + y];
        }

        public Block GetBlock(int a) {
            return Blocks[a];
        }

        public void SetFloor(int x, int y, string floorId) {
            Floors[x*Ry + y].Id = floorId;
            Floors[x * Ry + y].MTex = Floors[x * Ry + y].Data.RandomMtexFromAlters();
            GeomReady = false;
        }

        public Floor GetFloor(int x, int y)
        {
            return Floors[x * Ry + y];
        }

        public string GetId(int x, int y) {
            return Blocks[x*Ry + y].Id;
        }

        public string GetId(int oneDimCoord) {
            return Blocks[oneDimCoord].Id;
        }

        /// <summary>
        ///     Base standart block setter
        /// </summary>
        /// <param name="oneDimCoord"></param>
        /// <param name="id"></param>
        public void SetBlock(int oneDimCoord, string id) {
            Blocks[oneDimCoord] = BlockFactory.GetInstance(id);
            Block block = Blocks[oneDimCoord];
            block.Id = id;
            block.MTex = block.Data.RandomMtexFromAlters();
            GeomReady = false;
        }

        /// <summary>
        ///     Base standart block setter
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="id"></param>
        public void SetBlock(int posX, int posY, string id) {
            SetBlock(posX*Ry + posY, id);
        }

        public void CreateAllMapFromArray(string[] arr) {
            for (int i = 0; i < Rx; i++) {
                for (int j = 0; j < Ry; j++) {
                    SetBlock(i, j, arr[i]);
                }
            }
            GeomReady = false;
        }

        public void AddDecal(Particle particle) {
            Decals.Add(particle);
            if (Decals.Count > Settings.DecalCount) {
                Decals.RemoveAt(0);
                Decals.RemoveAt(0);
            }
        }

        internal VertexPositionNormalTexture[] verteces, verteces_block, verteces_facer;
        internal VertexBufferBinding block_binding;
        internal VertexBuffer block_bufer;
        private IndexBuffer block_ibufer;
        internal short[] indexes_block;
        internal Vector3[] objWorld;
        internal BoundingBox bBox;
        public void RebuildGeometry() {
            var a = new List<VertexPositionNormalTexture>();
            var b = new List<VertexPositionNormalTexture>();
            var c = new List<short>();
            var d = new List<VertexPositionNormalTexture>();
            var e = new List<Vector3>();

            bBox = new BoundingBox(new Vector3(SectorOffsetX * Rx, SectorOffsetY * Ry, 0), new Vector3(SectorOffsetX * Rx + Rx, SectorOffsetY * Ry + Ry, 1));

            for (int i = 0; i < Rx; i++) {
                for (int j = 0; j < Ry; j++)
                {
                    Vector2 textureCoordinate = Floors[i * Rx + j].Source;
                    a.Add(new VertexPositionNormalTexture(
                              new Vector3(i, j, 0), Vector3.Up,
                              textureCoordinate));
                    a.Add(
                        new VertexPositionNormalTexture(
                            new Vector3(i + 1, j, 0), Vector3.Up,
                            textureCoordinate + new Vector2(Atlases.Instance.SpriteWidth, 0)));
                    a.Add(
                        new VertexPositionNormalTexture(
                            new Vector3(i, j + 1, 0), Vector3.Up,
                            textureCoordinate + new Vector2(0, Atlases.Instance.SpriteHeight)));

                    a.Add(
                        new VertexPositionNormalTexture(
                            new Vector3(i + 1, j, 0), Vector3.Up,
                            textureCoordinate + new Vector2(Atlases.Instance.SpriteWidth, 0)));
                    a.Add(
                        new VertexPositionNormalTexture(
                            new Vector3(i + 1, j + 1, 0), Vector3.Up,
                            textureCoordinate +
                            new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));
                    a.Add(
                        new VertexPositionNormalTexture(
                            new Vector3(i, j + 1, 0), Vector3.Up,
                            textureCoordinate + new Vector2(0, Atlases.Instance.SpriteHeight)));


                }
            }

            for (int i = 0; i < Rx; i++)
            {
                for (int j = 0; j < Ry; j++)
                {
                    Block block = Blocks[i * Rx + j];
                    if (block.Id != "0")
                    {
                        if (block.Data.Wallmaker) {
                            AddBlockGeom(b, c, i, j, block);
                            
                        }
                        else {
                             AddObjGeom(d, block);
                             e.Add(new Vector3(i + SectorOffsetX * Rx, j + SectorOffsetY * Ry, 0) + new Vector3(0.5f, 0.5f, 0.5f));
                        }
                    }
                }
            }

            verteces = a.ToArray();
            verteces_block = b.ToArray();
            verteces_facer = d.ToArray();
            indexes_block = c.ToArray();
            objWorld = e.ToArray();
            GeomReady = true;
        }

        private void AddObjGeom(List<VertexPositionNormalTexture> b, Block block) {
            b.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Up, block.Source));
            b.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Up, block.Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
            b.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Up, block.Source + new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));
            b.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Up, block.Source + new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));
            b.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0), Vector3.Up, block.Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
            b.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Up, block.Source));
        }

        internal void AddBlockGeom(List<VertexPositionNormalTexture> b, List<short> c, int i, int j, Block block) {
            var h = block.Data.Height;
            var l = (short)(c.Count/6*4);
            if (j == Ry - 1 || !Blocks[i * Rx + j + 1].Data.Wallmaker || Blocks[(i) * Rx + j + 1].Data.Height < 32)
            {
                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i, j + 1, h), Vector3.Backward,
                          block.Source));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j + 1, h), Vector3.Backward,
                        block.Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i, j + 1, 0), Vector3.Backward,
                        block.Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j + 1, 0), Vector3.Backward,
                        block.Source +
                        new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));

                c.Add((short) (l + 0));
                c.Add((short) (l + 1));
                c.Add((short) (l + 2));
                c.Add((short) (l + 3));
                c.Add((short) (l + 2));
                c.Add((short) (l + 1));
                l += 4;
            }

            if (j == 0 || !Blocks[(i) * Rx + j - 1].Data.Wallmaker || Blocks[(i ) * Rx + j - 1].Data.Height < 32)
            {
                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i, j, h), Vector3.Right,
                          block.Source));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i, j, 0), Vector3.Right,
                        block.Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j, h), Vector3.Right,
                        block.Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j, 0), Vector3.Right,
                        block.Source +
                        new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));

                c.Add((short) (l + 0));
                c.Add((short) (l + 1));
                c.Add((short) (l + 2));
                c.Add((short) (l + 2));
                c.Add((short) (l + 1));
                c.Add((short) (l + 3));
                l += 4;
            }

            if (i == 0 || !Blocks[(i - 1) * Rx + j].Data.Wallmaker || Blocks[(i - 1) * Rx + j].Data.Height < 32)
            {
                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i, j, h), Vector3.Forward,
                          block.Source));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i, j + 1, h), Vector3.Forward,
                        block.Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i, j, 0), Vector3.Forward,
                        block.Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i, j + 1, 0), Vector3.Forward,
                        block.Source +
                        new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));

                c.Add((short)(l + 0));
                c.Add((short)(l + 1));
                c.Add((short)(l + 2));
                c.Add((short)(l + 1));
                c.Add((short)(l + 3));
                c.Add((short)(l + 2));
                l += 4;
            }

            if (i == Rx - 1 || !Blocks[(i + 1) * Rx + j].Data.Wallmaker || Blocks[(i + 1) * Rx + j].Data.Height < 32)
            {
                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i + 1, j, h), Vector3.Left,
                          block.Source));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j, 0), Vector3.Left,
                        block.Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j + 1, h), Vector3.Left,
                        block.Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1, j + 1, 0), Vector3.Left,
                        block.Source +
                        new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));

                c.Add((short)(l + 0));
                c.Add((short)(l + 1));
                c.Add((short)(l + 2));
                c.Add((short)(l + 2));
                c.Add((short)(l + 1));
                c.Add((short)(l + 3));
                l += 4;
            }


            b.Add(new VertexPositionNormalTexture(
                      new Vector3(i, j, h), Vector3.Up,
                      block.Source));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i + 1, j, h), Vector3.Up,
                    block.Source + new Vector2(Atlases.Instance.SpriteWidth, 0)));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i, j + 1, h), Vector3.Up,
                    block.Source + new Vector2(0, Atlases.Instance.SpriteHeight)));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i + 1, j + 1, h), Vector3.Up,
                    block.Source +
                    new Vector2(Atlases.Instance.SpriteWidth, Atlases.Instance.SpriteHeight)));

            c.Add((short) (l + 0));
            c.Add((short) (l + 1));
            c.Add((short) (l + 2));
            c.Add((short) (l + 1));
            c.Add((short) (l + 3));
            c.Add((short) (l + 2));
            l += 4;
        }
    }
}