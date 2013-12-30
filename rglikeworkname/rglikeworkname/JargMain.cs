using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mork;
using NLog;
using WMPLib;
using rglikeworknamelib;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dialogs;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Generation.Names;
using rglikeworknamelib.Parser;
using rglikeworknamelib.Window;
using Color = Microsoft.Xna.Framework.Color;
using Label = rglikeworknamelib.Window.Label;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Version = rglikeworknamelib.Version;

namespace jarg {
    public partial class JargMain : Game {
        private static readonly Logger Logger = LogManager.GetLogger("JargMain");
        public static bool ErrorExit;
        private readonly GraphicsDeviceManager graphics_;
        private readonly Color lwstatusColor_ = new Color(1, 1, 1, 0.2f);
        private readonly Stopwatch swDraw_ = new Stopwatch();
        private readonly Stopwatch swUpdate_ = new Stopwatch();
        private Effect effectOmnilight_;
        private bool flashlight_ = true;
        public float PlayerSeeAngle;
        private Action<GameTime> updateAction_ = x => { };
        private WindowsMediaPlayer wmPs_;
        private Texture2D ardown_;
        private Texture2D arup_;
        private Texture2D bag_;
        private BulletSystem bs_;
        private Vector2 camera_;
        private Texture2D caracter;
        private RenderTarget2D colorMapRenderTarget_;
        private GameLevel currentFloor_;
        private Action<GameTime> drawAction_ = x => { };
        private SpriteFont font1_;
        private Texture2D gear;
        private InventorySystem inventory_;
        private KeyboardState ks_;
        private Label labelMainVer_;
        private LevelWorker levelWorker_;
        private Effect lig1;
        private List<Light> lightCollection_;
        private Effect lightEffect1_;
        private Effect lightEffect2_;
        private LineBatch lineBatch_;
        private KeyboardState lks_;
        private MouseState lms_;
        private Texture2D map;
        private MouseState ms_;
        private bool needChangeSesolution_;
        private Vector2 pivotpoint_;
        private Player player_;
        private ParticleSystem ps_;
        public TimeSpan sec = TimeSpan.Zero;
        public float seeAngleDeg = 60;
        private RenderTarget2D lightMapRenderTarget_;
        private RenderTarget2D shadowMapRenderTarget_;
        private Texture2D lightMapTexture_;
        private SpriteBatch spriteBatch_;
        private VertexPositionTexture[] vertices_;

        private Texture2D whitepixel_;
        private WindowSystem ws_;

        public JargMain() {
#if DEBUG

            var myProcess = new Process {
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

            if (File.Exists("JARGLog_previous.txt")) {
                File.Delete("JARGLog_previous.txt");
            }
            if (File.Exists("JARGLog.txt")) {
                File.Move("JARGLog.txt", "JARGLog_previous.txt");
            }

            graphics_ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            if (!Directory.Exists(Settings.GetWorldsDirectory())) {
                Directory.CreateDirectory(Settings.GetWorldsDirectory());
            }
        }

        protected Vector2 ContainerOn { get; set; }

        protected override void Initialize() {
            var gameWindowForm = (Form) Control.FromHandle(Window.Handle);
            gameWindowForm.MinimumSize = new Size(800, 600);
            gameWindowForm.FormClosing += gameWindowForm_FormClosing;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            Settings.Resolution = new Vector2(1024, 768);
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferHeight = (int) Settings.Resolution.Y;
            graphics_.PreferredBackBufferWidth = (int) Settings.Resolution.X;
            InactiveSleepTime = TimeSpan.FromMilliseconds(200);

            IsFixedTimeStep = false;
            graphics_.SynchronizeWithVerticalRetrace = true;
            IsMouseVisible = true;
            graphics_.ApplyChanges();
            UpdateTitle();

            base.Initialize();
        }

        private void gameWindowForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !Settings.NeedExit;
            currentFloor_.SaveAllAndExit(player_, inventory_);
        }

