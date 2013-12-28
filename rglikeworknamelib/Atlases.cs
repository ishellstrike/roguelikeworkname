using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib {
    public class Atlases {
        public Dictionary<string, Texture2D> FloorArray;
        public Dictionary<string, int> FloorIndexes;


        public Dictionary<string, Texture2D> BlockArray;
        public Dictionary<string, int> BlockIndexes;


        public Dictionary<string, Texture2D> CreatureArray;

        public Dictionary<string, Texture2D> VehicleArray;
        public Dictionary<string, int> VehicleIndexes;

        public Texture2D MajorAtlas;
        public Dictionary<string, int> MajorIndexes;

        public ContentManager Content;
        public Collection<Texture2D> ParticleAtlas;
        public Dictionary<string, Texture2D> DressAtlas;
        public Dictionary<string, Texture2D> MinimapAtlas;
        public Collection<Texture2D> NormalAtlas;
        private SpriteBatch sb_;

        public static Atlases Instance;
        public int MajorCount {
            get { return MajorIndexes.Count; }
        }

        public int MajorCapacity {
            get { return MajorAtlas.Width*4096/1024; /* 1024 -- square of sprite*/ }
        }

        public Atlases(ContentManager c, GraphicsDevice gd) {
            Content = c;
            sb_ = new SpriteBatch(gd);
            Load(gd);

            Instance = this;
        }

        private void Load(GraphicsDevice gd) {           
            ParticleAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"\textureloadorder.ord",
                                                Content);
            DressAtlas = ParsersCore.LoadTexturesDirectory(Settings.GetDressTexturesDirectory(), Content);
            MinimapAtlas = ParsersCore.LoadTexturesTagged(Settings.GetMinimapTexturesDirectory() + @"\textureloadorder.ord",
                                               Content);

            
            RebuildAtlases(gd);
        }

        public void RebuildAtlases(GraphicsDevice gd) {
            CreatureArray = ParsersCore.LoadTexturesDirectory(Settings.GetUnitTextureDirectory(), Content);
            FloorArray = ParsersCore.LoadTexturesDirectory(Settings.GetFloorTextureDirectory(), Content);
            BlockArray = ParsersCore.LoadTexturesDirectory(Settings.GetObjectTextureDirectory(), Content);
            VehicleArray = ParsersCore.LoadTexturesTagged(Settings.GetVehicleTextureDirectory() + @"\textureloadorder.ord", Content);

            MajorAtlas = GenerateMajorAtlas(gd, out MajorIndexes, BlockArray, FloorArray, VehicleArray, CreatureArray);

            CreatureArray.Clear();
            FloorArray.Clear();
            VehicleArray.Clear();
        }

        private Texture2D GenerateMajorAtlas(GraphicsDevice gd, out Dictionary<string, int> indexes, params Dictionary<string, Texture2D>[] texes) {
            indexes = new Dictionary<string, int>();
            var totalcount = texes.Sum(dictionary => dictionary.Count);
            var atl = new RenderTarget2D(gd, 1024, (totalcount / 32 + 1) * 32);
            gd.SetRenderTarget(atl);
            gd.Clear(Color.Transparent);
            sb_.Begin();
            int i = 0;
            foreach (var dictionary in texes) {
                foreach (var tex in dictionary) {
                    sb_.Draw(tex.Value, new Vector2(i % 32 * 32, i / 32 * 32), Color.White);
                    indexes.Add(tex.Key, i);
                    i++;
                }
                
            }
            sb_.End();
            gd.SetRenderTarget(null);
            return atl;
        }
    }
}