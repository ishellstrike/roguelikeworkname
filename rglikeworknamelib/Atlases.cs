using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib
{
    public class Atlases {
        public static Dictionary<string, Texture2D> FloorAtlas;
        public static Dictionary<string, Texture2D> BlockAtlas, CreatureAtlas; 
        public static ContentManager Content;
        public static Collection<Texture2D> ParticleAtlas;
        public static Dictionary<string, Texture2D> DressAtlas;
        public static Dictionary<string, Texture2D> MinimapAtlas;

        public Atlases(ContentManager c) {
            Content = c;
            Load();
        }

        private void Load() {
            FloorAtlas = ParsersCore.LoadTexturesTagged(Settings.GetFloorTextureDirectory() + @"/textureloadorder.ord", Content);
            BlockAtlas = ParsersCore.LoadTexturesTagged(Settings.GetObjectTextureDirectory() + @"/textureloadorder.ord", Content);
            CreatureAtlas = ParsersCore.LoadTexturesTagged(Settings.GetUnitTextureDirectory() + @"/textureloadorder.ord", Content);
            ParticleAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content);
            DressAtlas = ParsersCore.LoadTexturesTagged(Settings.GetDressTexturesDirectory() + @"/textureloadorder.ord", Content);
            MinimapAtlas = ParsersCore.LoadTexturesTagged(Settings.GetMinimapTexturesDirectory() + @"/textureloadorder.ord", Content);
        }
    }
}
