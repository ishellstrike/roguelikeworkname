using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib
{
    public class Atlases {
        public static Dictionary<string, Texture2D> FloorArray;
        public static Texture2D FloorAtlas;
        public static Dictionary<string, int> FloorIndexes;
        public static Dictionary<string, Texture2D> BlockArray;
        public static Texture2D BlockAtlas;
        public static Dictionary<string, int> BlockIndexes;
        public static Dictionary<string, Texture2D> CreatureAtlas;
        public static ContentManager Content;
        public static Collection<Texture2D> ParticleAtlas;
        public static Dictionary<string, Texture2D> DressAtlas;
        public static Dictionary<string, Texture2D> MinimapAtlas;
        public static Collection<Texture2D> NormalAtlas;
        private static SpriteBatch sb;

        public Atlases(ContentManager c, GraphicsDevice gd) {
            Content = c;
            sb = new SpriteBatch(gd);
            Load(gd);
        }

        private void Load(GraphicsDevice gd) {
            FloorArray = ParsersCore.LoadTexturesTagged(Settings.GetFloorTextureDirectory() + @"\textureloadorder.ord", Content);
            BlockArray = ParsersCore.LoadTexturesTagged(Settings.GetObjectTextureDirectory() + @"\textureloadorder.ord", Content);
            RebuildAtlases(gd);
           // BlockArray.Clear();
            CreatureAtlas = ParsersCore.LoadTexturesTagged(Settings.GetUnitTextureDirectory() + @"\textureloadorder.ord", Content);
            ParticleAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"\textureloadorder.ord", Content);
            DressAtlas = ParsersCore.LoadTexturesTagged(Settings.GetDressTexturesDirectory() + @"\textureloadorder.ord", Content);
            MinimapAtlas = ParsersCore.LoadTexturesTagged(Settings.GetMinimapTexturesDirectory() + @"\textureloadorder.ord", Content);

            NormalAtlas = new Collection<Texture2D>();
            NormalAtlas.Add(Content.Load<Texture2D>(@"Textures/Dungeon/Normals/bricks"));
        }

        public static void RebuildAtlases(GraphicsDevice gd) {
            FloorIndexes = new Dictionary<string, int>();
            FloorAtlas = GenerateAtlas(gd, FloorArray, ref FloorIndexes);
            BlockIndexes = new Dictionary<string, int>();
            BlockAtlas = GenerateAtlas(gd, BlockArray, ref BlockIndexes);
        }

        private static Texture2D GenerateAtlas(GraphicsDevice gd, Dictionary<string, Texture2D> texes, ref Dictionary<string, int> indexes) {
            RenderTarget2D atl = new RenderTarget2D(gd, 1024, (texes.Count / 32 + 1)*32);
            gd.SetRenderTarget(atl);
            gd.Clear(Color.Transparent);
            sb.Begin();
            int i = 0;
                foreach (var tex in texes) {
                    sb.Draw(tex.Value, new Vector2(i%32*32, i/32*32), Color.White);
                    indexes.Add(tex.Key, i);
                    i++;
                }
            sb.End();
            gd.SetRenderTarget(null);
            return atl;
        }
    }
}
