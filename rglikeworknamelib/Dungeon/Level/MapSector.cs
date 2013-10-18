using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Dungeon.Vehicles;
using rglikeworknamelib.Generation;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class MapSector {
        /// <summary>
        /// Sector ox size
        /// </summary>
        public const int Rx = 16;

        /// <summary>
        /// Sectro oy size
        /// </summary>
        public const int Ry = 16;

        public bool Ready;

        public GameLevel Parent;

        public int SectorOffsetX, SectorOffsetY;

        internal List<IBlock> Blocks;
        internal Floor[] Floors;
        internal List<ICreature> Creatures;
        internal List<Particle> Decals;
        internal SectorBiom Biom;
        internal List<Vehicle> Vehicles;

        internal List<Light> Lights; 

        public MapSector(int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;

            Blocks = new List<IBlock>(Rx * Ry);
            Floors = new Floor[Rx * Ry];
            Creatures = new List<ICreature>();

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }
        }

        public MapSector(GameLevel parent, int sectorOffsetX, int sectorOffsetY)
        {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;
            Parent = parent;

            Blocks = new List<IBlock>(Rx * Ry);
            Floors = new Floor[Rx * Ry];
            Creatures = new List<ICreature>();
            Decals = new List<Particle>();
            Vehicles = new List<Vehicle>();

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks.Add(new Block());
            }
            Lights = new List<Light>();
        }

        /// <summary>
        /// Map from just serialized data
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sectorOffsetX"></param>
        /// <param name="sectorOffsetY"></param>
        /// <param name="blocksArray"></param>
        /// <param name="floorsArray"></param>
        /// <param name="obiom"></param>
        /// <param name="creat"></param>
        /// <param name="decal"></param>
        public MapSector(GameLevel parent, object sectorOffsetX, object sectorOffsetY, object blocksArray, object floorsArray, object obiom, object creat, object decal)
        {
            SectorOffsetX = (int)sectorOffsetX;
            SectorOffsetY = (int)sectorOffsetY;
            Parent = parent;

            Blocks = blocksArray as List<IBlock>;
            Floors = floorsArray as Floor[];
            Biom = (SectorBiom)obiom;
            Creatures = (List<ICreature>)creat;
            Decals = (List<Particle>)decal;
            Lights = new List<Light>();
            Vehicles = new List<Vehicle>();
            ResetLightingSources();

            foreach (var block in Blocks) {
                block.OnLoad();
            }
            foreach (var floor in Floors) {
                floor.Source = FloorData.GetSource(floor.MTex);
            }
        }

        public List<StorageBlock> GetStorageBlocks() {
            return (from a in Blocks where a.Id != "0" && a.Data.Prototype == typeof (StorageBlock) select a as StorageBlock).ToList();
        }

        public void ResetLightingSources() {
            Lights.Clear();
            int i = 0;
            for (int index = 0; index < Blocks.Count; index++) {
                var block = Blocks[index];
                if (block is ILightSource) {
                    Lights.Add(GetLights(block, i/Ry*32 + SectorOffsetX*Rx*32, i%Ry*32 + SectorOffsetY*Ry*32));
                }
                i++;
            }
        }

        private Light GetLights(IBlock block, int x, int y) {
            var a1 = block as ILightSource;
            var t = new Light {
                                  Color = a1.LightColor, LightRadius = a1.LightRange, Power = a1.LightPower, Position = new Vector3(x+16, y, 1+32)
                              };
            return t;
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
            a(mapseed);
        }

        /// <summary>
        /// ###---### Main generation proc ###---###
        /// </summary>
        /// <param name="mapseed">Global map seed</param>
        private void AsyncGeneration(int mapseed) {
            int s = (int) (MapGenerators.Noise2D(SectorOffsetX, SectorOffsetY)*int.MaxValue);
            Random rand = new Random(s);

            Biom = GetBiom(SectorOffsetX, SectorOffsetY, Parent);
            if(Biom == SectorBiom.House) {
                Biom = SectorBiom.Field;
            }

            MapGenerators.FillTest1(this, "1");
            MapGenerators.ClearBlocks(this);
            MapGenerators.FloorPerlin(this);

            switch (Biom) {
                case SectorBiom.Bushland:
                    int next = rand.Next(1, 3);
                    for (int i = 0; i < next; i++ ) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "bbochka");
                    break;

                case SectorBiom.Forest:
                    var aa = rand.Next(2, 5);
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;

                case SectorBiom.WildForest:
                    aa = rand.Next(10, 20);
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;

                case SectorBiom.SuperWildForest:
                    aa = rand.Next(30, 40);
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;
            }

            var sb = GetStorageBlocks();

            foreach (var block in sb) {
                int next = rand.Next(0, 3);
                for (int i = 0; i < next; i++) {
                    block.StoredItems.Add(new Items.Item(ItemDataBase.Data.ElementAt(rand.Next(0, ItemDataBase.Data.Count)).Key, rand.Next(1, 2)));
                }
            }
            var rnd1 = rand.Next(0, 5);
            var rnd2 = rand.Next(0, 3);
            for (int i = 1; i< rnd1; i++ ) {
                Spawn("zombie1", rand);
            }
            for (int i = 1; i < rnd2; i++)
            {
                Spawn("hdzombie", rand);
            }

            ResetLightingSources();

            Parent.generated++;
            Ready = true;
        }

        public static SectorBiom GetBiom(int offX, int offY, GameLevel gl) {
            SectorBiom biom;
            int s = (int)(MapGenerators.Noise2D(offX, offY) * int.MaxValue);
            Random rand = new Random(s);
            var most = MapGenerators.GetMost(offX, offY);

            biom = (SectorBiom)rand.Next(0, Enum.GetNames(typeof(SectorBiom)).Length - 3);

            if (most == "1")
            {
                if (biom == SectorBiom.Forest || biom == SectorBiom.WildForest || biom == SectorBiom.SuperWildForest)
                {
                    biom = SectorBiom.Field;
                }
            }

            return biom;
        }

        public void Spawn(string creatureId, Random rnd) {
            Spawn(creatureId, rnd.Next(0, Rx), rnd.Next(0, Ry));
        }

        public void Spawn(string creatureId, int x, int y) {
            var n = (ICreature) Activator.CreateInstance(MonsterDataBase.Data[creatureId].Prototype);
            n.Position = new Vector2(x*32, y*32);
            n.Id = creatureId;
            Creatures.Add(n);
        }

        public IBlock GetBlock(int x, int y)
        {
            return Blocks[x * Ry + y];
        }

        public IBlock GetBlock(int a)
        {
            return Blocks[a];
        }

        public void SetFloor(int x, int y, string floorId)
        {
            Floors[x * Ry + y].Id = floorId;
            Floors[x * Ry + y].Source = FloorDataBase.Data[floorId].RandomMtexFromAlters(ref Floors[x * Ry + y].MTex);
        }

        public string GetId(int x, int y)
        {
            return Blocks[x * Ry + y].Id;
        }

        public string GetId(int oneDimCoord)
        {
            return Blocks[oneDimCoord].Id;
        }

        /// <summary>
        /// Base standart block setter
        /// </summary>
        /// <param name="oneDimCoord"></param>
        /// <param name="id"></param>
        public void SetBlock(int oneDimCoord, string id) {
            Blocks[oneDimCoord] = (IBlock)Activator.CreateInstance(BlockDataBase.Data[id].Prototype);
            var block = Blocks[oneDimCoord];
            block.Id = id;
            block.MTex = block.Data.RandomMtexFromAlters();
        }

        /// <summary>
        /// Base standart block setter
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="id"></param>
        public void SetBlock(int posX, int posY, string id)
        {
            SetBlock(posX * Ry + posY, id);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if (Blocks[x * Rx + y].Data.SmartAction == SmartAction.ActionOpenClose)
            {
                SetBlock(x, y, Blocks[x * Ry + y].Data.AfterDeathId);
            }
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

        public void AddDecal(Particle particle) {
            Decals.Add(particle);
            if(Decals.Count > 256) {
                Decals.RemoveAt(0);
            }
        }
    }
}