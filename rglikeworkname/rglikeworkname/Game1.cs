using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mork;
using System;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Parser;
using rglikeworknamelib.Dungeon.Particles;
using Settings = rglikeworknamelib.Settings;

namespace jarg
{
    public class Game1 : Game
    {
        private BlockDataBase bdb_;
        private FloorDataBase fdb_;
        private MonsterDataBase mdb_;
        private SchemesDataBase sdb_;
        private ItemDataBase idb_;
        private WindowSystem ws_;
        private ParticleSystem ps_;
        private BulletSystem bs_;

        private GraphicsDeviceManager graphics_;
        private Vector2 camera_;
        private GameLevel currentFloor_;
        private SpriteFont font1_;
        private KeyboardState ks_;
        private Vector2 lastPos_;
        private LineBatch lineBatch_;
        private KeyboardState lks_;
        private MouseState lms_;
        private MonsterSystem monsterSystem_;
        private MouseState ms_;
        private Vector2 pivotpoint_;
        private Player player_;
        private SpriteBatch spriteBatch_;

        private Texture2D whitepixel;

        //private Manager manager_;

        private GamePhase gamePhase_ = GamePhase.testgame;

        enum GamePhase
        {
            testgame
        }

        public Game1()
        {
            graphics_ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            if (!Directory.Exists(Settings.GetWorldsDirectory())) {
                Directory.CreateDirectory(Settings.GetWorldsDirectory());
            }
        }

        private void f_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            currentFloor_.SaveAll();
        }

        protected override void Initialize()
        {
            Log.Init();

            Assembly currentAssembly = Assembly.Load("jarg");
            var OurName = AssemblyName.GetAssemblyName(currentAssembly.Location).Name;
            var OurVer = "v" + AssemblyName.GetAssemblyName(currentAssembly.Location).Version;

            Window.Title = OurName + " " + OurVer;

            Settings.Resolution = new Vector2(1024, 768);
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
            graphics_.PreferredBackBufferWidth = (int)Settings.Resolution.X;
            graphics_.SynchronizeWithVerticalRetrace = false;
            
            IsFixedTimeStep = true; //wierd, actually it's true
            IsMouseVisible = true;
            graphics_.ApplyChanges();

            base.Initialize();

        }

#region Window Designer
#region Windows Vars

        private Window WindowStats;
        private ProgressBar StatsHunger;
        private ProgressBar StatsJajda;
        private ProgressBar StatsHeat;
        private Button CloseAllTestButton;

        private Window WindowMinimap;
        private Image ImageMinimap;

        private Window WindowSettings;
        private Label LabelHudColor;
        private Button ButtonHudColor1;
        private Button ButtonHudColor2;
        private Button ButtonHudColor3;
        private Button ButtonHudColor4;
        private Button ButtonHudColor5;

        private Window WindowIngameMenu;
        private Label LabelIngameMenu1;
        private Button ButtonIngameMenuSettings;
#endregion

        private void CreateWindows(Texture2D wp, SpriteFont sf, WindowSystem ws) {
            WindowStats = new Window(new Rectangle(50,50,400,400), "Stats", true, wp, sf, ws);
            StatsHeat = new ProgressBar(new Rectangle(50,50,100,20), "", wp, sf, WindowStats);
            StatsJajda = new ProgressBar(new Rectangle(50, 50 + 30, 100, 20), "", wp, sf, WindowStats);
            StatsHunger = new ProgressBar(new Rectangle(50, 50 + 30*2, 100, 20), "", wp, sf, WindowStats);
            CloseAllTestButton = new Button(new Vector2(10,100), "Close all", wp, sf, WindowStats);
            CloseAllTestButton.onPressed += CloseAllTestButton_onPressed;

            WindowMinimap = new Window(new Rectangle((int) Settings.Resolution.X - 180, 10, 100, 100), "minimap", true,
                                       wp, sf, ws) {NoBorder = true, Closable = false, Moveable = false};
            ImageMinimap = new Image(new Vector2(10,10), new Texture2D(GraphicsDevice, 88, 88), Color.White, WindowMinimap);

            WindowSettings =
                new Window(
                    new Rectangle(100, 100, (int) Settings.Resolution.X - 200, (int) Settings.Resolution.Y - 200),
                    "Settings", true, wp, sf, ws) {Visible = false};
            LabelHudColor = new Label(new Vector2(10,10), "HUD color", wp, sf, WindowSettings);
            ButtonHudColor1 = new Button(new Vector2(10 + 50 + 40 * 1, 10), "1", wp, sf, WindowSettings);
            ButtonHudColor1.onPressed += ButtonHudColor1_onPressed;
            ButtonHudColor2 = new Button(new Vector2(10 + 50 + 40 * 2, 10), "2", wp, sf, WindowSettings);
            ButtonHudColor2.onPressed += ButtonHudColor2_onPressed;
            ButtonHudColor3 = new Button(new Vector2(10 + 50 + 40 * 3, 10), "3", wp, sf, WindowSettings);
            ButtonHudColor3.onPressed += ButtonHudColor3_onPressed;
            ButtonHudColor4 = new Button(new Vector2(10 + 50 + 40 * 4, 10), "4", wp, sf, WindowSettings);
            ButtonHudColor4.onPressed += ButtonHudColor4_onPressed;
            ButtonHudColor5 = new Button(new Vector2(10 + 50 + 40 * 5, 10), "5", wp, sf, WindowSettings);
            ButtonHudColor5.onPressed += ButtonHudColor5_onPressed;

            WindowIngameMenu = new Window(new Vector2(300, 400), "Pause", true, wp, sf, ws) {Visible = false};
            ButtonIngameMenuSettings = new Button(new Vector2(20,100), "Settings", wp, sf, WindowIngameMenu);
            ButtonIngameMenuSettings.onPressed += ButtonIngameMenuSettings_onPressed;
        }

