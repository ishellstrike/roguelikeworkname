using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
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
        public const int Rx = 16;

        /// <summary>
        ///     Sectro oy size
        /// </summary>
        public const int Ry = 16;

        internal SectorBiom Biom;

        internal List<Block> Blocks;
        internal List<Creature> Creatures;
        internal List<Particle> Decals;
        internal Floor[] Floors;

        internal List<Light> Lights;
        public GameLevel Parent;
        public bool Ready;
        public int SectorOffsetX, SectorOffsetY;
        internal List<Vehicle> Vehicles;

        public MapSector(int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;

            Blocks = new List<Block>(Rx*Ry);
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

            Blocks = new List<Block>(Rx*Ry);
            Floors = new Floor[Rx*Ry];
            Creatures = new List<Creature>();
            Decals = new List<Particle>();
            Vehicles = new List<Vehicle>();

            int i = Rx*Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks.Add(new Block());
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

            Blocks = blocksArray as List<Block>;
            Floors = floorsArray as Floor[];
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

        public void ResetLightingSources() {
            Lights.Clear();
            //int i = 0;
            //for (int index = 0; index < Blocks.Count; index++) {
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
                for (int i = 0; i < 3; i++) {
                    Spawn("rabbit", rand);
                }
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
            Creatures.Add(CreatureFactory.GetInstance(creatureId, new Vector2(x * 32, y * 32)));
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

        public void OpenCloseDoor(int x, int y) {
            if (Blocks[x*Rx + y].Data.SmartAction == SmartAction.ActionOpenClose) {
                SetBlock(x, y, Blocks[x*Ry + y].Data.AfterDeathId);
            }
        }

        public void CreateAllMapFromArray(string[] arr) {
            for (int i = 0; i < Rx; i++) {
                for (int j = 0; j < Ry; j++) {
                    SetBlock(i, j, arr[i]);
                }
            }
        }

        public void AddDecal(Particle particle) {
            Decals.Add(particle);
            if (Decals.Count > Settings.DecalCount) {
                Decals.RemoveAt(0);
                Decals.RemoveAt(0);
            }
        }
    }
}