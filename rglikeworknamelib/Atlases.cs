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
        public static Collection<Texture2D>  FloorAtlas;
        public static Dictionary<string, Texture2D> BlockAtlas, CreatureAtlas; 
        public static ContentManager Content;
        public static Collection<Texture2D> ParticleAtlas;

        public Atlases(ContentManager c) {
            Content = c;
            Load();
        }

        private void Load() {
            FloorAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetFloorDataDirectory() + @"/textureloadorder.ord", Content);
            BlockAtlas = ParsersCore.LoadTexturesTagged(Settings.GetObjectDataDirectory() + @"/textureloadorder.ord", Content);
            CreatureAtlas = ParsersCore.LoadTexturesTagged(Settings.GetUnitTextureDirectory() + @"/textureloadorder.ord", Content);
            ParticleAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content);
        }
    }
}
