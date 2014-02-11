using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib {
    public static class Settings {
        private static Vector2 _resolution;

        public static bool DebugInfo;
        public static bool DebugWire;
        public static bool Lighting = true;

        public static bool NeedToShowInfoWindow;
        public static string NTS1, NTS2;

        public static Random rnd = new Random();
        public static int MegaSectorSize = 15;

        public static Rectangle KillArea;

        public static Vector2 FloorSpriteSize = new Vector2(32, 32);
        public static Vector2 Sqeezer = new Vector2(1, 1);
        public static bool GamePause;

        private static Color hcol_ = Color.LightGray;
        public static bool IsAMDM;
        public static SpriteFont Font;
        public static bool NeedExit;
        public static bool InventoryUpdate;
        public static bool SeeAll = false;
        public static bool Noclip;
        public static int DecalCount = 256;

        public static Vector2 Resolution {
            set {
                _resolution = value;
                KillArea = new Rectangle(-50, -50, (int) _resolution.X + 50, (int) _resolution.Y + 50);
                Sqeezer = new Vector2(_resolution.X/1080, _resolution.Y/1920);
            }
            get { return _resolution; }
        }

        public static Color HudСolor {
            get { return hcol_; }
            set { hcol_ = value; }
        }

        public static bool Server;
        public static bool Normalwalk = true;

        public static int GetFloorAtlasWidth() {
            return 10;
        }

        public static float G() {
            return 9.8f;
        }

        public static float H() {
            return 9.8f;
        }

        public static string GetDataDirectory() {
            return @"Content\Data";
        }

        public static string GetTextureDirectory() {
            return @"Content\Textures";
        }

        public static string GetFloorDataDirectory() {
            return GetDataDirectory() + @"\Floor";
        }

        public static string GetObjectDataDirectory() {
            return GetDataDirectory() + @"\Blocks";
        }

        public static string GetItemDataDirectory() {
            return GetDataDirectory() + @"\Items";
        }

        public static float GetMeeleActionRange() {
            return 64;
        }

        public static string GetWorldsDirectory() {
            return @"Worlds\";
        }

        public static void ChangeResolution(Vector2 res) {
            Resolution = res;
        }

        public static string GetParticleTextureDirectory() {
            return GetTextureDirectory() + @"\Particles";
        }

        public static string GetUnitTextureDirectory() {
            return GetTextureDirectory() + @"\Units";
        }

        public static string GetCreatureDataDirectory() {
            return GetDataDirectory() + @"\Units";
        }

        public static string GetDressTexturesDirectory() {
            return GetUnitTextureDirectory() + @"\Dress";
        }

        public static string GetMinimapTexturesDirectory() {
            return GetTextureDirectory() + @"\Minimap";
        }

        public static string GetFloorTextureDirectory() {
            return GetTextureDirectory() + @"\Floor";
        }

        public static string GetObjectTextureDirectory() {
            return GetTextureDirectory() + @"\Blocks";
        }

        public static string GetEffectDataDirectory() {
            return GetDataDirectory() + @"\Effects";
        }

        public static string GetDialogDataDirectory() {
            return GetDataDirectory() + @"\Dialogs";
        }

        public static string GetNamesDataDirectory() {
            return GetDataDirectory() + @"\Names";
        }

        public static string GetVehicleTextureDirectory() {
            return GetTextureDirectory() + @"\Vehicle";
        }

        public static string GetCraftsDirectory() {
            return GetItemDataDirectory() + @"\Crafts";
        }

        public static string GetSpawnlistsDataDirectory() {
            return GetItemDataDirectory() + @"\Spawnlists";
        }
    }
}