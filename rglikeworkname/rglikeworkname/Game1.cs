using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
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

namespace jarg
{
    public partial class Game1 : Game
    {
        private WindowSystem ws_;
        private ParticleSystem ps_;
        private BulletSystem bs_;
        private GameLevel currentFloor_;
        private InventorySystem inventory_;
        
        private Player player_;

        private bool GameStarted;

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

        private Achievements achievements_;

        private Texture2D whitepixel_;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Game1() {
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
#endif

            if (File.Exists("JARGLog_previous.txt"))
            {
                File.Delete("JARGLog_previous.txt");
            }
            if (File.Exists("JARGLog.txt"))
            {
                File.Move("JARGLog.txt", "JARGLog_previous.txt");
            }

            Version.Init();

            graphics_ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            if (!Directory.Exists(Settings.GetWorldsDirectory())) {
                Directory.CreateDirectory(Settings.GetWorldsDirectory());
            }
        }

        protected override void Initialize()
        {
            Window.Title = Version.GetLong();

            Settings.Resolution = new Vector2(1024, 768);
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
            graphics_.PreferredBackBufferWidth = (int)Settings.Resolution.X;
            graphics_.SynchronizeWithVerticalRetrace = false;
            
            IsFixedTimeStep = false; //wierd, actually it's true
            IsMouseVisible = true;
            graphics_.ApplyChanges();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            Atlases a = new Atlases(Content);

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

            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(whitepixel_, font1_, ws_);

            Action dbl = DataBasesLoadAndThenInitialGeneration;
            dbl.BeginInvoke(null, null);

            achievements_ = new Achievements();

            rt2d = new RenderTarget2D(GraphicsDevice, (int)Settings.Resolution.X, (int)Settings.Resolution.Y, false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
        }

        private void InitialGeneration() {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ShowInfoWindow("Initial map generation", "");
            currentFloor_ = new GameLevel(spriteBatch_, font1_, GraphicsDevice);
            ps_ = new ParticleSystem(spriteBatch_,
                                     ParsersCore.LoadTexturesInOrder(
                                         Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content));
            bs_ = new BulletSystem(spriteBatch_,
                                   ParsersCore.LoadTexturesInOrder(
                                       Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content),
                                   currentFloor_, font1_, lineBatch_);
            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_);
            inventory_ = new InventorySystem();
            inventory_.AddItem(new Item("testhat", 1));
            inventory_.AddItem(new Item("testhat2", 1));
            inventory_.AddItem(new Item("ak47", 1));
            inventory_.AddItem(new Item("a762", 100));
            inventory_.AddItem(new Item("a762", 100000));
            UpdateInventoryContainer();
            HideInfoWindow();
            sw.Stop();
            logger.Info("Initial generation in {0}",sw.Elapsed);
            player_.onUpdatedEquip += UpdateCaracterWindowItems;
            player_.onShoot +=player__onShoot;
        }

        void player__onShoot(object sender, EventArgs e) {

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
            logger.Info("Total:\n     {1} Monsters\n     {2} Blocks\n     {3} Floors\n     {4} Items\n     {5} Schemes\n     {6} Buffs\n     {7} Dialogs\n     {8} Names\nloaded in {0}", 
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

        protected override void UnloadContent()
        {
           
        }

        private Stopwatch sw_update = new Stopwatch();
        private Stopwatch sw_draw = new Stopwatch();
        private Stopwatch sw_shadows = new Stopwatch();

        public static bool ErrorExit;
        public float seeAngleDeg = 60;
        public float PlayerSeeAngle;
        public TimeSpan sec = TimeSpan.Zero;
        public int jji=-2, jjj=-2;
        private Action<GameTime> UpdateAction = x => { };
        protected override void Update(GameTime gameTime) {
            SecondTimespan += gameTime.ElapsedGameTime;
            //first

            if(Settings.DebugInfo) {
                sw_update.Restart();
            }

            if (ErrorExit) Exit();

            base.Update(gameTime);

            WindowsUpdate(gameTime);
            ws_.Update(gameTime, ms_, lms_, false);

            KeyboardUpdate(gameTime);
            MouseUpdate(gameTime);

            UpdateAction(gameTime);

            if (Settings.DebugInfo) {
                FrameRateCounter.Update(gameTime);
            }

            if (player_ != null) {
                PlayerSeeAngle =
                    (float) Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X);
            }

            if (Settings.DebugInfo)
            {
                sw_update.Stop();
            }

            // last
            if (SecondTimespan.TotalSeconds >= 1)
            {
                SecondTimespan = TimeSpan.Zero;
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
                if(WindowGlobal.Visible) {
                    currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_);
                }
            }


            if ((int) player_.Position.X/3 == (int) player_.LastPos.X/3 &&
                (int) player_.Position.Y/3 == (int) player_.LastPos.Y/3) {
                if (seeAngleDeg < 150) {
                    seeAngleDeg += (float) gameTime.ElapsedGameTime.TotalSeconds*50;
                }
                else {
                    seeAngleDeg = 150;
                }
            }
            else {
                if (seeAngleDeg > 60) {
                    seeAngleDeg -= (float) gameTime.ElapsedGameTime.TotalSeconds*2000;
                }
                else {
                    seeAngleDeg = 60;
                }
            }
            currentFloor_.CalcWision(player_,
                                        (float)
                                        Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y,
                                                ms_.X - player_.Position.X + camera_.X),
                                        seeAngleDeg);