        private void RunRadioGhostBox() {
            wmPs_ = new WindowsMediaPlayer();
            wmPs_.settings.volume = 20;
            wmPs_.URL = "http://208.43.42.26:8086";
            wmPs_.controls.play();
            wmPs_.MediaChange += WMPs_MediaChange;
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
            Window.Title = Version.GetLong() + string.Format(" - {0}x{1} S: {2}x{3}", Settings.Resolution.X, Settings.Resolution.Y, (int)(Settings.Resolution.X/LightQ), (int)(Settings.Resolution.Y/LightQ));
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e) {
            Settings.Resolution = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
            needChangeSesolution_ = true;
        }

        private int LightQ = 4;
        private void ResolutionChanging() {
            var height = (int) Settings.Resolution.Y;
            var width = (int) Settings.Resolution.X;

            graphics_.PreferredBackBufferHeight = height;
            graphics_.PreferredBackBufferWidth = width;
            graphics_.ApplyChanges();
            Tuple<int, bool>[] t = ws_.GetVisibleList();
            ws_.Clear();
            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(ws_);
            ws_.SetVisibleList(t);
            UpdateTitle();
            UpdateInventoryContainer();
            UpdateCaracterWindowItems(null, null);
            EventLog_onLogUpdate(null, null);
            colorMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            lightMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width / LightQ, height / LightQ, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            shadowMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            effectOmnilight_.Parameters["screenWidth"].SetValue(width);
            effectOmnilight_.Parameters["screenHeight"].SetValue(height);
            lineBatch_.UpdateProjection(GraphicsDevice);
            Atlases.Instance.RebuildAtlases(GraphicsDevice);
        }

        protected override void OnActivated(object sender, EventArgs args) {
            //IsFixedTimeStep = Settings.Framelimit;
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args) {
            base.OnDeactivated(sender, args);
        }

        private Texture2D fltex;
        protected override void LoadContent() {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            new Atlases(Content, GraphicsDevice);

            whitepixel_ = new Texture2D(graphics_.GraphicsDevice, 1, 1);
            whitepixel_.SetData(new[] {Color.White});

            font1_ = Content.Load<SpriteFont>(@"Fonts/Font1");
            Settings.Font = font1_;

            lig1 = Content.Load<Effect>(@"Effects/Lighting1");
            effectOmnilight_ = Content.Load<Effect>(@"Effects/Effect1");

            arup_ = Content.Load<Texture2D>(@"Textures/arrow_up");
            ardown_ = Content.Load<Texture2D>(@"Textures/arrow_down");
            gear = Content.Load<Texture2D>(@"Textures/gear");
            bag_ = Content.Load<Texture2D>(@"Textures/bag");
            caracter = Content.Load<Texture2D>(@"Textures/caracter");
            map = Content.Load<Texture2D>(@"Textures/map");
            fltex = Content.Load<Texture2D>(@"Textures/Effects/fl1");

            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_);

            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(ws_);

            Action dbl = DataBasesLoadAndThenInitialGeneration;
            dbl.BeginInvoke(null, null);

            new AchievementDataBase();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            colorMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            lightMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width / LightQ, height / LightQ, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            shadowMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);

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

        private bool InitialFinish;
        //invoke after DataBasesLoadAndThenInitialGeneration
        private void InitialGeneration() {
            RunRadioGhostBox();
            wmPs_.controls.stop();
            WindowRadio.Visible = false;

            var sw = new Stopwatch();
            sw.Start();
            ShowInfoWindow("Initial map generation", "");
            currentFloor_ = new GameLevel(spriteBatch_, font1_, GraphicsDevice, levelWorker_);
            if (levelWorker_ != null) {
                levelWorker_.Stop();
            }
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
            player_.Inventory = inventory_;
            inventory_.AddItem(ItemFactory.GetInstance("testhat", 1));
            inventory_.AddItem(ItemFactory.GetInstance("testhat2", 1));
            inventory_.AddItem(ItemFactory.GetInstance("ak47", 1));
            inventory_.AddItem(ItemFactory.GetInstance("a762", 100000));
            inventory_.AddItem(ItemFactory.GetInstance("resp1", 1));
            inventory_.AddItem(ItemFactory.GetInstance("kitty_collar", 1));
            inventory_.AddItem(ItemFactory.GetInstance("radio1", 3));
            inventory_.AddItem(ItemFactory.GetInstance("otvertka", 1));
            inventory_.AddItem(ItemFactory.GetInstance("meatcan1ñl", 1));
            inventory_.AddItem(ItemFactory.GetInstance("meatcan2ñl", 1));
            player_.Weared.Add(ItemFactory.GetInstance("jeans", 1));
            player_.Weared.Add(ItemFactory.GetInstance("t-shirt1", 1));
            player_.Weared.Add(ItemFactory.GetInstance("haer1", 1));

            UpdateInventoryContainer();

            Update_Craft_Items();

            player_.OnUpdatedEquip += UpdateCaracterWindowItems;
            player_.OnShoot += player__onShoot;

            player_.Load();
            inventory_.Load();
            UpdateInventoryContainer();

            HideInfoWindow();
            sw.Stop();
            Logger.Info("Initial generation in {0}", sw.Elapsed);
            InitialFinish = true;
        }

