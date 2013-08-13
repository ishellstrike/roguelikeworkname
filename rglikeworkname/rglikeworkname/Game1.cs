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
    public class Game1 : Game
    {
        private WindowSystem ws_;
        private ParticleSystem ps_;
        private BulletSystem bs_;
        private GameLevel currentFloor_;
        private InventorySystem inventory_;
        
        private Player player_;

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
        private Button ButtonIngameExit;

        private Window WindowMainMenu;
        private Label LabelMainMenu;
        private Button ButtonNewGame;
        private Button ButtonSettings;
        private RunningLabel RunningMotd;
        private Label LabelControls;
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
        private Button IntentoryEquip;

        private Window WindowContainer;
        private ListContainer ContainerContainer;
        private LabelFixed LabelContainer;
        private Button ButtonContainerTakeAll;

        private Window WindowEventLog;
        private ListContainer ContainerEventLog;

        private Window WindowIngameHint;
        private Label LabelIngameHint;

        private Window WindowGlobal;
        private Image ImageGlobal;

        private Window WindowCaracter;
        private DoubleLabel LabelCaracterHat;
        private DoubleLabel LabelCaracterGlaces;
        private DoubleLabel LabelCaracterHelmet;
        private DoubleLabel LabelCaracterChest;
        private DoubleLabel LabelCaracterShirt;
        private DoubleLabel LabelCaracterPants;
        private DoubleLabel LabelCaracterGloves;
        private DoubleLabel LabelCaracterBoots;
        private DoubleLabel LabelCaracterGun;
        private DoubleLabel LabelCaracterMeele;
        private DoubleLabel LabelCaracterAmmo;
        private DoubleLabel LabelCaracterBag;
        private DoubleLabel LabelCaracterHp;

        private Window InfoWindow;
        private DoubleLabel InfoWindowLabel;
#endregion

        private void CreateWindows(Texture2D wp, SpriteFont sf, WindowSystem ws) {
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

            WindowMinimap = new Window(new Rectangle((int) Settings.Resolution.X - 180, 10, 128 + 20, 128 + 40), "minimap", true, wp, sf, ws) {Closable = false, hides = true};
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
            ButtonIngameExit = new Button(new Vector2(20, 100 + 30*3), "Exit game", wp, sf, WindowIngameMenu);
            ButtonIngameExit.onPressed += new EventHandler(ButtonIngameExit_onPressed);

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
            LabelControls = new Label(new Vector2(10, Settings.Resolution.Y/2 + 10), "I-inventory C-caracter page L-event log M-map WASD-moving LMB-shooting F1-debug info", wp, sf, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(LabelControls);
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
            IntentoryEquip = new Button(new Vector2(WindowInventory.Locate.Width - 100, WindowInventory.Locate.Height - 200 + 30*2), "Equip", wp, sf, WindowInventory);
            IntentoryEquip.onPressed += new EventHandler(IntentoryEquip_onPressed);

            WindowContainer = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Container", true, wp, sf, ws) { Visible = false };
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

            WindowGlobal = new Window(new Vector2(Settings.Resolution.X - 100, Settings.Resolution.Y - 50), "MAP", true, wp, sf, ws) {Visible = false};
            ImageGlobal = new Image(new Vector2(10,10), new Texture2D(GraphicsDevice, 10,10), Color.White, WindowGlobal);

            int ii = 0;
            WindowCaracter = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Container", true, wp, sf, ws) { Visible = false };
            LabelCaracterHat = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Hat : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterGlaces = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Glaces : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterHelmet = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Helmet : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterChest = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Chest Armor : ", wp, sf, WindowCaracter);ii++;
            LabelCaracterShirt = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Shirt : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterPants = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Pants : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterGloves = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Gloves : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterBoots = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Boots : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterGun = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Ranged Weapon : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterMeele = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Meele Weapon : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterAmmo = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Ammo : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterBag = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Bag : ", wp, sf, WindowCaracter);
            ii = 0;
            LabelCaracterHp = new DoubleLabel(new Vector2(10+300, 10 + 15 * ii), "HP : ", wp, sf, WindowCaracter);

            InfoWindow = new Window(new Vector2(200,100), "Info", true, wp, sf, ws){Visible = false};
            InfoWindowLabel = new DoubleLabel(new Vector2(20,20), "some info", wp, sf, InfoWindow);
        }

        void ShowInfoWindow(string s1, string s2) {
            InfoWindowLabel.Text = s1;
            InfoWindowLabel.Text2 = s2;
           // InfoWindow.CenterComponentHor(InfoWindowLabel);
            InfoWindow.Visible = true;
            InfoWindow.OnTop();
        }

        void HideInfoWindow()
        {
            InfoWindow.Visible = false;
        }

        void IntentoryEquip_onPressed(object sender, EventArgs e) {
            inventory_.UseItem(selectedItem, player_);
            UpdateInventoryContainer();
            selectedItem = null;
            InventoryMoreInfo.Text = "";
        }

        void ButtonIngameExit_onPressed(object sender, EventArgs e)
        {
            currentFloor_.SaveAll(this);
        }

        void ButtonContainerTakeAll_onPressed(object sender, EventArgs e)
        {
            inventory_.Items.AddRange(inContainer_);
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
                ContainerEventLog.AddItem(new LabelFixed(Vector2.Zero, ss.message, whitepixel_, font1_, ss.col, 35, ContainerEventLog));
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
                var i = new LabelFixed(Vector2.Zero, string.Format("{0} x{1}", ItemDataBase.Data[item.Id].Name, item.Count), 22, whitepixel_, font1_, ContainerInventoryItems);
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
                var i = new LabelFixed(Vector2.Zero, string.Format("{0} x{1}", ItemDataBase.Data[item.Id].Name, item.Count), 22, whitepixel_, font1_, ContainerContainer);
                i.Tag = cou;
                i.onPressed += PressInContainer;
                cou++;
                ContainerContainer.AddItem(i);
            }
        }

        private Item selectedItem;
        void PressInInventory(object sender, EventArgs e) {
            var a = (int) (sender as Label).Tag;
                selectedItem = inInv_[a];

            if (!doubleclick) {
                InventoryMoreInfo.Text = ItemDataBase.GetItemFullDescription(inInv_[a]);
            } else {
                IntentoryEquip_onPressed(null, null);
            }
        }

        private Item ContainerSelected;
        void PressInContainer(object sender, EventArgs e)
        {
            var a = (int)(sender as Label).Tag;
            if (inInv_.Count > a) {
                ContainerSelected = inContainer_[a];
                LabelContainer.Text = ItemDataBase.GetItemFullDescription(ContainerSelected);
                if(doubleclick) {
                    if (inContainer_.Contains(ContainerSelected)) {
                        inventory_.Items.Add(ContainerSelected);
                        inContainer_.Remove(ContainerSelected);
                        inventory_.StackSimilar();
                        UpdateInventoryContainer();
                        UpdateContainerContainer(inContainer_);
                    }
                }
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
            Process.Start("https://github.com/ishellstrike/roguelikeworkname/issues");
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

            if(currentFloor_ != null && WindowMinimap.Visible) {
                ImageMinimap.image = currentFloor_.GetMinimap();
            }

            if(WindowCaracter.Visible) {
                LabelCaracterGun.Text2 = player_.ItemGun != null ? ItemDataBase.Data[player_.ItemGun.Id].Name : "";
                LabelCaracterHat.Text2 = player_.ItemHat != null ? ItemDataBase.Data[player_.ItemHat.Id].Name : "";
                LabelCaracterHat.Text2 = player_.ItemAmmo != null ? ItemDataBase.Data[player_.ItemAmmo.Id].Name + " x" + player_.ItemAmmo.Count : "";

                LabelCaracterHp.Text2 = string.Format("{0}/{1}",player_.Hp.Current, player_.Hp.Max);
            }
        }
#endregion

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

            ws_ = new WindowSystem(whitepixel_, font1_);
            CreateWindows(whitepixel_, font1_, ws_);

            Action dbl = DataBasesLoadAndThenInitialGeneration;
            dbl.BeginInvoke(null, null);
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
            inventory_.Items.Add(new Item("testhat", 1));
            inventory_.Items.Add(new Item("testhat2", 1));
            inventory_.Items.Add(new Item("ak47", 1));
            inventory_.Items.Add(new Item("a762", 100));
            inventory_.Items.Add(new Item("a762", 100000));
            UpdateInventoryContainer();
            HideInfoWindow();
            sw.Stop();
            logger.Info("Initial generation in {0}",sw.Elapsed);
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
        }

        private void GameUpdate(GameTime gameTime) {
            sec += gameTime.ElapsedGameTime;
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

            if (!ws_.Mopusehook) {
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
                                            EventLog.Add("¬˚ ‚Ë‰ËÚÂ " + undermouseblock.Name,
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
                                s += " (‰‡ÎÂÍÓ)";
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
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.Hud—olor);
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