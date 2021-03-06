﻿using System.Collections.Generic;
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
        public Dictionary<string, ushort> MajorIndexes;
        public Dictionary<ushort, string> MajorIndexesReverse;

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

        public Atlases(GraphicsDevice gd) {
            sb_ = new SpriteBatch(gd);
            Load(gd);

            Instance = this;
        }

        private void Load(GraphicsDevice gd) {           
            ParticleAtlas = ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"\textureloadorder.ord");
            DressAtlas = ParsersCore.LoadTexturesDirectory(Settings.GetDressTexturesDirectory());
            MinimapAtlas = ParsersCore.LoadTexturesTagged(Settings.GetMinimapTexturesDirectory() + @"\textureloadorder.ord");

            
            RebuildAtlases(gd);
        }

        public void RebuildAtlases(GraphicsDevice gd) {
            CreatureArray = ParsersCore.LoadTexturesDirectory(Settings.GetUnitTextureDirectory());
            FloorArray = ParsersCore.LoadTexturesDirectory(Settings.GetFloorTextureDirectory());
            BlockArray = ParsersCore.LoadTexturesDirectory(Settings.GetObjectTextureDirectory());
            VehicleArray = ParsersCore.LoadTexturesTagged(Settings.GetVehicleTextureDirectory() + @"\textureloadorder.ord");

            MajorAtlas = GenerateMajorAtlas(gd, out MajorIndexes, out MajorIndexesReverse, BlockArray, FloorArray, VehicleArray, CreatureArray);
            SpriteWidth = 32f / MajorAtlas.Width;
            SpriteHeight = 32f / MajorAtlas.Height;

            CreatureArray.Clear();
            VehicleArray.Clear();
        }

        public float SpriteWidth { get; private set; }
        public float SpriteHeight { get; private set; }


        public static Vector2 GetSource(string s)
        {
            //server notex
            if (Instance == null)
            {
                return new Vector2(0, 0);
            }
            int index = Instance.MajorIndexes[s];
            // ReSharper disable PossibleLossOfFraction
            return new Vector2((index % 32 * 32f) / Instance.MajorAtlas.Width, (index / 32 * 32f) / Instance.MajorAtlas.Height);
            // ReSharper restore PossibleLossOfFraction
        }

        private Texture2D GenerateMajorAtlas(GraphicsDevice gd, out Dictionary<string, ushort> indexes, out Dictionary<ushort, string> reverse, params Dictionary<string, Texture2D>[] texes)
        {
            indexes = new Dictionary<string, ushort>();
            reverse = new Dictionary<ushort, string>();
            var totalcount = texes.Sum(dictionary => dictionary.Count);
            var atl = new RenderTarget2D(gd, 1024, (totalcount / 32 + 1) * 32);
            Texture2D wp = new Texture2D(gd, 32, 32);
            Color[] c = new Color[32*32];
            for (int j = 0; j < 32*32; j++) {
                c[j] = Color.Black;
            }
            wp.SetData(c);

            gd.SetRenderTarget(atl);
            gd.Clear(Color.Transparent);
            sb_.Begin();
            int i = 0;
            foreach (var dictionary in texes) {
                foreach (var tex in dictionary) {
                    sb_.Draw(tex.Value, new Vector2(i % 32 * 32, i / 32 * 32), Color.White);
                    indexes.Add(tex.Key, (ushort) i);
                    reverse.Add((ushort) i, tex.Key);
                    i++;
                }
            }
            sb_.End();
            gd.SetRenderTarget(null);
            return atl;
        }
    }
}