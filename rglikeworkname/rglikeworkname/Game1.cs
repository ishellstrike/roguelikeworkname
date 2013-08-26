using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
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
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Parser;
using rglikeworknamelib.Dungeon.Particles;
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
    public class Game1 : Game
    {
        private rglikeworknamelib.Window.WindowSystem ws_;
        private ParticleSystem ps_;
        private BulletSystem bs_;
        private GameLevel currentFloor_;
        private InventorySystem inventory_;
        
        private Player player_;

        private GraphicsDeviceManager graphics_;
        private Vector2 camera_;
        private SpriteFont font1_;
        private KeyboardState ks_;
        private Vector2 lastPos_;
        private LineBatch lineBatch_;
        private KeyboardState lks_;
        private MouseState lms_;
        private MouseState ms_;
        private Vector2 pivotpoint_;
        private SpriteBatch spriteBatch_;

        private Texture2D whitepixel;

        //private Manager manager_;

        private GamePhase gamePhase_ = GamePhase.testgame;

        enum GamePhase
        {
            testgame
        }

        public Game1() {
#if DEBUG

            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = @"/C cd " + Application.StartupPath + " & VersionGetter.cmd";
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            myProcess.WaitForExit(3000);
#endif

            if (File.Exists("JARGErrorLog_1.txt"))
            {
                File.Delete("JARGErrorLog_1.txt");
            }
            if (File.Exists("JARGErrorLog.txt"))
            {
                File.Move("JARGErrorLog.txt", "JARGErrorLog_1.txt");
            }

            Version.Init();

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
            Window.Title = Version.GetLong();

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
        private ListContainer contaiter1;

        private Window WindowMinimap;
        private Image ImageMinimap;

        private Window WindowSettings;
        private Label LabelHudColor;
        private Button ButtonHudColor1;
        private Button ButtonHudColor2;
        private Button ButtonHudColor3;
        private Button ButtonHudColor4;
        private Button ButtonHudColor5;
        private Label LabelTimeType;
        private Button Button12h, Button24h;

        private Window WindowIngameMenu;
        private Label LabelIngameMenu1;
        private Button ButtonIngameMenuSettings;

        private Window WindowMainMenu;
        private Label LabelMainMenu;
        private Button ButtonNewGame;
        private Button ButtonSettings;
        private RunningLabel RunningMotd;
        private Button ButtonOpenGit;

        private Window WindowCaracterCration;
        private Button ButtonCaracterConfirm;
        private Button ButtonCaracterCancel;

        private Window WindowPickup;

        private Window WindowInventory;
        private ListContainer ContainerInventoryItems;
        private LabelFixed InventoryMoreInfo;
        private Button InventorySortAll;
        private Button InventorySortMedicine;
        private Button InventorySortFood;

        private Window WindowContainer;
        private ListContainer ContainerContainer;
        private LabelFixed LabelContainer;
        private Button ButtonContainerTakeAll;

        private Window WindowEventLog;
        private ListContainer ContainerEventLog;

        private Window WindowIngameHint;
        private Label LabelIngameHint;
#endregion

        private void CreateWindows(Texture2D wp, SpriteFont sf, rglikeworknamelib.Window.WindowSystem ws) {
            Random rnd = new Random();

            WindowStats = new Window(new Rectangle(50, 50, 400, 400), "Stats", true, wp, sf, ws) { Visible = false };
            StatsHeat = new ProgressBar(new Rectangle(50,50,100,20), "", wp, sf, WindowStats);
            StatsJajda = new ProgressBar(new Rectangle(50, 50 + 30, 100, 20), "", wp, sf, WindowStats);
            StatsHunger = new ProgressBar(new Rectangle(50, 50 + 30*2, 100, 20), "", wp, sf, WindowStats);
            CloseAllTestButton = new Button(new Vector2(10,100), "Close all", wp, sf, WindowStats);
            CloseAllTestButton.onPressed += CloseAllTestButton_onPressed;
            contaiter1 = new ListContainer(new Rectangle(200,200,100,200), wp, sf, WindowStats);
            for (int i = 1; i < 20; i++ )
                contaiter1.AddItem(new Button(Vector2.Zero, rnd.Next(1, 1000).ToString(), wp, sf, WindowStats));

            WindowMinimap = new Window(new Rectangle((int) Settings.Resolution.X - 180, 10, 128 + 20, 128 + 40), "minimap", true,
                                       wp, sf, ws) {Closable = false, hides = true};
            ImageMinimap = new Image(new Vector2(10,10), new Texture2D(GraphicsDevice, 88, 88), Color.White, WindowMinimap);

            WindowSettings =
                new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y),
                    "Settings", true, wp, sf, ws) {Visible = false, Moveable = false};
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
            LabelHudColor = new Label(new Vector2(10, 10 + 40 * 1), "Time format", wp, sf, WindowSettings);
            Button12h = new Button(new Vector2(10 + 50 + 40 * 2, 10 + 40 * 1), "12h", wp, sf, WindowSettings);
            Button12h.onPressed += Button12h_onPressed;
            Button24h = new Button(new Vector2(10 + 50 + 40 * 3, 10 + 40 * 1), "24h", wp, sf, WindowSettings);
            Button24h.onPressed += Button24h_onPressed;      

            WindowIngameMenu = new Window(new Vector2(300, 400), "Pause", true, wp, sf, ws) {Visible = false};
            ButtonIngameMenuSettings = new Button(new Vector2(20,100), "Settings", wp, sf, WindowIngameMenu);
            ButtonIngameMenuSettings.onPressed += ButtonIngameMenuSettings_onPressed;

            WindowMainMenu = new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y), "MAIN MENU",
                                        false, wp, sf, ws) {NoBorder = true, Moveable = false};
            LabelMainMenu = new Label(new Vector2(10, 10), @"     __                      
    |__|____ _______  ____  
    |  \__  \\_  __ \/ ___\ 
    |  |/ __ \|  | \/ /_/  >
/\__|  (____  /__|  \___  / 
\______|    \/     /_____/", wp, sf, WindowMainMenu);
            LabelMainVer = new Label(new Vector2(10, LabelMainMenu.Height + 10), Version.GetShort(), wp, sf, Color.Gray, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(LabelMainMenu);
            WindowMainMenu.CenterComponentHor(LabelMainVer);
            ButtonNewGame = new Button(new Vector2(10,120 + 40*1), "New game", wp, sf, WindowMainMenu);
            ButtonNewGame.onPressed += ButtonNewGame_onPressed;
            WindowMainMenu.CenterComponentHor(ButtonNewGame);

            ButtonSettings = new Button(new Vector2(10, 100 + 40 * 5), "Settings", wp, sf, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(ButtonSettings);
            ButtonSettings.onPressed += ButtonIngameMenuSettings_onPressed;
            RunningMotd = new RunningLabel(new Vector2(10, Settings.Resolution.Y / 2 - 50), "Jarg now in early development. It's tottaly free and opensource. Please send your suggestions to ishellstrike@gmail.com or github.com/ishellstrike/roguelikeworkname/issues.", 50, wp, sf, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(RunningMotd);
            ButtonOpenGit = new Button(new Vector2(10, Settings.Resolution.Y / 2 - 20), "Open in browser", wp, sf, WindowMainMenu);
            ButtonOpenGit.onPressed += ButtonOpenGit_onPressed;
            WindowMainMenu.CenterComponentHor(ButtonOpenGit);

            WindowCaracterCration = new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y), "CARACTER CREATION",
                                        false, wp, sf, ws) { NoBorder = true, Moveable = false,Visible = false};
            ButtonCaracterConfirm = new Button(new Vector2(Settings.Resolution.X / 4*2, Settings.Resolution.Y / 2 - 20), "Continue", wp, sf, WindowCaracterCration);
            ButtonCaracterConfirm.onPressed += ButtonCaracterConfirm_onPressed;
            ButtonCaracterCancel = new Button(new Vector2(0, Settings.Resolution.Y / 2 - 20), "Cancel", wp, sf, WindowCaracterCration);
            ButtonCaracterCancel.onPressed += ButtonCaracterCancel_onPressed;

            WindowInventory = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Inventory", true, wp, sf, ws) { Visible = false };
            ContainerInventoryItems = new ListContainer(new Rectangle(10, 10, WindowInventory.Locate.Width / 2, WindowInventory.Locate.Height - 40), wp, sf, WindowInventory);
            InventoryMoreInfo = new LabelFixed(new Vector2(WindowInventory.Locate.Width - 200, 40), "", 20, wp, sf, WindowInventory);
            InventorySortAll = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200), "All", wp, sf, WindowInventory);
            InventorySortAll.onPressed += InventorySortAll_onPressed;
            InventorySortMedicine = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30), "Medicine", wp, sf, WindowInventory);
            InventorySortMedicine.onPressed += InventorySortMedicine_onPressed;
            InventorySortFood = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30*2), "Food", wp, sf, WindowInventory);
            InventorySortFood.onPressed += InventorySortFood_onPressed;

            WindowContainer = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Inventory", true, wp, sf, ws) { Visible = false };
            WindowContainer.SetPosition(new Vector2(Settings.Resolution.X/2, 0));
            ContainerContainer = new ListContainer(new Rectangle(10, 10, WindowInventory.Locate.Width / 2, WindowInventory.Locate.Height - 40), wp, sf, WindowContainer);
            LabelContainer = new LabelFixed(new Vector2(WindowInventory.Locate.Width - 200, 40), "", 20, wp, sf, WindowContainer);
            ButtonContainerTakeAll = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30 * 2), "Take All (R)", wp, sf, WindowContainer);
            ButtonContainerTakeAll.onPressed += ButtonContainerTakeAll_onPressed;

            WindowEventLog = new Window(new Rectangle(3,570,(int)Settings.Resolution.X / 3, (int)Settings.Resolution.Y / 4), "Log", true, wp, sf, ws_) { Visible = false, Closable = false, hides = true};
            ContainerEventLog = new ListContainer( new Rectangle(0,0,(int)Settings.Resolution.X/3, (int)Settings.Resolution.Y/4-20), wp, sf, WindowEventLog);
            EventLog.onLogUpdate += EventLog_onLogUpdate;

            WindowIngameHint = new Window(new Vector2(50, 50), "HINT", false, wp, sf, ws) {NoBorder = true};
            LabelIngameHint = new Label(new Vector2(10,3), "a-ha", wp, sf, WindowIngameHint);
        }

        void ButtonContainerTakeAll_onPressed(object sender, EventArgs e)
        {
            inventory_.items.AddRange(inContainer_);
            inContainer_.Clear();
            inventory_.StackSimilar();
            UpdateContainerContainer(inContainer_);
            UpdateInventoryContainer();
        }

        void Button24h_onPressed(object sender, EventArgs e) {
            Settings.IsAMDM = false;
        }

        void Button12h_onPressed(object sender, EventArgs e) {
            Settings.IsAMDM = true;
        }

        void EventLog_onLogUpdate(object sender, EventArgs e)
        {
            ContainerEventLog.Clear();
            int i = 0;
            foreach (var ss in EventLog.log) {
                ContainerEventLog.AddItem(new LabelFixed(Vector2.Zero, ss.message, whitepixel, font1_, ss.col, 35, ContainerEventLog));
                i++;
            }
            ContainerEventLog.ScrollBottom();
        }

        void InventorySortFood_onPressed(object sender, EventArgs e)
        {
            nowSort_ = ItemType.Food;
            UpdateInventoryContainer();
        }

        void InventorySortMedicine_onPressed(object sender, EventArgs e)
        {
            nowSort_ = ItemType.Medicine;
            UpdateInventoryContainer();
        }

        void InventorySortAll_onPressed(object sender, EventArgs e)
        {
            nowSort_ = ItemType.Nothing;
            UpdateInventoryContainer();
        }

        private ItemType nowSort_ = ItemType.Nothing;
        private List<Item> inInv_ = new List<Item>(); 
        void UpdateInventoryContainer() {
            var a = inventory_.FilterByType(nowSort_);
            inInv_ = a;

            ContainerInventoryItems.Clear();

            int cou = 0;
            foreach (var item in a) {
                var i = new LabelFixed(Vector2.Zero, string.Format("{0} x{1}", ItemDataBase.data[item.Id].name, item.Count), 22, whitepixel, font1_, ContainerInventoryItems);
                i.Tag = cou;
                i.onPressed += PressInInventory;
                cou++;
                ContainerInventoryItems.AddItem(i);
            }
        }

        private ItemType nowSortContainer_ = ItemType.Nothing;
        private List<Item> inContainer_ = new List<Item>();
        private Vector2 containerOn = new Vector2();
        void UpdateContainerContainer(List<Item> a)
        {
            inContainer_ = a;

            ContainerContainer.Clear();

            int cou = 0;
            foreach (var item in a)
            {
                var i = new LabelFixed(Vector2.Zero, string.Format("{0} x{1}", ItemDataBase.data[item.Id].name, item.Count), 22, whitepixel, font1_, ContainerContainer);
                i.Tag = cou;
                i.onPressed += PressInContainer;
                cou++;
                ContainerContainer.AddItem(i);
            }
        }

        void PressInInventory(object sender, EventArgs e) {
            var a = (int) (sender as Label).Tag;
            InventoryMoreInfo.Text = ItemDataBase.GetItemFullDescription(inInv_[a]);
        }

        void PressInContainer(object sender, EventArgs e)
        {
            var a = (int)(sender as Label).Tag;
            if (inInv_.Count > a) {
                LabelContainer.Text = ItemDataBase.GetItemFullDescription(inInv_[a]);
            }
        }

        void ButtonCaracterCancel_onPressed(object sender, EventArgs e)
        {
            WindowMainMenu.Visible = true;
            WindowCaracterCration.Visible = false;
        }

        void ButtonCaracterConfirm_onPressed(object sender, EventArgs e) {
            WindowCaracterCration.Visible = false;
            DrawAction = GameDraw;
            UpdateAction = GameUpdate;
        }

        void ButtonNewGame_onPressed(object sender, EventArgs e) {
            WindowMainMenu.Visible = false;
            WindowCaracterCration.Visible = true;
        }

        void ButtonOpenGit_onPressed(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/ishellstrike/roguelikeworkname/issues");
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
            player_.Hunger.Current--;
        }

        private void WindowsUpdate(GameTime gt) {

            if (WindowStats.Visible) {
                StatsHeat.Max = (int) player_.Heat.Max;
                StatsHeat.Progress = (int) player_.Heat.Current;

                StatsJajda.Max = (int) player_.Thirst.Max;
                StatsJajda.Progress = (int) player_.Thirst.Current;

                StatsHunger.Max = (int) player_.Hunger.Max;
                StatsHunger.Progress = (int) player_.Heat.Current;
            }

            if(WindowMinimap.Visible) {
                ImageMinimap.image = currentFloor_.GetMinimap();
            }
        }
