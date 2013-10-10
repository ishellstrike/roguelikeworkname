using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mork;
using NLog;
using rglikeworknamelib;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dialogs;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Generation.Names;
using rglikeworknamelib.Parser;
using rglikeworknamelib.Window;
using Button = rglikeworknamelib.Window.Button;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using EventLog = rglikeworknamelib.EventLog;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Label = rglikeworknamelib.Window.Label;
using ProgressBar = rglikeworknamelib.Window.ProgressBar;
using Settings = rglikeworknamelib.Settings;
using WMPLib;

namespace jarg {
    public partial class JargMain : Game {
        private WindowSystem ws_;
        private ParticleSystem ps_;
        private BulletSystem bs_;
        private GameLevel currentFloor_;
        private InventorySystem inventory_;
        private LevelWorker levelWorker_;

        private Player player_ = null;

        private GraphicsDeviceManager graphics_;
        private Vector2 camera_;
        private SpriteFont font1_;
        private KeyboardState ks_;
        private LineBatch lineBatch_;
        private KeyboardState lks_;
        private MouseState lms_;
        private MouseState ms_;
        private Vector2 pivotpoint_;
        private SpriteBatch spriteBatch_;
        private Effect lig1;
        private Effect EffectOmnilight;
        private Effect lig3;
        private Effect lig4;
        private Texture2D arup, ardown, gear, bag, map, caracter;
        private Color lwstatus_color = new Color(1, 1, 1, 0.2f);

        private RenderTarget2D colorMapRenderTarget_;
        private RenderTarget2D depthMapRenderTarget_;
        private RenderTarget2D normalMapRenderTarget_;
        private RenderTarget2D shadowMapRenderTarget_;
        private Texture2D shadowMapTexture_;
        private VertexPositionTexture[] vertices_;
        private Effect lightEffect1_;
        private Effect lightEffect2_;

        private Achievements achievements_;

