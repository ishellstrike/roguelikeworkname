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
        private readonly Stopwatch swDraw_ = new Stopwatch();
        private readonly Stopwatch swUpdate_ = new Stopwatch();
        private Effect effectOmnilight_;
        private BasicEffect basicE;
        private bool flashlight_ = true;
        public float PlayerSeeAngle;
        private Action<GameTime> updateAction_ = x => { };
        private WindowsMediaPlayer wmPs_;
        private Texture2D bag_;
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
        private Camera cam;

        private Effect solidEffect, billboardEffect;

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

#if DEBUG   
            CopyDir(@"..\..\..\..\rglikeworknamecontent\Data", @"Content\Data");
            CopyDir(@"..\..\..\..\rglikeworknamecontent\Textures", @"Content\Textures");
#endif

            graphics_ = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";

            if (!Directory.Exists(Settings.GetWorldsDirectory())) {
                Directory.CreateDirectory(Settings.GetWorldsDirectory());
            }
        }


        void CopyDir(string FromDir, string ToDir)
        {
            Directory.CreateDirectory(ToDir);
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + "\\" + Path.GetFileName(s1);
                File.Copy(s1, s2, true);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {
                CopyDir(s, ToDir + "\\" + Path.GetFileName(s));
            }
        }


        protected Vector2 ContainerOn { get; set; }

        protected override void Initialize() {
            CreateQube();
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
            if (client_ == null) {
                currentFloor_.SaveAllAndExit(player_, inventory_);
            }
            else {
                Settings.NeedExit = true;
            }
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

        private int LightQ = 1;
        private void ResolutionChanging() {
            var height = (int) Settings.Resolution.Y;
            var width = (int) Settings.Resolution.X;

            if(height == 0 || width == 0) {return;}
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
            colorMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height);

            lightMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width / LightQ, height / LightQ, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            shadowMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width / LightQ, height / LightQ, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            effectOmnilight_.Parameters["screenWidth"].SetValue(width);
            effectOmnilight_.Parameters["screenHeight"].SetValue(height);
            Atlases.Instance.RebuildAtlases(GraphicsDevice);

            var a = cam.Yaw;
            var b = cam.Pitch;
            var c = cam.Zoom;
            cam = new Camera(Settings.Resolution.X / Settings.Resolution.Y, Vector3.Zero){Yaw = a, Pitch = b, Zoom = c};
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

            ContentProvider.Init(GraphicsDevice);

            new Atlases(GraphicsDevice);

            whitepixel_ = new Texture2D(graphics_.GraphicsDevice, 1, 1);
            whitepixel_.SetData(new[] {Color.White});

            yellowpixel_ = new Texture2D(graphics_.GraphicsDevice, 1, 1);
            yellowpixel_.SetData(new[] { new Color(255,255,255,64) });

            font1_ = Content.Load<SpriteFont>(@"Fonts\Font1");
            Settings.Font = font1_;

            lig1 = Content.Load<Effect>(@"Effects\Lighting1");
            effectOmnilight_ = Content.Load<Effect>(@"Effects\Effect1");
            solidEffect = Content.Load<Effect>(@"Effects\solid");
            solidShadowEffect = Content.Load<Effect>(@"Effects\solidShadow");

            gear = ContentProvider.LoadTexture(@"Textures\gear");
            bag_ = ContentProvider.LoadTexture(@"Textures\bag");
            caracter = ContentProvider.LoadTexture(@"Textures\caracter");
            map = ContentProvider.LoadTexture(@"Textures\map");
            fltex = ContentProvider.LoadTexture(@"Textures\Effects\fl1");
            basicE = new BasicEffect(GraphicsDevice);

            ps_ = new ParticleSystem(spriteBatch_,
                         ParsersCore.LoadTexturesInOrder(
                             Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord"));
            new BulletSystem(spriteBatch_,
                                   ParsersCore.LoadTexturesInOrder(
                                       Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord"),
                                   currentFloor_, font1_, lineBatch_);
            inventory_ = new InventorySystem();

            player_ = new Player(spriteBatch_, ContentProvider.LoadTexture(@"Textures\Units\car"), font1_, inventory_);

            Logger.Info("ContentProvider loaded {0} textures", ContentProvider.TotalLoaded());

            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(ws_);

            levelWorker_ = new LevelWorker(currentFloor_);
            levelWorker_.Start();
            Action dbl = DataBasesLoadAndThenInitialGeneration;
            dbl.BeginInvoke(null, null);

            cam = new Camera(Settings.Resolution.X / Settings.Resolution.Y, Vector3.Zero);

            new AchievementDataBase();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            colorMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color,
                                                       DepthFormat.Depth24Stencil8, 1, RenderTargetUsage.PreserveContents);
            lightMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width / LightQ, height / LightQ, false, SurfaceFormat.Color,
                                                       DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            shadowMapRenderTarget_ = new RenderTarget2D(GraphicsDevice, width / LightQ, height / LightQ, false, SurfaceFormat.Color,
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

        public Texture2D yellowpixel_ { get; set; }

        private bool InitialFinish;
        //invoke after DataBasesLoadAndThenInitialGeneration
        private void InitialGeneration() {
            RunRadioGhostBox();
            wmPs_.controls.stop();
            WindowRadio.Visible = false;

            var sw = new Stopwatch();
            sw.Start();
            ShowInfoWindow("Initial map generation", "");
            currentFloor_ = new GameLevel(spriteBatch_, lineBatch_, font1_, GraphicsDevice, levelWorker_);
            if (levelWorker_ != null) {
                levelWorker_.Stop();
                levelWorker_ = new LevelWorker(currentFloor_);
                levelWorker_.Start();
            }

            currentFloor_.lw_ = levelWorker_;
            player_.Inventory = inventory_;
            BulletSystem.level = currentFloor_;
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
                ItemDataBase.Craft.Count);

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

            
            WindowsUpdate(gameTime);
            ws_.Update(gameTime, ms_, lms_, ks_, lks_, false);

            if (IsActive && InitialFinish) {
                UpdateKeyboard(gameTime);
                UpdateMouse(gameTime);
            }

            if (player_ != null)
            {
                pivotpoint_ = Vector2.Subtract(new Vector2(player_.Position.X, player_.Position.Y), Vector2.Divide(Settings.Resolution, 2));
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
        int SendTick;
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

            //currentFloor_.CalcWision(player_,
            //                         (float)
            //                         Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y,
            //                                    ms_.X - player_.Position.X + camera_.X),
            //                         seeAngleDeg);

            
            //if (car != null) {
            //    car.Update(gameTime, player_);
            //}
            player_.Update(gameTime, currentFloor_.GetSector((int) aa.X, (int) aa.Y), player_);
            if (client_ != null && SendTick >= 6) {
                SendTick = 0;
                client_.SendStruct(new JargPack { action = "position", name = client_.name, x = player_.Position.X, y = player_.Position.Y, angle = PlayerSeeAngle});
            }
            SendTick++;

            currentFloor_.KillFarSectors(player_, gameTime, camera_);
            BulletSystem.Update(gameTime);
            //currentFloor_.UpdateBlocks(gameTime, camera_);
            GlobalWorldLogic.Update(gameTime);

            currentFloor_.UpdateCreatures(gameTime, player_, GraphicsDevice);

            ps_.Update(gameTime);

            PlayerSeeAngle =
                (float) Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X);

            cam.LookAt = new Vector3(player_.Position.X/32f, player_.Position.Y/32f, 0);

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

            lineBatch_.Draw(cam);

            if (Settings.DebugInfo) {
                swDraw_.Stop();
                DebugInfoDraw();
                FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int) Settings.Resolution.X,
                                      (int) Settings.Resolution.Y, swDraw_, swUpdate_);
            }
        }


        private void GameDraw(GameTime gameTime) {

            GraphicsDevice.SetRenderTarget(colorMapRenderTarget_);
            GraphicsDevice.Clear(Color.Black);
            currentFloor_.RenderMap(GraphicsDevice,cam,solidEffect, billboardEffect, player_);
            currentFloor_.RenderCreatures(GraphicsDevice, cam, solidEffect);

            solidEffect.Parameters["worldMatrix"].SetValue(Matrix.CreateBillboard(player_.creatureWorld.Translation, cam.Position, cam.Backward, null));
            foreach (var pass in solidEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, player_.vert, 0, player_.vert.Length / 3);
            }

            //GraphicsDevice.SetRenderTarget(shadowMapRenderTarget_);
            //float currentSlen = GlobalWorldLogic.GetCurrentSlen();
            //GraphicsDevice.Clear(new Color(0,0,0,currentSlen));
            //currentFloor_.RenderShadowMap(GraphicsDevice, cam, solidShadowEffect);
            //solidEffect.Parameters["worldMatrix"].SetValue(Matrix.CreateRotationZ(PlayerSeeAngle)*Matrix.CreateScale(5)*Matrix.CreateTranslation(player_.Position/32));
            //solidEffect.Parameters["shaderTexture"].SetValue(fltex);
            //solidEffect.Parameters["lightDirection"].SetValue(Vector3.Forward);
            //foreach (var pass in solidEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    VertexPositionNormalTexture[] v;
            //    List<VertexPositionNormalTexture> vv = new List<VertexPositionNormalTexture>();

            //    vv.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.1f), Vector3.Zero, new Vector2(0, 1)));
            //    vv.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 2, 0.1f), Vector3.Zero, new Vector2(0, 0)));
            //    vv.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 2, 0.1f), Vector3.Zero, new Vector2(1, 0)));
            //    vv.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 2, 0.1f), Vector3.Zero, new Vector2(1, 0)));
            //    vv.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.1f), Vector3.Zero, new Vector2(1, 1)));
            //    vv.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.1f),Vector3.Zero, new Vector2(0, 1)));

            //    v = vv.ToArray();
            //    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, v, 0, v.Length / 3);
            //}

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch_.Begin();
            spriteBatch_.Draw(colorMapRenderTarget_, Vector2.Zero, Color.White);
            spriteBatch_.Draw(shadowMapRenderTarget_, Vector2.Zero, null, new Color(1, 1, 1, 0.5f), 0, Vector2.Zero, LightQ, SpriteEffects.None, 0);
            spriteBatch_.End();

            currentFloor_.RenderBlockMap(GraphicsDevice, cam, solidEffect);

            lineBatch_.Draw(cam);

            //solidShadowEffect.Parameters["worldMatrix"].SetValue(Matrix.CreateTranslation(nx, ny, 0));
            //foreach (var pass in solidShadowEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, qube, 0, qube.Length / 3);
            //}

            if (Settings.DebugInfo)
            {
                spriteBatch_.Begin();
                spriteBatch_.DrawString(font1_, "req:" + levelWorker_.LoadCount, new Vector2(11, 101), Color.Black);
                spriteBatch_.DrawString(font1_, "req:" + levelWorker_.LoadCount, new Vector2(10, 100), Color.White);
                spriteBatch_.DrawString(font1_, "store:" + levelWorker_.StoreCount, new Vector2(11, 121), Color.Black);
                spriteBatch_.DrawString(font1_, "store:" + levelWorker_.StoreCount, new Vector2(10, 120), Color.White);
                spriteBatch_.DrawString(font1_, "ready:" + levelWorker_.ReadyCount, new Vector2(11, 141), Color.Black);
                spriteBatch_.DrawString(font1_, "ready:" + levelWorker_.ReadyCount, new Vector2(10, 140), Color.White);
                spriteBatch_.End();
            }

            //base.Draw(gameTime);
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

            DepthStencilState depthStencilState = new DepthStencilState();
            depthStencilState.DepthBufferFunction = CompareFunction.LessEqual;
            GraphicsDevice.DepthStencilState = depthStencilState;
            spriteBatch_.Draw(
                colorMapRenderTarget_,
                new Rectangle(0, size.Height,
                              size.Width,
                              size.Height),
                Color.White);


            spriteBatch_.Draw(
                shadowMapRenderTarget_,
                new Rectangle(size.Width, 0,
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
        private Effect solidShadowEffect;

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
            spriteBatch_.Draw(fltex, (new Vector2(player_.Position.X,player_.Position.Y)  - camera_)/LightQ, null, Color.White, PlayerSeeAngle + MathHelper.PiOver2, new Vector2((fltex.Width / 2f), fltex.Height), 1f/LightQ, SpriteEffects.None, 0);
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

        private VertexPositionNormalTexture[] qube;
        private void CreateQube() {
            var b = new List<VertexPositionNormalTexture>();
            int i = 0;
            int j = 0;
            var h = 1.05f;
                b.Add(new VertexPositionNormalTexture(
                          new Vector3(0 -0.05f, 1.05f, h), Vector3.Backward, Vector2.Zero));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(1.05f, 1.05f, h), Vector3.Backward, new Vector2(1, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(0 - 0.05f, 1.05f, 0), Vector3.Backward, new Vector2(0, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(1.05f, 1.05f, h), Vector3.Backward, new Vector2(1, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(1.05f, 1.05f, 0), Vector3.Backward, new Vector2(1, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(0 - 0.05f, 1.05f, 0), Vector3.Backward, new Vector2(0, 1)));

                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i - 0.05f, j - 0.05f, h), Vector3.Right, Vector2.Zero));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j - 0.05f, 0), Vector3.Right,
                        new Vector2(0, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j - 0.05f, h), Vector3.Right,
                        new Vector2(1, 0)));

                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j - 0.05f, h), Vector3.Right,
                        new Vector2(1, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j - 0.05f, 0), Vector3.Right,
                       new Vector2(0, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j - 0.05f, 0), Vector3.Right,

                        new Vector2(1, 1)));

                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i - 0.05f, j - 0.05f, h), Vector3.Forward,
                          Vector2.Zero));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j + 1.05f, h), Vector3.Forward,
                         new Vector2(1, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j - 0.05f, 0), Vector3.Forward,
                        new Vector2(0, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j + 1.05f, h), Vector3.Forward,
                         new Vector2(1, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j + 1.05f, 0), Vector3.Forward,
                        
                        new Vector2(1, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i - 0.05f, j - 0.05f, 0), Vector3.Forward,
                        new Vector2(0, 1.05f)));

                b.Add(new VertexPositionNormalTexture(
                          new Vector3(i + 1.05f, j - 0.05f, h), Vector3.Left,
                          Vector2.Zero));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j - 0.05f, 0), Vector3.Left,
                        new Vector2(0, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j + 1.05f, h), Vector3.Left,
                        new Vector2(1, 0)));

                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j + 1.05f, h), Vector3.Left,
                        new Vector2(1, 0)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j - 0.05f, 0), Vector3.Left,
                        new Vector2(0, 1)));
                b.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(i + 1.05f, j + 1.05f, 0), Vector3.Left,
                        
                        new Vector2(1, 1)));


            b.Add(new VertexPositionNormalTexture(
                      new Vector3(i - 0.05f, j - 0.05f, h), Vector3.Up,
                       Vector2.Zero));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i + 1.05f, j - 0.05f, h), Vector3.Up,
                   new Vector2(1, 0)));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i, j + 1.05f, h), Vector3.Up,
                    new Vector2(0, 1)));

            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i + 1.05f, j - 0.05f, h), Vector3.Up,
                    new Vector2(1, 0)));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i + 1.05f, j + 1.05f, h), Vector3.Up,
                   
                    new Vector2(1, 1)));
            b.Add(
                new VertexPositionNormalTexture(
                    new Vector3(i - 0.05f, j + 1.05f, h), Vector3.Up,
                     new Vector2(0, 1)));

            qube = b.ToArray();
        }

        private int nx, ny;
        private void DebugInfoDraw() {
            string ss =
                string.Format(
                    "SAng {0} \nPCount {1}   BCount {5}\nDT {3} WorldT {2} \nSectors {4} Generated {6} \nSTri {7} slen {8} {9}\nMH={10} KH={11}\n{12}",
                    PlayerSeeAngle, ps_.Count(), GlobalWorldLogic.Temperature, GlobalWorldLogic.CurrentTime,
                    currentFloor_.SectorCount(), BulletSystem.GetCount(), currentFloor_.generated,
                    currentFloor_.GetShadowrenderCount()/3, GlobalWorldLogic.GetCurrentSlen(),
                    GlobalWorldLogic.DayPart, ws_.Mopusehook, ws_.Keyboardhook,cam);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, ss, new Vector2(500, 10), Color.White);


            spriteBatch_.DrawString(font1_, string.Format("       {0} {1}", nx, ny), new Vector2(ms_.X, ms_.Y),
                                    Color.White);

            spriteBatch_.End();
        }
    }
}