        void ButtonIngameMenuSettings_onPressed(object sender, EventArgs e) {
            WindowSettings.Visible = true;
            WindowSettings.OnTop();
        }

        private void ButtonHudColor3_onPressed(object sender, EventArgs e) {
            Settings.Hud—olor = Color.DarkGray;
        }

        private void ButtonHudColor5_onPressed(object sender, EventArgs e) {
            Settings.Hud—olor = Color.LightGreen;
        }

        private void ButtonHudColor4_onPressed(object sender, EventArgs e) {
            Settings.Hud—olor = Color.DarkOrange;
        }

        private void ButtonHudColor2_onPressed(object sender, EventArgs e) {
            Settings.Hud—olor = Color.LightGray;
        }

        void ButtonHudColor1_onPressed(object sender, EventArgs e) {
            Settings.Hud—olor = Color.White;
        }

        void CloseAllTestButton_onPressed(object sender, EventArgs e) {
            player_.hunger_--;
        }

        private void WindowsUpdate(GameTime gt) {

            if (WindowStats.Visible) {
                StatsHeat.Max = (int) player_.maxHeat_;
                StatsHeat.Progress = (int) player_.heat_;

                StatsJajda.Max = (int) player_.maxThirst_;
                StatsJajda.Progress = (int) player_.thirst_;

                StatsHunger.Max = (int) player_.maxHunger_;
                StatsHunger.Progress = (int) player_.hunger_;
            }

            if(WindowMinimap.Visible) {
                ImageMinimap.image = currentFloor_.GetMinimap();
            }
        }
#endregion

        private Texture2D itemSelectTex_;
        private Texture2D transparentPixel_;
        private ProgressBar fff;
        protected override void LoadContent()
        {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            whitepixel = new Texture2D(graphics_.GraphicsDevice, 1, 1);
            var data = new uint[1];
            data[0] = 0xffffffff;
            whitepixel.SetData(data);

            itemSelectTex_ = Content.Load<Texture2D>(@"Textures/Dungeon/Items/itemselect");
            transparentPixel_ = Content.Load<Texture2D>(@"Textures/transparent_pixel");
            font1_ = Content.Load<SpriteFont>(@"Fonts/Font1");

            bdb_ = new BlockDataBase();
            fdb_ = new FloorDataBase();
            mdb_ = new MonsterDataBase();
            sdb_ = new SchemesDataBase();
            idb_ = new ItemDataBase(/*ParsersCore.LoadTexturesInOrder(rglikeworknamelib.Settings.GetItemDataDirectory() + @"/textureloadorder.ord", Content))*/);
            ws_ = new WindowSystem(whitepixel, font1_);

            ps_ = new ParticleSystem(spriteBatch_, ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content));