            var aa = currentFloor_.GetInSectorPosition(player_.GetPositionInBlocks());
            player_.Update(gameTime, currentFloor_.GetSector((int)aa.X, (int)aa.Y), player_);
            currentFloor_.KillFarSectors(player_, gameTime);
            ps_.Update(gameTime);
            bs_.Update(gameTime);
            currentFloor_.UpdateBlocks(gameTime, camera_);
            GlobalWorldLogic.Update(gameTime);

            currentFloor_.UpdateCreatures(gameTime, player_);

            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float) gameTime.ElapsedGameTime.TotalSeconds*4);

            if(Settings.NeedToShowInfoWindow) {
                ShowInfoWindow(Settings.NTS1, Settings.NTS2);
                Settings.NeedToShowInfoWindow = false;
            }

            if(Settings.NeedExit) {
                Exit();
            }
        }

        private void KeyboardUpdate(GameTime gameTime)
        {
            lks_ = ks_;
            ks_ = Keyboard.GetState();

            if (ks_[Keys.F1] == KeyState.Down && lks_[Keys.F1] == KeyState.Up) {
                Settings.DebugInfo = !Settings.DebugInfo;
            }

            if (ks_[Keys.F2] == KeyState.Down && lks_[Keys.F2] == KeyState.Up)
            {
                Settings.DebugWire = !Settings.DebugWire;
            }

            if (ks_[Keys.W] == KeyState.Down) {
                player_.Accelerate(new Vector2(0, -10));
            }
            if (ks_[Keys.S] == KeyState.Down) {
                player_.Accelerate(new Vector2(0, 10));
            }
            if (ks_[Keys.A] == KeyState.Down) {
                player_.Accelerate(new Vector2(-10, 0));
            }
            if (ks_[Keys.D] == KeyState.Down) {
                player_.Accelerate(new Vector2(10, 0));
            }

            if (ks_[Keys.G] == KeyState.Down && lks_[Keys.G] == KeyState.Up) {
                currentFloor_.Rebuild();
            }

            if (ks_[Keys.H] == KeyState.Down && lks_[Keys.H] == KeyState.Up) {
                currentFloor_.ExploreAllMap();
            }

            if (ks_[Keys.Escape] == KeyState.Down && lks_[Keys.Escape] == KeyState.Up) {
                if (!ws_.CloseTop()) {
                    WindowIngameMenu.Visible = true;
                }
            }

            if (ks_[Keys.M] == KeyState.Down && lks_[Keys.M] == KeyState.Up) {
                WindowGlobal.Visible = !WindowGlobal.Visible;
                if(WindowGlobal.Visible) {
                    currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_);
                    ImageGlobal.image = currentFloor_.GetMap();
                }
            }

            if (ks_[Keys.C] == KeyState.Down && lks_[Keys.C] == KeyState.Up) {
                WindowCaracter.Visible = !WindowCaracter.Visible;
            }

            if (ks_[Keys.I] == KeyState.Down && lks_[Keys.I] == KeyState.Up) {
                    WindowInventory.Visible = !WindowInventory.Visible;
                if (WindowInventory.Visible) {
                    WindowInventory.OnTop();
                    WindowInventory.SetPosition(new Vector2(0, 0));
                }
            }

            if (ks_[Keys.L] == KeyState.Down && lks_[Keys.L] == KeyState.Up)
            {
                WindowEventLog.Visible = !WindowEventLog.Visible;
                if (WindowEventLog.Visible)
                {
                    WindowEventLog.OnTop();
                }
            }

            if (ks_[Keys.O] == KeyState.Down && lks_[Keys.O] == KeyState.Up)
            {
                WindowStatist.Visible = !WindowStatist.Visible;
                if (WindowStatist.Visible)
                {
                    WindowStatist.OnTop();
                }
            }

            if (ks_[Keys.P] == KeyState.Down && lks_[Keys.P] == KeyState.Up)
            {
                WindowStatist.Visible = !WindowStatist.Visible;
                if (WindowStatist.Visible)
                {
                    WindowStatist.OnTop();
                }
            }

            if (WindowContainer.Visible && ks_[Keys.R] == KeyState.Down && lks_[Keys.R] == KeyState.Up)
            {
                ButtonContainerTakeAll_onPressed(null, null);
            }

            if (player_ != null) {
                pivotpoint_ = new Vector2(player_.Position.X - (Settings.Resolution.X - 200)/2,
                                          player_.Position.Y - Settings.Resolution.Y/2);
            }

        }

        private TimeSpan doubleclicktimer = TimeSpan.Zero;
        private TimeSpan sec20glitch;
        private bool firstclick;
        private bool doubleclick;
        private void MouseUpdate(GameTime gameTime)
        {
            lms_ = ms_;
            ms_ = Mouse.GetState();

            doubleclicktimer += gameTime.ElapsedGameTime;

            doubleclick = false;
            if(firstclick && ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released && doubleclicktimer.TotalMilliseconds < 300) {
                doubleclick = true;
                firstclick = false;
            }
            if (!firstclick && ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                firstclick = true;
                doubleclicktimer = TimeSpan.Zero;
            }
            if (doubleclicktimer.TotalMilliseconds > 300) {
                firstclick = false;
                doubleclick = false;
            }

            //sec20glitch += gameTime.ElapsedGameTime;
            //if(sec20glitch.TotalSeconds > 1 && (ms_.X != lms_.X || ms_.Y != lms_.Y)) {
            //    //ws_.DoGlitch();
            //    sec20glitch = TimeSpan.Zero;
            //}

            if (!ws_.Mopusehook && GameStarted) {
                if (ms_.LeftButton == ButtonState.Pressed) {
                    player_.TryShoot(bs_, PlayerSeeAngle);
                }

                int nx = (ms_.X + (int) camera_.X)/32;
                int ny = (ms_.Y + (int) camera_.Y)/32;

                if (ms_.X + camera_.X < 0) nx--;
                if (ms_.Y + camera_.Y < 0) ny--;

                WindowIngameHint.Visible = false;

                if (player_ != null && !currentFloor_.IsCreatureMeele((int) containerOn.X, (int) containerOn.Y, player_)) {
                    WindowContainer.Visible = false;
                }

                if (currentFloor_ != null) {
                    var nxny = currentFloor_.GetBlock(nx, ny);
                    if (nxny != null && nxny.Lightness == Color.White) // currentFloor_.IsExplored(aa))
                    {
                        var a = currentFloor_.GetBlock(nx, ny);
                        if (a != null) {
                            var b = BlockDataBase.Data[a.Id];
                            string s = Block.GetSmartActionName(b.SmartAction) + " " + b.Name;
                            if (Settings.DebugInfo) s += " id" + a.Id + " tex" + b.MTex;

                            if (currentFloor_.IsCreatureMeele(nx, ny, player_)) {
                                if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                                    var undermouseblock = BlockDataBase.Data[a.Id];
                                    switch (undermouseblock.SmartAction) {
                                        case SmartAction.ActionSee:
                                            EventLog.Add("Âû âèäèòå " + undermouseblock.Name,
                                                         GlobalWorldLogic.CurrentTime,
                                                         Color.Gray, LogEntityType.SeeSomething);
                                            break;
                                        case SmartAction.ActionOpenContainer:
                                            WindowContainer.Visible = true;
                                            WindowContainer.SetPosition(new Vector2(Settings.Resolution.X/2, 0));
                                            UpdateContainerContainer((a as StorageBlock).StoredItems);
                                            containerOn = new Vector2(nx, ny);
                                            break;
                                        case SmartAction.ActionOpenClose:
                                            currentFloor_.OpenCloseDoor(nx, ny);
                                            break;
                                    }
                                }
                            }
                            else {
                                s += " (äàëåêî)";
                                if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                                    WindowContainer.Visible = false;
                                }
                            }

                            if (WindowIngameHint.Visible = a.Id != "0") {
                                LabelIngameHint.Text = s;
                                WindowIngameHint.Locate.Width = (int) LabelIngameHint.Width + 20;
                                WindowIngameHint.SetPosition(new Vector2(ms_.X + 10, ms_.Y + 10));
                            }
                        }
                    }
                }
            }
        }


        private Action<GameTime> DrawAction = x => { };
        private Label LabelMainVer;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (Settings.DebugInfo)
            {
                sw_draw.Restart();
            }

            GraphicsDevice.Clear(Color.Black);

            DrawAction(gameTime);

            base.Draw(gameTime);

            lineBatch_.Draw();
            lineBatch_.Clear();

            ws_.Draw(spriteBatch_, lig1, gameTime);

            if (Settings.DebugInfo)
            {
                sw_draw.Stop();
            }

            if (Settings.DebugInfo)
            {
                DebugInfoDraw(gameTime);
            }
        }

        private RenderTarget2D rt2d;
        private void GameDraw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(rt2d);
            currentFloor_.RenderFloors(camera_, GraphicsDevice);
            spriteBatch_.Begin();
            EffectOmnilight.Parameters["cpos"].SetValue(new[] { player_.Position.X - camera_.X, player_.Position.Y - camera_.Y });
            currentFloor_.DrawFloors(gameTime, camera_, GraphicsDevice);
            spriteBatch_.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                         DepthStencilState.None, RasterizerState.CullNone, EffectOmnilight);
            spriteBatch_.Draw(rt2d, Vector2.Zero, Color.White);
            spriteBatch_.End();

            GraphicsDevice.SetRenderTarget(rt2d);
            currentFloor_.ShadowRender();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                         DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch_.Draw(rt2d, Vector2.Zero, new Color(1, 1, 1, 0.5f));
            spriteBatch_.End();

            GraphicsDevice.SetRenderTarget(rt2d);
            spriteBatch_.Begin();
            currentFloor_.DrawBlocks(gameTime, camera_, player_);
            currentFloor_.DrawCreatures(gameTime, camera_);
            player_.Draw(gameTime, camera_);
            bs_.Draw(gameTime, camera_);
            ps_.Draw(gameTime, camera_);
            spriteBatch_.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch_.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, EffectOmnilight);
            spriteBatch_.Draw(rt2d, Vector2.Zero, Color.White);
            spriteBatch_.End();
        }

        private void DebugInfoDraw(GameTime gameTime)
        {
            FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int)Settings.Resolution.X,
                                  (int)Settings.Resolution.Y, sw_draw, sw_update);

            string ss =
                string.Format("SAng {0} \nPCount {1}   BCount {5}\nDT {3} WorldT {2} \nSectors {4} Generated {6} \nSTri {7}+{10} slen {8} {9}",
                              PlayerSeeAngle, ps_.Count(), GlobalWorldLogic.Temperature, GlobalWorldLogic.CurrentTime,
                              currentFloor_.SectorCount(), bs_.GetCount(), currentFloor_.generated, currentFloor_.GetShadowrenderCount() / 3, GlobalWorldLogic.GetCurrentSlen(), GlobalWorldLogic.dayPart_, currentFloor_.GetFloorRenderCount() / 3);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, ss, new Vector2(500, 10), Color.White);

            int nx = (int)((ms_.X + camera_.X) / 32.0);
            int ny = (int)((ms_.Y + camera_.Y) / 32.0);

            if (ms_.X + camera_.X < 0) nx--;
            if (ms_.Y + camera_.Y < 0) ny--;

            spriteBatch_.DrawString(font1_,string.Format("       {0} {1}", nx, ny), new Vector2(ms_.X, ms_.Y), Color.White);

            spriteBatch_.End();
        }
    }

    internal class DoubleLabel : Label {
        public string Text2;

        public DoubleLabel(Vector2 p, string s, Texture2D wp, SpriteFont wf, Color c, IGameContainer win) : base(p, s, wp, wf, c, win) {
        }

        public DoubleLabel(Vector2 p, string s, Texture2D wp, SpriteFont wf, IGameContainer win) : base(p, s, wp, wf, win) {
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Text != null && Visible)
            {
                if (isHudColored)
                {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.HudÑolor);
                    if (Text2 != null) {
                        sb.DrawString(font1_, Text2, Parent.GetPosition() + pos_ + new Vector2(Width, 0),
                                      Color.LightBlue);
                    }
                }
                else
                {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, col_);
                    if (Text2 != null) {
                        sb.DrawString(font1_, Text2, Parent.GetPosition() + pos_ + new Vector2(Width, 0), Color.LightBlue);
                    }
                }
            }
        }
    }
}