        private void player__onShoot(object sender, EventArgs e) {
            //shootFlashTS = TimeSpan.Zero;
            //lig2.Parameters["slen"].SetValue(GlobalWorldLogic.GetCurrentSlen()/2);
            //lig2.Parameters["shine"].SetValue(1.5f);
            //shootFlash = true;
        }

        private void DataBasesLoadAndThenInitialGeneration() {
            ShowInfoWindow("Loading...", "");
            var sw = new Stopwatch();
            sw.Start();
            new CreatureDataBase();
            new FloorDataBase();
            new BlockDataBase();
            new SchemesDataBase();
            new BuffDataBase();
            new NameDataBase();
            new CraftDataBase();
            sw.Stop();
            Logger.Info(
                "\nTotal:\n     {1} Monsters\n     {2} Blocks\n     {3} Floors\n     {4} Items\n     {5} Schemes\n     {6} Buffs\n     {7} Dialogs\n     {8} Names\n     {9} Crafts\n     loaded in {0}",
                sw.Elapsed,
                CreatureDataBase.Data.Count,
                BlockDataBase.Data.Count,
                FloorDataBase.Data.Count,
                ItemDataBase.Instance.Data.Count,
                SchemesDataBase.Data.Count,
                BuffDataBase.Data.Count,
                DialogDataBase.data.Count,
                NameDataBase.data.Count,
                CraftDataBase.Data.Count);

            sw.Start();
            BasesCheker.CheckAndResolve();
            sw.Stop();
            Logger.Info(string.Format("Check end in {0}", sw.Elapsed));

            SchemeEditorInit();

            Action igen = InitialGeneration;
            igen.BeginInvoke(null, null);
        }

        protected override void UnloadContent() {
            wmPs_.close();
            levelWorker_.Stop();
            if (client_ != null)
            {
                client_.Disconnect();
            }
            client_ = null;
        }

        protected override void Update(GameTime gameTime) {
            bool di = Settings.DebugInfo;
            SecondTimespan += gameTime.ElapsedGameTime;
            //first

            if (di) {
                swUpdate_.Restart();
            }

            if (ErrorExit || Settings.NeedExit) {
                Exit();
            }

            if (Settings.NeedToShowInfoWindow) {
                ShowInfoWindow(Settings.NTS1, Settings.NTS2);
            }
            else {
                InfoWindow.Visible = false;
            }

            if (InitialFinish) {
                WindowsUpdate(gameTime);
                ws_.Update(gameTime, ms_, lms_, ks_, lks_, false);
            }

            if (IsActive) {
                UpdateKeyboard(gameTime);
                UpdateMouse(gameTime);
            }

            if (player_ != null)
            {
                pivotpoint_ = Vector2.Subtract(player_.Position, Vector2.Divide(Settings.Resolution, 2));
            }

            if (!Settings.GamePause) {
                updateAction_(gameTime);
            }

            if (di) {
                FrameRateCounter.Update(gameTime);
                swUpdate_.Stop();
            }

            // last
            if (needChangeSesolution_) {
                needChangeSesolution_ = false;
                ResolutionChanging();
            }
        }

