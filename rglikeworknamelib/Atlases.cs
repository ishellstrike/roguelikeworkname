using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib {
    public class Atlases {
        public static Dictionary<string, Texture2D> FloorArray;
        public static Dictionary<string, int> FloorIndexes;
        public static Texture2D FloorAtlas;


        public static Dictionary<string, Texture2D> BlockArray;
        public static Dictionary<string, int> BlockIndexes;
        public static Texture2D BlockAtlas;


        public static Dictionary<string, Texture2D> CreatureAtlas;

        public static Dictionary<string, Texture2D> VehicleArray;
        public static Dictionary<string, int> VehicleIndexes;
        public static Texture2D VehicleAtlas;

        public static ContentManager Content;
        public static Collection<Texture2D> ParticleAtlas;
        public static Dictionary<string, Texture2D> DressAtlas;
        public static Dictionary<string, Texture2D> MinimapAtlas;
        public static Collection<Texture2D> NormalAtlas;
        private static SpriteBatch sb_;

        public Atlases(ContentManager c, GraphicsDevice gd) {
            Content = c;
            sb_ = new SpriteBatch(gd);
            Load(gd);
        }

        private void Load(GraphicsDevice gd) {
            FloorArray = ParsersCore.LoadTexturesDirectory(Settings.GetFloorTextureDirectory(), Content);
            BlockArray = ParsersCore.LoadTexturesDirectory(Settings.GetObjectTextureDirectory(), Content);
            VehicleArray = ParsersCore.LoadTexturesTagged(Settings.GetVehicleTextureDirectory() + @"\textureloadorder.ord", Content);
            RebuildAtlases(gd);
            // BlockArray.Clear();
            CreatureAtlas = ParsersCore.LoadTexturesDirectory(Settings.GetUnitTextureDirectory(), Content);
            ParticleAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"\textureloadorder.ord",
                                                Content);
            DressAtlas = ParsersCore.LoadTexturesDirectory(Settings.GetDressTexturesDirectory(), Content);
            MinimapAtlas = ParsersCore.LoadTexturesTagged(Settings.GetMinimapTexturesDirectory() + @"\textureloadorder.ord",
                                               Content);

            NormalAtlas = new Collection<Texture2D>();
            NormalAtlas.Add(Content.Load<Texture2D>(@"Textures/Dungeon/Normals/bricks"));
        }

        public static void RebuildAtlases(GraphicsDevice gd) {
            FloorAtlas = GenerateAtlas(gd, FloorArray, out FloorIndexes);
            BlockAtlas = GenerateAtlas(gd, BlockArray, out BlockIndexes);
            VehicleAtlas = GenerateAtlas(gd, VehicleArray, out VehicleIndexes);
        }

        private static Texture2D GenerateAtlas(GraphicsDevice gd, Dictionary<string, Texture2D> texes,
                                               out Dictionary<string, int> indexes) {
            indexes = new Dictionary<string, int>();
            var atl = new RenderTarget2D(gd, 1024, (texes.Count/32 + 1)*32);
            gd.SetRenderTarget(atl);
            gd.Clear(Color.Transparent);
            sb_.Begin();
            int i = 0;
            foreach (var tex in texes) {
                sb_.Draw(tex.Value, new Vector2(i%32*32, i/32*32), Color.White);
                indexes.Add(tex.Key, i);
                i++;
            }
            sb_.End();
            gd.SetRenderTarget(null);
            return atl;
        }
    }
}