#endregion

        protected override void LoadContent()
        {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            Atlases a = new Atlases(Content);

            whitepixel = new Texture2D(graphics_.GraphicsDevice, 1, 1);
            var data = new uint[1];
            data[0] = 0xffffffff;
            whitepixel.SetData(data);

            MonsterDataBase mdb = new MonsterDataBase();
            BlockDataBase bdb = new BlockDataBase();
            FloorDataBase fdb = new FloorDataBase();
            ItemDataBase idb = new ItemDataBase();
            SchemesDataBase sdb = new SchemesDataBase();

            font1_ = Content.Load<SpriteFont>(@"Fonts/Font1");
            Settings.Font = font1_;

            currentFloor_ = new GameLevel(spriteBatch_, font1_, GraphicsDevice);

            ws_ = new WindowSystem(whitepixel, font1_);

            ps_ = new ParticleSystem(spriteBatch_, ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content));

            bs_ = new BulletSystem(spriteBatch_, ParsersCore.LoadTexturesInOrder(Settings.GetParticleTextureDirectory() + @"/textureloadorder.ord", Content), currentFloor_, font1_, lineBatch_);

            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_);

            inventory_ = new InventorySystem();

            CreateWindows(whitepixel, font1_, ws_);
            
            UpdateInventoryContainer();
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

            if(Settings.DebugInfo) {
                sw_update.Restart();
            }

            if (ErrorExit) Exit();

            base.Update(gameTime);

            KeyboardUpdate(gameTime);
            MouseUpdate(gameTime);

            UpdateAction(gameTime);

            if (Settings.DebugInfo) {
                FrameRateCounter.Update(gameTime);
            }

            WindowsUpdate(gameTime);
            ws_.Update(gameTime, ms_, lms_, false);
            PlayerSeeAngle = (float) Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X);

            if (Settings.DebugInfo)
            {
                sw_update.Stop();
            }
        }

        private void GameUpdate(GameTime gameTime) {
            sec += gameTime.ElapsedGameTime;
            if (sec >= TimeSpan.FromSeconds(0.2)) {
               sec = TimeSpan.Zero;

               currentFloor_.GenerateMinimap(GraphicsDevice, spriteBatch_, player_);
            }


            lastPos_ = player_.Position;
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
            player_.Update(gameTime, currentFloor_);
            currentFloor_.KillFarSectors(player_, gameTime);
            ps_.Update(gameTime);
            bs_.Update(gameTime);
            currentFloor_.UpdateBlocks(gameTime, camera_);
            GlobalWorldLogic.Update(gameTime);

            currentFloor_.UpdateCreatures(gameTime, player_);

            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float) gameTime.ElapsedGameTime.TotalSeconds*4);
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
                if (!ws_.CloseTop()) {
                    WindowIngameMenu.Visible = true;
                }
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

            if (WindowContainer.Visible && ks_[Keys.R] == KeyState.Down && lks_[Keys.R] == KeyState.Up)
            {
                ButtonContainerTakeAll_onPressed(null, null);
            }

            pivotpoint_ = new Vector2(player_.Position.X - (Settings.Resolution.X - 200) / 2, player_.Position.Y - Settings.Resolution.Y / 2);

        }

        private TimeSpan sec_shoot = TimeSpan.Zero;
        private void MouseUpdate(GameTime gameTime)
        {
            lms_ = ms_;
            ms_ = Mouse.GetState();
            sec_shoot += gameTime.ElapsedGameTime;

            if(ms_.LeftButton == ButtonState.Pressed && sec_shoot.TotalSeconds > 0.2f) {
                bs_.AddBullet(player_, 50, PlayerSeeAngle);
                sec_shoot = TimeSpan.Zero;
            }

            int nx = (ms_.X + (int)camera_.X) / 32;
            int ny = (ms_.Y + (int)camera_.Y) / 32;

            if (ms_.X + camera_.X < 0) nx--;
            if (ms_.Y + camera_.Y < 0) ny--;

            WindowIngameHint.Visible = false;

            if(!currentFloor_.IsCreatureMeele((int)containerOn.X, (int)containerOn.Y, player_)) {
                WindowContainer.Visible = false;
            }

            var nxny = currentFloor_.GetBlock(nx, ny);
            if (nxny != null && nxny.Lightness == Color.White)// currentFloor_.IsExplored(aa))
            {
                var a = currentFloor_.GetBlock(nx, ny);
                if (a != null)
                {
                    var b = BlockDataBase.Data[a.Id];
                    string s = Block.GetSmartActionName(b.SmartAction) + " " + b.Name;
                    if (Settings.DebugInfo) s += " id" + a.Id + " tex" + b.MTex;

                    if (currentFloor_.IsCreatureMeele(nx, ny, player_))
                    {
                        if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released)
                        {
                            var undermouseblock = BlockDataBase.Data[a.Id];
                            switch (undermouseblock.SmartAction)
                            {
                                case SmartAction.ActionSee:
                                    EventLog.Add("¬˚ ‚Ë‰ËÚÂ " + undermouseblock.Name, GlobalWorldLogic.CurrentTime,
                                                 Color.Gray, LogEntityType.SeeSomething);
                                    break;
                                case SmartAction.ActionOpenContainer:
                                    WindowContainer.Visible = true;
                                    WindowContainer.SetPosition(new Vector2(Settings.Resolution.X / 2, 0));
                                    UpdateContainerContainer((a as StorageBlock).StoredItems);
                                    containerOn = new Vector2(nx, ny);
                                    break;
                                case SmartAction.ActionOpenClose:
                                    currentFloor_.OpenCloseDoor(nx, ny);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        s += " (‰‡ÎÂÍÓ)";
                        if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                            WindowContainer.Visible = false;
                        }
                    }

                    if (WindowIngameHint.Visible = a.Id != "0")
                    {
                        LabelIngameHint.Text = s;
                        WindowIngameHint.Locate.Width = (int)LabelIngameHint.Width + 20;
                        WindowIngameHint.SetPosition(new Vector2(ms_.X + 10, ms_.Y + 10));
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

            ws_.Draw(spriteBatch_);

            if (Settings.DebugInfo)
            {
                sw_draw.Stop();
            }

            if (Settings.DebugInfo)
            {
                DebugInfoDraw(gameTime);
            }
        }

        private void GameDraw(GameTime gameTime) {
            spriteBatch_.Begin();
                currentFloor_.DrawFloors(gameTime, camera_);
                
            spriteBatch_.End();
            currentFloor_.ShadowRender();
            spriteBatch_.Begin();
                currentFloor_.DrawBlocks(gameTime, camera_, player_);
                currentFloor_.DrawCreatures(gameTime, camera_);
                player_.Draw(gameTime, camera_);
                bs_.Draw(gameTime, camera_);
                ps_.Draw(gameTime, camera_);
            spriteBatch_.End();
        }

        private void DebugInfoDraw(GameTime gameTime)
        {
            FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int)Settings.Resolution.X,
                                  (int)Settings.Resolution.Y, sw_draw, sw_update);

            string ss =
                string.Format("SAng {0} \nPCount {1}   BCount {5}\nDT {3} WorldT {2} \nSectors {4} Generated {6} \nSTri {7}",
                              PlayerSeeAngle, ps_.Count(), GlobalWorldLogic.Temperature, GlobalWorldLogic.CurrentTime,
                              currentFloor_.SectorCount(), bs_.GetCount(), currentFloor_.generated, currentFloor_.GetShadowrenderCount()/3);
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
}