        private Vector2 aa, sectorLast;
        private void GameUpdate(GameTime gameTime) {
            sec += gameTime.ElapsedGameTime;

            sectorLast = aa;
            aa = currentFloor_.GetInSectorPosition(player_.GetPositionInBlocks());

            if ((int)aa.X != (int)sectorLast.X || (int)aa.Y != (int)sectorLast.Y)
            {
                currentFloor_.GenerateMinimap(GraphicsDevice, spriteBatch_, player_);
                if (WindowGlobal.Visible) {
                    currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_, mousemapoffset);
                }
            }

            currentFloor_.CalcWision(player_,
                                     (float)
                                     Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y,
                                                ms_.X - player_.Position.X + camera_.X),
                                     seeAngleDeg);

            
            //if (car != null) {
            //    car.Update(gameTime, player_);
            //}
            player_.Update(gameTime, currentFloor_.GetSector((int) aa.X, (int) aa.Y), player_);
            if (client_ != null) {
                client_.SendStruct(new JargPack { action = "position", name = client_.name, x = player_.Position.X, y = player_.Position.Y, angle = PlayerSeeAngle});
            }
            currentFloor_.KillFarSectors(player_, gameTime, camera_);
            bs_.Update(gameTime);
            //currentFloor_.UpdateBlocks(gameTime, camera_);
            GlobalWorldLogic.Update(gameTime);

            currentFloor_.UpdateCreatures(gameTime, player_, GraphicsDevice);

            ps_.Update(gameTime);

            PlayerSeeAngle =
                (float) Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X);
            if (car != null) {
                float f = -car.Roration - 3.14f/4f;
                pivotpoint_ += new Vector2((float) (Math.Cos(f)*car.Vel + Math.Sin(f)*car.Vel)*50,
                                           (float) (-Math.Sin(f)*car.Vel + Math.Cos(f)*car.Vel)*50/
                                           GraphicsDevice.DisplayMode.AspectRatio);
            }
            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float) gameTime.ElapsedGameTime.TotalSeconds*4);
            camera_.X = (int)camera_.X;
            camera_.Y = (int)camera_.Y;

            //LightCollection[0].Position = new Vector3(ms_.X+camera_.X,ms_.Y+camera_.Y,10);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            if (Settings.DebugInfo) {
                swDraw_.Restart();
            }

            GraphicsDevice.Clear(Color.Black);

            drawAction_(gameTime);

            ws_.Draw(spriteBatch_, lig1, gameTime);

            lineBatch_.Draw();
            lineBatch_.Clear();

            if (Settings.DebugInfo) {
                swDraw_.Stop();
                DebugInfoDraw();
                FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int) Settings.Resolution.X,
                                      (int) Settings.Resolution.Y, swDraw_, swUpdate_);
            }
        }

        private void GameDraw(GameTime gameTime) {
            //lightCollection_.Clear();
            //lightCollection_.AddRange(currentFloor_.GetLights());
            //if (Flashlight) {
            //    var hpos = new Vector3(player_.Position.X, player_.Position.Y, 0.5f);
                //var ray = new Ray(hpos, Vector3.Normalize(new Vector3(ms_.X + camera_.X, ms_.Y + camera_.Y, 1) - hpos));
                //lightCollection_.Add(new Light {
                //    Color = Color.White,
                //    LightRadius = 30*3,
                //    Position = ray.Position + ray.Direction*40,
                //    Power = 10
                //});
                //lightCollection_.Add(new Light {
                //    Color = Color.White,
                //    LightRadius = 50*3,
                //    Position = ray.Position + ray.Direction*90,
                //    Power = 50
                //});
                //lightCollection_.Add(new Light {
                //    Color = Color.White,
                //    LightRadius = 80*3,
                //    Position = ray.Position + ray.Direction*160,
                //    Power = 50
                //});
                //lightCollection_.Add(new Light {
                //    Color = Color.White,
                //    LightRadius = 90*3,
                //    Position = ray.Position + ray.Direction*280,
                //    Power = 150
                //});
            //}


            //EffectOmnilight.Parameters["cpos"].SetValue(new[]
            //{player_.Position.X - camera_.X, player_.Position.Y - camera_.Y});

            GraphicsDevice.SetRenderTarget(colorMapRenderTarget_);
            GraphicsDevice.Clear(Color.Black);
            //color maps
            spriteBatch_.Begin();
            currentFloor_.DrawFloors(gameTime, camera_);
            currentFloor_.DrawDecals(gameTime, camera_);
            spriteBatch_.End();
            GraphicsDevice.SetRenderTarget(shadowMapRenderTarget_);
            GraphicsDevice.Clear(Color.Transparent);
            currentFloor_.ShadowRender();
            GraphicsDevice.SetRenderTarget(colorMapRenderTarget_);
            spriteBatch_.Begin();
            spriteBatch_.Draw(shadowMapRenderTarget_, Vector2.Zero, new Color(1,1,1,0.75f));
            currentFloor_.DrawBlocks(gameTime, camera_, player_);
            player_.Draw(gameTime, camera_, null);
            if (client_ != null) {
                foreach (var otherclient in client_.otherclients) {
                    player_.Draw(gameTime, camera_, otherclient);
                }
            }
            spriteBatch_.End();
            currentFloor_.DrawEntities(gameTime, camera_);
            spriteBatch_.Begin();
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

                lightMapTexture_ = GenerateShadowMap();

                GraphicsDevice.SetRenderTarget(null);
                DrawCombinedMaps();
            }
            else {
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch_.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, ScaleAll2);
                spriteBatch_.Draw(colorMapRenderTarget_, Vector2.Zero, Color.White);
                spriteBatch_.End();
            }

            spriteBatch_.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            if (levelWorker_.Generating()) {
                spriteBatch_.Draw(gear, new Vector2(10, 10), lwstatusColor_);
            }
            if (levelWorker_.Loading()) {
                spriteBatch_.Draw(arup_, new Vector2(42, 10), lwstatusColor_);
            }
            if (levelWorker_.Storing()) {
                spriteBatch_.Draw(ardown_, new Vector2(74, 10), lwstatusColor_);
            }
            if (Settings.DebugInfo) {
                spriteBatch_.DrawString(font1_, levelWorker_.GenerationCount().ToString(), new Vector2(10, 10),
                                        Color.White);
                spriteBatch_.DrawString(font1_, levelWorker_.LoadCount().ToString(), new Vector2(42, 10), Color.White);
                spriteBatch_.DrawString(font1_, levelWorker_.StoreCount().ToString(), new Vector2(74, 10), Color.White);
                spriteBatch_.DrawString(font1_, levelWorker_.ReadyCount().ToString(), new Vector2(140, 10), Color.Red);
            }
            spriteBatch_.End();
        }

        private Texture2D GetRenderedFlashlight() {
            return null;
        }

        public void DrawDebugRenderTargets(GameTime time) {
            GameDraw(time);
            // Draw some debug textures
            //GraphicsDevice.Clear(Color.DarkGreen);
            spriteBatch_.Begin();

            var size = new Rectangle(0, 0, colorMapRenderTarget_.Width/2, colorMapRenderTarget_.Height/2);
            spriteBatch_.Draw(
                colorMapRenderTarget_,
                new Rectangle(0, 0,
                              size.Width,
                              size.Height),
                Color.White);

            spriteBatch_.Draw(
                lightMapRenderTarget_,
                new Rectangle(0, size.Height,
                              size.Width,
                              size.Height),
                Color.White);

            spriteBatch_.End();
        }

        public void DrawMajorAtlas(GameTime time)
        {
            GameDraw(time);
            spriteBatch_.Begin();

            var size = new Rectangle(0, 0, Atlases.Instance.MajorAtlas.Width, Atlases.Instance.MajorAtlas.Height);
            spriteBatch_.Draw(
                Atlases.Instance.MajorAtlas,
                size,
                Color.White);
            string s = string.Format("{0}/{1} ({2}x{3})", Atlases.Instance.MajorCount, Atlases.Instance.MajorCapacity,
                                     Atlases.Instance.MajorAtlas.Width, Atlases.Instance.MajorAtlas.Height);
            spriteBatch_.DrawString(font1_, s, Vector2.Zero, Color.Black);
            spriteBatch_.DrawString(font1_, s, Vector2.One, Color.White);

            spriteBatch_.End();
        }

        private Matrix ScaleAll2 = Matrix.CreateScale(1);
        private void DrawCombinedMaps() {
            lightEffect2_.Parameters["ambient"].SetValue(GlobalWorldLogic.GetCurrentSlen());
            lightEffect2_.Parameters["ambientColor"].SetValue(Color.White.ToVector4());

            //light burst
            lightEffect2_.Parameters["lightAmbient"].SetValue(1);
            lightEffect2_.Parameters["ColorMap"].SetValue(colorMapRenderTarget_);
            lightEffect2_.Parameters["ShadingMap"].SetValue(lightMapTexture_);

            spriteBatch_.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, ScaleAll2);
            foreach (EffectPass pass in lightEffect2_.CurrentTechnique.Passes) {
                pass.Apply();
                spriteBatch_.Draw(colorMapRenderTarget_, Vector2.Zero, Color.White);
            }
            spriteBatch_.End();
        }

        private Texture2D GenerateShadowMap() {
            //GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.SetRenderTarget(lightMapRenderTarget_);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch_.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatch_.Draw(fltex, (player_.Position - camera_)/LightQ, null, Color.White, PlayerSeeAngle + MathHelper.PiOver2, new Vector2((fltex.Width / 2f), fltex.Height), 1f/LightQ, SpriteEffects.None, 0);
            if (client_ != null) {
                foreach (var otherclient in client_.otherclients) {
                    spriteBatch_.Draw(fltex, (new Vector2(otherclient.Value.x, otherclient.Value.y) - camera_)/LightQ,
                                      null, Color.White, otherclient.Value.angle + MathHelper.PiOver2,
                                      new Vector2((fltex.Width/2f), fltex.Height), 1f/LightQ, SpriteEffects.None, 0);
                }
            }
            //currentFloor_.DrawAmbient(camera_, LightQ);
            spriteBatch_.End();

            GraphicsDevice.BlendState = BlendState.Additive;
            //GraphicsDevice.BlendState = new BlendState{AlphaSourceBlend = Blend.Zero, AlphaDestinationBlend = Blend.One};

            // For every light inside the current scene, you can optimize this
            // list to only draw the lights that are visible a.t.m.
            foreach (Light light in lightCollection_) {
                lightEffect1_.CurrentTechnique = lightEffect1_.Techniques["DeferredPointLight"];
                lightEffect1_.Parameters["lightStrength"].SetValue(light.Power);
                lightEffect1_.Parameters["lightPosition"].SetValue(light.GetWorldPosition(camera_, LightQ));
                lightEffect1_.Parameters["lightColor"].SetValue(light.Color.ToVector3());
                lightEffect1_.Parameters["lightRadius"].SetValue(light.LightRadius/LightQ);

                lightEffect1_.Parameters["screenWidth"].SetValue(GraphicsDevice.Viewport.Width);
                lightEffect1_.Parameters["screenHeight"].SetValue(GraphicsDevice.Viewport.Height);

                foreach (EffectPass pass in lightEffect1_.CurrentTechnique.Passes) {
                    pass.Apply();

                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices_, 0, 2);
                }
            }

            return lightMapRenderTarget_;
        }

        private void DebugInfoDraw() {
            string ss =
                string.Format(
                    "SAng {0} \nPCount {1}   BCount {5}\nDT {3} WorldT {2} \nSectors {4} Generated {6} \nSTri {7} slen {8} {9}\nMH={10} KH={11}",
                    PlayerSeeAngle, ps_.Count(), GlobalWorldLogic.Temperature, GlobalWorldLogic.CurrentTime,
                    currentFloor_.SectorCount(), bs_.GetCount(), currentFloor_.generated,
                    currentFloor_.GetShadowrenderCount()/3, GlobalWorldLogic.GetCurrentSlen(),
                    GlobalWorldLogic.DayPart, ws_.Mopusehook, ws_.Keyboardhook);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, ss, new Vector2(500, 10), Color.White);

            var nx = (int) ((ms_.X + camera_.X)/32.0);
            var ny = (int) ((ms_.Y + camera_.Y)/32.0);

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