        private Texture2D whitepixel_;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public JargMain() {
#if DEBUG

            Process myProcess = new Process {
                                                StartInfo = {
                                                                FileName = "cmd.exe",
                                                                Arguments =
                                                                    @"/C cd " + Application.StartupPath +
                                                                    " & VersionGetter.cmd",
                                                                WindowStyle = ProcessWindowStyle.Hidden,
                                                                CreateNoWindow = true
                                                            }
                                            };
            myProcess.Start();
            myProcess.WaitForExit(3000);
            myProcess.Close();
            myProcess.Dispose();
#endif
            Application.ApplicationExit += Application_ApplicationExit;

            if (File.Exists("JARGLog_previous.txt")) {
                File.Delete("JARGLog_previous.txt");
            }
            if (File.Exists("JARGLog.txt")) {
                File.Move("JARGLog.txt", "JARGLog_previous.txt");
            }

            Version.Init();

            graphics_ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            if (!Directory.Exists(Settings.GetWorldsDirectory())) {
                Directory.CreateDirectory(Settings.GetWorldsDirectory());
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e) {
            if (currentFloor_.SectorCount() > 0) {
                currentFloor_.SaveAllAndExit();
            }
        }

        private static Form gameWindowForm_;
        private WindowsMediaPlayer WMPs;

        protected override void Initialize() {
            var gameWindowForm = (Form) Control.FromHandle(Window.Handle);
            gameWindowForm.MinimumSize = new System.Drawing.Size(800, 600);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            Settings.Resolution = new Vector2(1024, 768);
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferHeight = (int) Settings.Resolution.Y;
            graphics_.PreferredBackBufferWidth = (int) Settings.Resolution.X;
            graphics_.SynchronizeWithVerticalRetrace = false;
            InactiveSleepTime = TimeSpan.FromMilliseconds(500);

            IsFixedTimeStep = true;
            IsMouseVisible = true;
            graphics_.ApplyChanges();
            UpdateTitle();

            base.Initialize();
        }

        private void RunRadioGhostBox() {
            WMPs = new WindowsMediaPlayer(); //создаётся плеер 
            WMPs.settings.volume = 20;
            WMPs.URL = "http://208.43.42.26:8086";
            WMPs.controls.play();
            WMPs.MediaChange += WMPs_MediaChange;
        }

        private void WMPs_MediaChange(object item) {
            var a = item as IWMPMedia;
            if (a != null) {
                LabelRadio.Text = " --- " + a.name + " --- from " + a.sourceURL;
                LabelRadio.SetPosition(new Vector2(0, 8));
                WindowRadio.CenterComponentHor(LabelRadio);
            }
        }

        private void UpdateTitle() {
            Window.Title = Version.GetLong() + string.Format(" - {0}x{1}", Settings.Resolution.X, Settings.Resolution.Y);
        }

        private bool needChangeSesolution_;

        private void Window_ClientSizeChanged(object sender, EventArgs e) {
            Settings.Resolution = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
            needChangeSesolution_ = true;
        }

        private void ResolutionChanging() {
            var height = (int) Settings.Resolution.Y;
            var width = (int) Settings.Resolution.X;

            graphics_.PreferredBackBufferHeight = height;
            graphics_.PreferredBackBufferWidth = width;
            graphics_.ApplyChanges();
            var t = ws_.GetVisibleList();
            ws_.Clear();
            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(ws_);
            ws_.SetVisibleList(t);
            UpdateTitle();
            UpdateInventoryContainer();
            UpdateCaracterWindowItems(null, null);
            EventLog_onLogUpdate(null, null);
            colorMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                       DepthFormat.Depth24Stencil8);
            depthMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                       DepthFormat.Depth24Stencil8);
            normalMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                        DepthFormat.Depth24Stencil8);
            shadowMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                        DepthFormat.Depth24Stencil8);
            EffectOmnilight.Parameters["screenWidth"].SetValue(width);
            EffectOmnilight.Parameters["screenHeight"].SetValue(height);
            lineBatch_.UpdateProjection(GraphicsDevice);
            Atlases.RebuildAtlases(GraphicsDevice);
        }

        protected override void OnActivated(object sender, EventArgs args) {
            //IsFixedTimeStep = Settings.Framelimit;
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args) {
            base.OnDeactivated(sender, args);
        }

        protected override void LoadContent() {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            Atlases a = new Atlases(Content, GraphicsDevice);

            whitepixel_ = new Texture2D(graphics_.GraphicsDevice, 1, 1);
            var data = new uint[1];
            data[0] = 0xffffffff;
            whitepixel_.SetData(data);

            font1_ = Content.Load<SpriteFont>(@"Fonts/Font1");
            Settings.Font = font1_;

            lig1 = Content.Load<Effect>(@"Effects/Lighting1");
            EffectOmnilight = Content.Load<Effect>(@"Effects/Effect1");
            lig3 = Content.Load<Effect>(@"Effects/Effect11");
            lig4 = Content.Load<Effect>(@"Effects/Effect111");

            arup = Content.Load<Texture2D>(@"Textures/arrow_up");
            ardown = Content.Load<Texture2D>(@"Textures/arrow_down");
            gear = Content.Load<Texture2D>(@"Textures/gear");
            bag = Content.Load<Texture2D>(@"Textures/bag");
            caracter = Content.Load<Texture2D>(@"Textures/caracter");
            map = Content.Load<Texture2D>(@"Textures/map");

            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_);

            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(ws_);

            Action dbl = DataBasesLoadAndThenInitialGeneration;
            dbl.BeginInvoke(null, null);

            achievements_ = new Achievements();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            colorMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                       DepthFormat.Depth24Stencil8);
            depthMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                       DepthFormat.Depth24Stencil8);
            normalMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                        DepthFormat.Depth24Stencil8);
            shadowMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, true, SurfaceFormat.Color,
                                                        DepthFormat.Depth24Stencil8);

            lightEffect1_ = Content.Load<Effect>(@"Effects/ShadersLightningShadow");
            lightEffect2_ = Content.Load<Effect>(@"Effects/ShadersLightningCombined");

            vertices_ = new VertexPositionTexture[4];
            vertices_[0] = new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0));
            vertices_[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
            vertices_[2] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1));
            vertices_[3] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1));

            lightCollection_ = new List<Light>();
            lightCollection_.Add(new Light
                                {Color = Color.Brown, LightRadius = 2000f, Position = Vector3.Zero, Power = 100});
        }

        private void InitialGeneration() {

            RunRadioGhostBox();
            WMPs.controls.stop();
            WindowRadio.Visible = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            ShowInfoWindow("Initial map generation", "");
            currentFloor_ = new GameLevel(spriteBatch_, font1_, GraphicsDevice, levelWorker_);
            levelWorker_ = new LevelWorker();
            Action lw_ = levelWorker_.Run;
            lw_.BeginInvoke(null, null);
            currentFloor_.lw_ = levelWorker_;
            ps_ = new ParticleSystem(spriteBatch_,
                                     ParsersCore.LoadTexturesInOrder(
                                         Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content));
            bs_ = new BulletSystem(spriteBatch_,
                                   ParsersCore.LoadTexturesInOrder(
                                       Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content),
                                   currentFloor_, font1_, lineBatch_);
            inventory_ = new InventorySystem();
            inventory_.AddItem(new Item("testhat", 1));
            inventory_.AddItem(new Item("testhat2", 1));
            inventory_.AddItem(new Item("ak47", 1));
            inventory_.AddItem(new Item("a762", 100));
            inventory_.AddItem(new Item("a762", 100000));
            UpdateInventoryContainer();
            HideInfoWindow();
            sw.Stop();
            logger.Info("Initial generation in {0}", sw.Elapsed);
            player_.OnUpdatedEquip += UpdateCaracterWindowItems;
            player_.OnShoot += player__onShoot;
            RenderedFlashlight = GetRenderedFlashlight();

            currentFloor_.MegaMapPreload();

            var inv = new Action<int, int>(currentFloor_.GenerateMegaSector);
            inv.BeginInvoke(0, 0, null, null);
        }

        private void player__onShoot(object sender, EventArgs e) {
            //shootFlashTS = TimeSpan.Zero;
            //lig2.Parameters["slen"].SetValue(GlobalWorldLogic.GetCurrentSlen()/2);
            //lig2.Parameters["shine"].SetValue(1.5f);
            //shootFlash = true;
        }

        private void DataBasesLoadAndThenInitialGeneration() {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ShowInfoWindow("Bases loading :", "1/8");
            new MonsterDataBase();
            ShowInfoWindow("Bases loading :", "2/8");
            new BlockDataBase();
            ShowInfoWindow("Bases loading :", "3/8");
            new FloorDataBase();
            ShowInfoWindow("Bases loading :", "4/8");
            new ItemDataBase();
            ShowInfoWindow("Bases loading :", "5/8");
            new SchemesDataBase();
            ShowInfoWindow("Bases loading :", "6/8");
            new BuffDataBase();
            ShowInfoWindow("Bases loading :", "7/8");
            new DialogDataBase();
            ShowInfoWindow("Bases loading :", "8/8");
            new NameDataBase();
            HideInfoWindow();
            sw.Stop();
            logger.Info(
                "Total:\n     {1} Monsters\n     {2} Blocks\n     {3} Floors\n     {4} Items\n     {5} Schemes\n     {6} Buffs\n     {7} Dialogs\n     {8} Names\nloaded in {0}",
                sw.Elapsed,
                MonsterDataBase.Data.Count,
                BlockDataBase.Data.Count,
                FloorDataBase.Data.Count,
                ItemDataBase.Data.Count,
                SchemesDataBase.Data.Count,
                BuffDataBase.Data.Count,
                DialogDataBase.data.Count,
                NameDataBase.data.Count);

            Action igen = InitialGeneration;
            igen.BeginInvoke(null, null);
        }

        protected override void UnloadContent() {
            WMPs.close();
            levelWorker_.Stop();
        }

        private Stopwatch sw_update = new Stopwatch();
        private Stopwatch sw_draw = new Stopwatch();
        private Stopwatch sw_shadows = new Stopwatch();

        public static bool ErrorExit;
        public float seeAngleDeg = 60;
        public float PlayerSeeAngle;
        public TimeSpan sec = TimeSpan.Zero;
        public int jji = -2, jjj = -2;
        private Action<GameTime> UpdateAction = x => { };
        private Vector2 player_last_pos;

        protected override void Update(GameTime gameTime) {
            SecondTimespan += gameTime.ElapsedGameTime;
            //first

            if (Settings.DebugInfo) {
                sw_update.Restart();
            } else {sw_update.Stop();}

            if (ErrorExit || Settings.NeedExit) {
                Exit();
            }

            if (Settings.NeedToShowInfoWindow) {
                ShowInfoWindow(Settings.NTS1, Settings.NTS2);
                Settings.NeedToShowInfoWindow = false;
            }

            WindowsUpdate(gameTime);
            ws_.Update(gameTime, ms_, lms_, ks_, lks_, false);

            if (IsActive) {
                UpdateKeyboard(gameTime);
                UpdateMouse(gameTime);
            }

            if (!Settings.GamePause) {
                UpdateAction(gameTime);
            }

            if (Settings.DebugInfo) {
                FrameRateCounter.Update(gameTime);
                sw_update.Stop();
            }

            // last
            if (SecondTimespan.TotalSeconds >= 1) {
                SecondTimespan = TimeSpan.Zero;
                InfoWindow.Visible = false;
            }

            if (needChangeSesolution_) {
                needChangeSesolution_ = false;
                ResolutionChanging();
            }
        }

        private void GameUpdate(GameTime gameTime) {
            sec += gameTime.ElapsedGameTime;

            EffectOmnilight.Parameters["slen"].SetValue(GlobalWorldLogic.GetCurrentSlen());
            var aaa = 1 - GlobalWorldLogic.GetCurrentSlen()/7 + 0.5f;
            aaa = Math.Max(aaa, 1.1f);
            aaa = Math.Max(aaa, 0.8f);
            EffectOmnilight.Parameters["shine"].SetValue(aaa);

            if (sec >= TimeSpan.FromSeconds(0.5)) {
                sec = TimeSpan.Zero;

                currentFloor_.GenerateMinimap(GraphicsDevice, spriteBatch_, player_);
                if (WindowGlobal.Visible) {
                    currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_);
                }
            }

            currentFloor_.CalcWision(player_,
                                     (float)
                                     Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y,
                                                ms_.X - player_.Position.X + camera_.X),
                                     seeAngleDeg);

            var aa = currentFloor_.GetInSectorPosition(player_.GetPositionInBlocks());
            player_.Update(gameTime, currentFloor_.GetSector((int) aa.X, (int) aa.Y), player_);
            currentFloor_.KillFarSectors(player_, gameTime);
            bs_.Update(gameTime);
            currentFloor_.UpdateBlocks(gameTime, camera_);
            GlobalWorldLogic.Update(gameTime);

            currentFloor_.UpdateCreatures(gameTime, player_);

            ps_.Update(gameTime);

            PlayerSeeAngle = (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X);
            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float)gameTime.ElapsedGameTime.TotalSeconds * 4);
            player_last_pos = player_.Position;

            //LightCollection[0].Position = new Vector3(ms_.X+camera_.X,ms_.Y+camera_.Y,10);
        }

        protected Vector2 ContainerOn
        {
            get; set; }


        private Action<GameTime> drawAction_ = x => { };
        private Label labelMainVer_;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            if (Settings.DebugInfo) {
                sw_draw.Restart();
            }

            GraphicsDevice.Clear(Color.Black);

            drawAction_(gameTime);

            ws_.Draw(spriteBatch_, lig1, gameTime);

            lineBatch_.Draw();
            lineBatch_.Clear();

            if (Settings.DebugInfo) {
                sw_draw.Stop();
                DebugInfoDraw();
                FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int)Settings.Resolution.X,
                                  (int)Settings.Resolution.Y, sw_draw, sw_update);
            }
        }

        private bool Flashlight = true;

        private void GameDraw(GameTime gameTime) {
            lightCollection_.Clear();
            lightCollection_.AddRange(currentFloor_.GetLights());
            if (Flashlight) {
                var hpos = new Vector3(player_.Position.X, player_.Position.Y, 0.5f);
                Ray ray = new Ray(hpos, Vector3.Normalize(new Vector3(ms_.X + camera_.X, ms_.Y + camera_.Y, 1) - hpos));
                lightCollection_.Add(new Light {
                                                  Color = Color.White,
                                                  LightRadius = 30*3,
                                                  Position = ray.Position + ray.Direction*40,
                                                  Power = 10
                                              });
                lightCollection_.Add(new Light {
                                                  Color = Color.White,
                                                  LightRadius = 50*3,
                                                  Position = ray.Position + ray.Direction*90,
                                                  Power = 50
                                              });
                lightCollection_.Add(new Light {
                                                  Color = Color.White,
                                                  LightRadius = 80*3,
                                                  Position = ray.Position + ray.Direction*160,
                                                  Power = 50
                                              });
                lightCollection_.Add(new Light {
                                                  Color = Color.White,
                                                  LightRadius = 90*3,
                                                  Position = ray.Position + ray.Direction*280,
                                                  Power = 150
                                              });
            }


            EffectOmnilight.Parameters["cpos"].SetValue(new[]
                                                        {player_.Position.X - camera_.X, player_.Position.Y - camera_.Y});

            GraphicsDevice.SetRenderTarget(colorMapRenderTarget_);
            GraphicsDevice.Clear(Color.Black);
            //color maps
            spriteBatch_.Begin();
                currentFloor_.DrawFloors(gameTime, camera_);
                currentFloor_.DrawDecals(gameTime, camera_);
            spriteBatch_.End();
            currentFloor_.ShadowRender();
            spriteBatch_.Begin();
                currentFloor_.DrawBlocks(gameTime, camera_, player_);
                currentFloor_.DrawCreatures(gameTime, camera_);
                player_.Draw(gameTime, camera_);
                bs_.Draw(gameTime, camera_);
                ps_.Draw(gameTime, camera_);
            spriteBatch_.End();


            if (Settings.Lighting) {
                //GraphicsDevice.SetRenderTarget(normalMapRenderTarget_);
                //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);
                //normal maps
                //spriteBatch_.Begin();
                //for (int i = 0; i < Settings.Resolution.X / 32 + 1; i++)
                //{
                //    for (int j = 0; j < Settings.Resolution.Y / 32 + 1; j++)
                //    {
                //        spriteBatch_.Draw(Atlases.NormalAtlas[0],
                //                          new Vector2(i*32 - camera_.X%32 - 32, j*32 - camera_.Y%32 - 32), Color.White);
                //    }
                //}
                //spriteBatch_.End();

                //GraphicsDevice.SetRenderTarget(depthMapRenderTarget_);
                //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1, 0);
                //depth maps
                //spriteBatch_.Begin();
                //    currentFloor_.DrawFloorsInnerDepth(gameTime, camera_);
                //spriteBatch_.End();

                GraphicsDevice.SetRenderTarget(null);
                shadowMapTexture_ = GenerateShadowMap();

                GraphicsDevice.SetRenderTarget(null);
                DrawCombinedMaps();
            }
            else {
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch_.Begin();
                spriteBatch_.Draw(colorMapRenderTarget_, Vector2.Zero, Color.White);
                spriteBatch_.End();
            }

            spriteBatch_.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            if (levelWorker_.Generating())
            {
                spriteBatch_.Draw(gear, new Vector2(10, 10), lwstatus_color);
            }
            if (levelWorker_.Loading())
            {
                spriteBatch_.Draw(arup, new Vector2(42, 10), lwstatus_color);
            }
            if (levelWorker_.Saving())
            {
                spriteBatch_.Draw(ardown, new Vector2(74, 10), lwstatus_color);
            }
            if(Settings.DebugInfo) {
                spriteBatch_.DrawString(font1_, levelWorker_.GenerationCount().ToString(), new Vector2(10, 10), Color.White);
                spriteBatch_.DrawString(font1_, levelWorker_.LoadCount().ToString(), new Vector2(42, 10), Color.White);
                spriteBatch_.DrawString(font1_, levelWorker_.SaveCount().ToString(), new Vector2(74, 10), Color.White);
            }
            spriteBatch_.End();
        }

        private Texture2D RenderedFlashlight;

        private Texture2D GetRenderedFlashlight() {
            return null;
        }

        public void DrawDebugRenderTargets(GameTime time) {
            GameDraw(time);
            // Draw some debug textures
            GraphicsDevice.Clear(Color.DarkGreen);
            spriteBatch_.Begin();

            var size = new Rectangle(0, 0, colorMapRenderTarget_.Width/2, colorMapRenderTarget_.Height/2);
            spriteBatch_.Draw(
                colorMapRenderTarget_,
                new Rectangle(0, 0,
                              size.Width,
                              size.Height),
                Color.White);

            spriteBatch_.Draw(
                depthMapRenderTarget_,
                new Rectangle(size.Width, 0,
                              size.Width,
                              size.Height),
                Color.White);

            spriteBatch_.Draw(
                normalMapRenderTarget_,
                new Rectangle(size.Width, size.Height,
                              size.Width,
                              size.Height),
                Color.White);

            spriteBatch_.Draw(
                shadowMapRenderTarget_,
                new Rectangle(0, size.Height,
                              size.Width,
                              size.Height),
                Color.White);

            spriteBatch_.End();
        }

        private void DrawCombinedMaps() {
            var value = 7/4 - GlobalWorldLogic.GetCurrentSlen()/3 + 0.2f;
            value = Math.Max(value, 0.1f);
            lightEffect2_.Parameters["ambient"].SetValue(value);
            lightEffect2_.Parameters["ambientColor"].SetValue(Color.White.ToVector4());

            // This variable is used to boost to output of the light sources when they are combined
            // I found 4 a good value for my lights but you can also make this dynamic if you want
            lightEffect2_.Parameters["lightAmbient"].SetValue(4);
            lightEffect2_.Parameters["ColorMap"].SetValue(colorMapRenderTarget_);
            lightEffect2_.Parameters["ShadingMap"].SetValue(shadowMapTexture_);
            lightEffect1_.Parameters["DepthMap"].SetValue(depthMapRenderTarget_);

            spriteBatch_.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (var pass in lightEffect2_.CurrentTechnique.Passes) {
                pass.Apply();
                spriteBatch_.Draw(colorMapRenderTarget_, Vector2.Zero, Color.White);
            }
            spriteBatch_.End();
        }

        private List<Light> lightCollection_;

        private Texture2D GenerateShadowMap() {
            //GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.SetRenderTarget(shadowMapRenderTarget_);
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.BlendState = BlendState.Additive;
            //GraphicsDevice.BlendState = new BlendState{AlphaSourceBlend = Blend.Zero, AlphaDestinationBlend = Blend.One};

            // For every light inside the current scene, you can optimize this
            // list to only draw the lights that are visible a.t.m.
            foreach (var light in lightCollection_) {
                lightEffect1_.CurrentTechnique = lightEffect1_.Techniques["DeferredPointLight"];
                lightEffect1_.Parameters["lightStrength"].SetValue(light.Power);
                lightEffect1_.Parameters["lightPosition"].SetValue(light.GetWorldPosition(camera_));
                lightEffect1_.Parameters["lightColor"].SetValue(light.Color.ToVector3());
                lightEffect1_.Parameters["lightRadius"].SetValue(light.LightRadius);

                lightEffect1_.Parameters["screenWidth"].SetValue(GraphicsDevice.Viewport.Width);
                lightEffect1_.Parameters["screenHeight"].SetValue(GraphicsDevice.Viewport.Height);
                lightEffect1_.Parameters["NormalMap"].SetValue(normalMapRenderTarget_);
                lightEffect1_.Parameters["DepthMap"].SetValue(depthMapRenderTarget_);

                foreach (var pass in lightEffect1_.CurrentTechnique.Passes) {
                    pass.Apply();

                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices_, 0, 2);
                }
            }

            return shadowMapRenderTarget_;
        }

        private void DebugInfoDraw() {
            string ss =
                string.Format(
                    "SAng {0} \nPCount {1}   BCount {5}\nDT {3} WorldT {2} \nSectors {4} Generated {6} \nSTri {7} slen {8} {9}\nMH={10} KH={11}",
                    PlayerSeeAngle, ps_.Count(), GlobalWorldLogic.Temperature, GlobalWorldLogic.CurrentTime,
                    currentFloor_.SectorCount(), bs_.GetCount(), currentFloor_.generated,
                    currentFloor_.GetShadowrenderCount()/3, 7/4 - GlobalWorldLogic.GetCurrentSlen()/3,
                    GlobalWorldLogic.DayPart, ws_.Mopusehook, ws_.Keyboardhook);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, ss, new Vector2(500, 10), Color.White);

            int nx = (int) ((ms_.X + camera_.X)/32.0);
            int ny = (int) ((ms_.Y + camera_.Y)/32.0);

            if (ms_.X + camera_.X < 0) {
                nx--;
            }
            if (ms_.Y + camera_.Y < 0) {
                ny--;
            }

            spriteBatch_.DrawString(font1_, string.Format("       {0} {1}", nx, ny), new Vector2(ms_.X, ms_.Y),
                                    Color.White);

            spriteBatch_.End();
        }
    }
}