            bs_ = new BulletSystem(spriteBatch_, ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content), ps_);

            var tex = new Collection<Texture2D> {
                                           Content.Load<Texture2D>(@"Textures/transparent_pixel"),
                                           Content.Load<Texture2D>(@"Textures/Units/car")
                                        };
            monsterSystem_ = new MonsterSystem(spriteBatch_, tex);

            Window bbb = new Window(new Rectangle(10, 10, 200, 200), "Info", true, whitepixel, font1_, ws_);
            Label ccc = new Label(new Vector2(20,20), "qwerr", whitepixel, font1_, bbb);
            Image ddd = new Image(new Vector2(40,40), tex[1], Color.Red, bbb);
            fff = new ProgressBar(new Rectangle(10,50,100,20), "Progress", whitepixel, font1_, bbb);
            CreateWindows(whitepixel, font1_, ws_);


            currentFloor_ = new GameLevel(spriteBatch_, 
                                                      ParsersCore.LoadTexturesInOrder(Settings.GetFloorDataDirectory() + @"/textureloadorder.ord", Content),
                                                      ParsersCore.LoadTexturesInOrder(Settings.GetObjectDataDirectory() + @"/textureloadorder.ord", Content), 
                                                      font1_,
                                                      bdb_, 
                                                      fdb_, 
                                                      sdb_
                                                     );

            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_);
        }

        protected override void UnloadContent()
        {
           
        }

        public static bool ErrorExit;
        public float seeAngleDeg = 60;
        public float PlayerSeeAngle;
        public TimeSpan sec = TimeSpan.Zero;
        protected override void Update(GameTime gameTime) {

            if (ErrorExit) Exit();

            base.Update(gameTime);

            KeyboardUpdate(gameTime);
            MouseUpdate(gameTime);

            sec += gameTime.ElapsedGameTime;
            if (sec >= TimeSpan.FromSeconds(0.5)) {
                sec = TimeSpan.Zero;

                currentFloor_.GenerateMinimap(GraphicsDevice, player_);
            }


            lastPos_ = player_.Position;
            if ((int)player_.Position.X / 3 == (int)player_.LastPos.X / 3 && (int)player_.Position.Y / 3 == (int)player_.LastPos.Y / 3) {
                if (seeAngleDeg < 150) {
                    seeAngleDeg += (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
                } else {
                    seeAngleDeg = 150;
                }
            } else {
                if (seeAngleDeg > 60) {
                    seeAngleDeg -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2000;
                } else {
                    seeAngleDeg = 60;
                }
            }
            currentFloor_.CalcWision(player_, (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X), seeAngleDeg);
            player_.Update(gameTime, currentFloor_, bdb_);
            currentFloor_.KillFarSectors(player_);
            ps_.Update(gameTime);
            bs_.Update(gameTime);
            GlobalWorldLogic.Update(gameTime);

            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);

            if (Settings.DebugInfo) {
                FrameRateCounter.Update(gameTime);
            }

            WindowsUpdate(gameTime);
            ws_.Update(gameTime, ms_, lms_);
            PlayerSeeAngle = (float) Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X);
        }

        private void KeyboardUpdate(GameTime gameTime)
        {
            lks_ = ks_;
            ks_ = Keyboard.GetState();

            if (ks_[Keys.F1] == KeyState.Down && lks_[Keys.F1] == KeyState.Up) {
                Settings.DebugInfo = !Settings.DebugInfo;
            }

            if (ks_[Keys.F2] == KeyState.Down)
            {
                currentFloor_.SetFloor(Settings.rnd.Next(-10000, 10000), Settings.rnd.Next(-10000, 10000), 1);
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
                if(!ws_.CloseTop()) {
                    WindowIngameMenu.Visible = true;
                }

            }

            pivotpoint_ = new Vector2(player_.Position.X - (Settings.Resolution.X - 200) / 2, player_.Position.Y - Settings.Resolution.Y / 2);

        }

        private void MouseUpdate(GameTime gameTime)
        {
            lms_ = ms_;
            ms_ = Mouse.GetState();

            if(ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                bs_.AddBullet(player_, 5, PlayerSeeAngle);
            }

            //if (aa >= 0 && currentFloor_.GetId(aa) != 0 )// currentFloor_.IsExplored(aa))
            {
                int nx = (ms_.X + (int)camera_.X) / 32;
                int ny = (ms_.Y + (int)camera_.Y) / 32;

                if (ms_.X + camera_.X < 0) nx--;
                if (ms_.Y + camera_.Y < 0) ny--;

                var a = currentFloor_.GetBlock(nx, ny);
                if (a != null) {
                    var b = bdb_.Data[a.Id];
                    string s = Block.GetSmartActionName(b.SmartAction) + " " + b.Name;
                    if (rglikeworknamelib.Settings.DebugInfo) s += " id" + a.Id + " tex" + b.MTex;

                    if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released &&
                        currentFloor_.IsCreatureMeele(nx, ny, player_)) {
                        var undermouseblock = bdb_.Data[a.Id];
                        if (undermouseblock.SmartAction == SmartAction.ActionOpenContainer) {
                        }

                        if (undermouseblock.SmartAction == SmartAction.ActionOpenClose) {
                            currentFloor_.OpenCloseDoor(nx, ny);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch_.Begin();
            currentFloor_.Draw(gameTime, camera_);
            currentFloor_.Draw2(gameTime, camera_);
            player_.Draw(gameTime, camera_);
            ps_.Draw(gameTime, camera_);
            bs_.Draw(gameTime, camera_);
            spriteBatch_.End();
            
            ws_.Draw(spriteBatch_);

            base.Draw(gameTime);

            if (Settings.DebugInfo) {
                DebugInfoDraw(gameTime);
            }

            lineBatch_.Draw();
            lineBatch_.Clear();
        }

        private void DebugInfoDraw(GameTime gameTime)
        {
            FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int)Settings.Resolution.X,
                                  (int)Settings.Resolution.Y);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, string.Format("SAng {0} \nPCount {1}   BCount {5}\nDT {3} WorldT {2} \nSectors {4} Generated {6}", PlayerSeeAngle, ps_.Count(), GlobalWorldLogic.temperature, GlobalWorldLogic.currentTime, currentFloor_.SectorCount(), bs_.GetCount(), currentFloor_.generated), new Vector2(500, 10), Color.White);

            int nx = (int)((ms_.X + camera_.X) / 32.0);
            int ny = (int)((ms_.Y + camera_.Y) / 32.0);

            if (ms_.X + camera_.X < 0) nx--;
            if (ms_.Y + camera_.Y < 0) ny--;

            spriteBatch_.DrawString(font1_,string.Format("       {0} {1}", nx, ny), new Vector2(ms_.X, ms_.Y), Color.White);

            spriteBatch_.End();
        }
    }
}