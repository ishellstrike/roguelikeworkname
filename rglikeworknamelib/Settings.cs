using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib {
    public static class Settings {
        public static int GetFloorAtlasWidth() {
            return 10;
        }

        private static Vector2 _resolution;
        public static int Framelimit;

        public static bool DebugInfo;

        public static Random rnd = new Random();

        public static Rectangle KillArea;

        public static float G() {
            return 9.8f;
        }

        public static float H() {
            return 9.8f;
        }

        public static string GetDataDirectory() {
            return @"Content/Data";
        }

        public static string GetTextureDirectory() {
            return @"Content/Textures";
        }

        public static string GetFloorDataDirectory() {
            return GetDataDirectory() + @"/Floor";
        }

        public static string GetObjectDataDirectory()
        {
            return GetDataDirectory() + @"/Object";
        }

        public static string GetItemDataDirectory()
        {
            return GetDataDirectory() + @"/Items";
        }

        public static float GetMeeleActionRange() {
            return 64;
        }

        public static Vector2 FloorSpriteSize = new Vector2(32, 32);
        public static Vector2 Sqeezer = new Vector2(1, 1);
        public static string GetWorldsDirectory() {
            return @"Worlds/";
        }
    

        public static Vector2 Resolution {
            set {
                _resolution = value;
                KillArea = new Rectangle(-50, -50, (int) _resolution.X + 50, (int) _resolution.Y + 50);
                Sqeezer = new Vector2(_resolution.X / 1080, _resolution.Y / 1920);
            }
            get { return _resolution; }
        }

        public static void ChangeResolution(Vector2 res) {
            Resolution = res;
        }

        private static Color hcol_ = Color.LightGray;
        public static bool IsAMDM;
        public static SpriteFont Font;

        public static Color HudСolor {
            get { return hcol_; }
            set { hcol_ = value; }
        }

        public static string GetParticleTextureDirectory() {
            return GetTextureDirectory() + @"/Particles";
        }

        public static string GetUnitTextureDirectory() {
            return GetTextureDirectory() + @"/Units";
        }

        public static string GetCreatureDataDirectory() {
            return GetDataDirectory() + @"/Units";
        }
    }
}