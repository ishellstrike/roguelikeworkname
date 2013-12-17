using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rglikeworknamelib;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Items.Subclases;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Window;
using EventLog = rglikeworknamelib.EventLog;
using IGameComponent = rglikeworknamelib.Window.IGameComponent;

namespace jarg {
    public partial class JargMain {
        #region Windows Vars

        private Window BigLogWindow;
        private ListContainer BigLogContainer;
        private Button ShowBigLogWindow;

        private Window LookWindow;
        private ListContainer LookContainer;

        private Window MoraleWindow;
        private ListContainer MoraleContainer;

        private Window AchievementsWindow;
        private ListContainer AchievementContainer;

        private Button SpawnItemButton;

        private Window SpawnSomeWindow;
        private ListContainer SpawnSomeList;

        private ListContainer SchemesExist;

        private Window InventoryDropDownWindow;
        private ListContainer InventoryDropDownContainer;

        private Button Button12h, Button24h;
        private Button ButtonCaracterCancel;
        private Button ButtonCaracterConfirm;
        private Button ButtonContainerTakeAll;
        private Button ButtonFramelimitOff;
        private Button ButtonFramelimitOn;
        private Button ButtonFullscreenOff;
        private Button ButtonFullscreenOn;
        private Button ButtonHudColor1, ButtonHudColor2, ButtonHudColor3, ButtonHudColor4, ButtonHudColor5;
        private Button ButtonIngameExit;
        private Button ButtonIngameMenuSettings;
        private Button ButtonLightOff;
        private Button ButtonLightOn1, ButtonLightOn2, ButtonLightOn4, ButtonLightOn8, ButtonLightOn12;
        private Button ButtonNewGame;
        private Button ButtonOpenGit;
        private Button ButtonRadioGB, ButtonRadioOff;
        private Button DeleteLastWorldButton;

        private Button ButtonResolution1024768,
                       ButtonResolution1280800,
                       ButtonResolution19201024,
                       ButtonResolution800600;

        private Button ButtonSettings;
        private Button CloseAllTestButton;
        private TextBox ConsoleTB;
        private Window ConsoleWindow;
        private ListContainer ContainerContainer;
        private ListContainer ContainerEventLog;
        private ListContainer ContainerInventoryItems;
        private ListContainer ContainerWearList;
        private ListContainer EffectsContainer;
        private ImageButton IBBag;
        private ImageButton IBCaracter;
        private ImageButton IBInv;
        private Image ImageGlobal;
        private Image ImageMinimap;
        private Window InfoWindow;
        private DoubleLabel InfoWindowLabel;
        private Button IntentoryEquip;
        private LabelFixed InventoryMoreInfo;
        private Button InventorySortAll;
        private Button InventorySortFood;
        private Button InventorySortMedicine;
        private Label InventoryTotalWV;

        private DoubleLabel LabelCaracterAmmo;
        private DoubleLabel LabelCaracterGun;
        private DoubleLabel LabelCaracterHp;
        private DoubleLabel LabelCaracterMeele;
        private LabelFixed LabelContainer;
        private Label LabelControls;
        private Label LabelFramelimit;
        private Label LabelFullscreen;
        private Label LabelHudColor;
        private Label LabelIngameHint;
        private Label LabelIngameMenu1;
        private Label LabelLight;
        private Label LabelMainMenu;
        private Label LabelOnlineRadio;
        private RunningLabel LabelRadio;
        private Label LabelResolution;
        private Label LabelTimeType;
        private Label LabelWearCaption;
        private List<Label> LabelsAbilities;
        private ListContainer StatistList;
        private Button ModLoaderButton;

        private ListContainer ModLoaderContainer;
        private Window ModLoaderWindow;
        private ListContainer PerksContainer;
        private RunningLabel RunningMotd;
        private ProgressBar StatsHeat;
        private ProgressBar StatsHunger;
        private ProgressBar StatsJajda;
        private Window CaracterWindow;
        private Window WindowCaracterCration;
        private Window WindowContainer;
        private Window EventLogWindow;
        private Window WindowGlobal;
        private Window WindowIngameHint;
        private Window WindowIngameMenu;
        private Window InventoryWindow;
        private Window WindowMainMenu;
        private Window WindowMinimap;
        private Window WindowRadio;
        private Window WindowSettings;
        private Window StatistWindow;
        private Window WindowStats;
        private Window WindowUIButtons;
        private ListContainer contaiter1;

        private Window CraftWindow;
        private Button CraftSortAll;
        private ListContainer CraftItems;
        private LabelFixed CraftMoreInfo;
        private Button CraftThisButton;

        private Window SchemesEditorWindow;
        private Image[,] SchemesImages;
        private ListContainer SchemesContainer;
        private Button SchemesLoadButton, SchemesSaveButton, SchemesClearButton;
        private Label SchemesInfoLabel;
        private Button SchemesSave;
        

        #endregion

        private IItem ContainerSelected;
        private TimeSpan SecondTimespan;
        private List<IItem> inContainer_ = new List<IItem>();
        private List<IItem> inInv_ = new List<IItem>();
        private ItemType nowSort_ = ItemType.Nothing;
        private Item selectedItem;

        private void CreateWindows(WindowSystem ws) {
            var rnd = new Random();

            BigLogWindow = new Window(new Vector2(Settings.Resolution.X/10*9, Settings.Resolution.Y/10*9), "Big Event Log", true, ws) {Visible = false};
            BigLogContainer = new ListContainer(new Rectangle(0, 0, (int)BigLogWindow.Width, (int)BigLogWindow.Height - 20), BigLogWindow);

            WindowStats = new Window(new Rectangle(50, 50, 400, 400), "Stats", true, ws) {Visible = false};
            StatsHeat = new ProgressBar(new Rectangle(50, 50, 100, 20), "", WindowStats);
            StatsJajda = new ProgressBar(new Rectangle(50, 50 + 30, 100, 20), "", WindowStats);
            StatsHunger = new ProgressBar(new Rectangle(50, 50 + 30*2, 100, 20), "", WindowStats);
            CloseAllTestButton = new Button(new Vector2(10, 100), "Close all", WindowStats);
            CloseAllTestButton.OnPressed += CloseAllTestButton_onPressed;
            contaiter1 = new ListContainer(new Rectangle(200, 200, 100, 200), WindowStats);
            for (int i = 1; i < 20; i++) {
                new Button(Vector2.Zero, rnd.Next(1, 1000).ToString(), contaiter1);
            }

            WindowMinimap = new Window(new Rectangle((int) Settings.Resolution.X - 180, 10, 128 + 20, 128 + 40),
                                       "minimap", false, ws) {NoBorder = true, Moveable = false};
            ImageMinimap = new Image(new Vector2(10, 10), new Texture2D(GraphicsDevice, 88, 88), Color.White,
                                     WindowMinimap);

            SpawnSomeWindow = new Window(new Vector2(200,400), "Spawn some", true, ws) {Visible = false};
            SpawnSomeList = new ListContainer(new Rectangle(0, 0, (int)SpawnSomeWindow.Width, (int)SpawnSomeWindow.Height - 20), SpawnSomeWindow);

            MoraleWindow = new Window(new Vector2(400, 400), "Morale", true, ws){Visible = false};
            MoraleContainer = new ListContainer(new Rectangle(0, 0, (int)MoraleWindow.Width, (int)MoraleWindow.Height - 20), MoraleWindow);

            AchievementsWindow = new Window(new Vector2(Settings.Resolution.X - 50, Settings.Resolution.Y - 50), "Achievements", true, ws) {Visible = false};
            AchievementContainer = new ListContainer(new Rectangle(10, 10, (int)(AchievementsWindow.Width - 20), (int)(AchievementsWindow.Height - 40)), AchievementsWindow);

            LookWindow = new Window(new Vector2(Settings.Resolution.X/3*2,Settings.Resolution.Y/2), "Item look", true, ws) {Visible = false};
            LookContainer = new ListContainer(new Rectangle(0,0,(int) LookWindow.Width,(int) (LookWindow.Height-20)), LookWindow);

            for (int i = 0; i < 40; i++) {
                new AchivementBox(Vector2.Zero, "some", bag, AchievementContainer);
            }

            InitWindowUI(ws);
            InitWindowSettings(ws);
            InitIngameMenu(ws);
            InitCaracterCration(ws);
            InitInventory(ws);
            InitContainer(ws);
            InitEventLog(ws);
            InitSchemes(ws);

            WindowIngameHint = new Window(new Vector2(50, 50), "HINT", false, ws)
            {NoBorder = true, Visible = false};
            LabelIngameHint = new Label(new Vector2(10, 3), "a-ha", WindowIngameHint);

            WindowGlobal = new Window(new Vector2(Settings.Resolution.X - 100, Settings.Resolution.Y - 50), "Map", true,
                                      ws) {Visible = false};
            ImageGlobal = new Image(new Vector2(10, 10), new Texture2D(GraphicsDevice, 10, 10), Color.White,
                                    WindowGlobal);
            ImageGlobal.OnMouseDown += ImageGlobal_OnMouseDown;
            ImageGlobal.OnMouseUp += ImageGlobalOnOnMouseUp;
            ImageGlobal.OnMouseMove += ImageGlobalOnOnMouseMove;

            InitCaracter(ws);


            InfoWindow = new Window(new Vector2(Settings.Resolution.X/3, Settings.Resolution.Y/6), "Info", true, ws) {
                Visible = false
            };
            InfoWindowLabel = new DoubleLabel(new Vector2(20, 20), "some info", InfoWindow);

            StatistWindow = new Window(new Vector2(Settings.Resolution.X/3, Settings.Resolution.Y/3), "Statistic", true,
                                       ws) {Visible = false};
            StatistList =
                new ListContainer(
                    new Rectangle(0, 0, (int) Settings.Resolution.X/3, (int) Settings.Resolution.Y/3 - 20),
                    StatistWindow);

            ConsoleWindow = new Window(new Vector2(Settings.Resolution.X/3, Settings.Resolution.Y/3), "Concole", true,
                                       ws) {Visible = false};
            ConsoleTB = new TextBox(new Vector2(10, 100), 200, ConsoleWindow);
            ConsoleTB.OnEnter += ConsoleTB_onEnter;
            ConsoleWindow.CenterComponentHor(ConsoleTB);
            SpawnItemButton = new Button(new Vector2(10,10), "Spawn item", ConsoleWindow);
            SpawnItemButton.OnPressed += SpawnItemButton_OnPressed;

            WindowRadio =
                new Window(
                    new Rectangle((int) (Settings.Resolution.X/2 - Settings.Resolution.X/6), -23,
                                  (int) (Settings.Resolution.X/6*2), (int) (Settings.Resolution.Y/15)), "radio", false,
                    ws) {Moveable = false};
            LabelRadio = new RunningLabel(new Vector2(0, 8), "radio string radio string radio string radio string",
                                          ((int) (Settings.Resolution.X/6*2) - 10)/9, WindowRadio);
            WindowRadio.CenterComponentHor(LabelRadio);

            ModLoaderWindow = new Window(new Vector2(Settings.Resolution.X/3*2, Settings.Resolution.Y/2), "ModLoader",
                                         true, ws) {Visible = false};
            ModLoaderContainer =
                new ListContainer(
                    new Rectangle(0, 0, (int) (ModLoaderWindow.Width/3*2), (int) ModLoaderWindow.Height - 20),
                    ModLoaderWindow);
            UpdateModLoader();

            InitCraft(ws);
        }

        void SpawnItemButton_OnPressed(object sender, EventArgs e)
        {
            ShowSpawnSomeWindow(ItemDataBase.Data.Keys.ToList(), true);
        }

        private void ShowSpawnSomeWindow(List<string> ids, bool isitem) {
            SpawnSomeWindow.Visible = true;
            SpawnSomeWindow.OnTop();
            SpawnSomeList.Clear();
            if (isitem) {
                int phase = 0;
                foreach (var id in ids) {
                    var a = new LabelFixed(Vector2.Zero, string.Format("{0} - {1}",id, ItemDataBase.Data[id].Name), phase == 0 ? Color.Gray : Color.CornflowerBlue, SpawnSomeList);
                    phase = 1 - phase;
                    a.Tag = id;
                    a.OnLeftPressed += a_OnLeftPressed;
                }
            }
        }

        void a_OnLeftPressed(object sender, LabelPressEventArgs e)
        {
           var a = ItemFactory.GetInstance((string)((Label)sender).Tag, 1);
           inventory_.AddItem(a);
           inventory_.StackSimilar();
           EventLog.Add(
               string.Format("Item {0} spawn in inventory", a.Id), GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
           UpdateInventoryContainer();
        }

        SchemesMap scheme = new SchemesMap(16, 16);
        Vector2 schemesOffset = new Vector2(0, 0);
        private void InitSchemes(WindowSystem ws) {
            SchemesEditorWindow = new Window(new Rectangle(0,0, (int)Settings.Resolution.X,(int)Settings.Resolution.Y), "Schemes editor", true, ws) {Visible = false};
            SchemesImages = new Image[20,20];
            for (int i = 0; i < SchemesImages.GetLength(0); i++) {
                for (int j = 0; j < SchemesImages.GetLength(1); j++) {
                        SchemesImages[i, j] = new Image(new Vector2(20 + 32*i, 20 + 32*j), Atlases.BlockArray["none"],
                                                        Color.White, SchemesEditorWindow);
                        SchemesImages[i, j].Tag = new Point(i, j);
                        SchemesImages[i, j].OnMouseDown += JargMain_OnMouseDown;
                }
            }
            SchemesContainer = new ListContainer(new Rectangle((int)Settings.Resolution.X/3*2, 40, (int)Settings.Resolution.X/4, (int)Settings.Resolution.Y/5*3), SchemesEditorWindow);
            SchemesSave = new Button(SchemesContainer.GetPosition()+new Vector2(-40,0), "Save", SchemesEditorWindow);
            SchemesSave.OnPressed += SchemesSave_OnPressed;

            SchemesExist = new ListContainer(new Rectangle((int)SchemesContainer.GetPosition().X, (int)(SchemesContainer.GetPosition().Y + SchemesContainer.Height), (int)Settings.Resolution.X / 4, (int)Settings.Resolution.Y / 5), SchemesEditorWindow);
        }

        /// <summary>
        /// #version = 1 (rle) scheme saver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SchemesSave_OnPressed(object sender, EventArgs e) {
            var sw = new StreamWriter(Directory.GetCurrentDirectory()+"\\"+"tempScheme.txt");
            sw.Write("#version = 1" + Environment.NewLine);
            sw.Write("~{0},{1},{2}{3}", scheme.rx, scheme.ry, "default", Environment.NewLine);
            for (int i = 0; i < scheme.rx * scheme.ry - 1; i++) {
                string id = scheme.block[i / scheme.ry, i % scheme.ry].Id;
                int count = 1;
                for (int j = i + 1; j < scheme.rx * scheme.ry - 1; j++) {
                    if (scheme.block[j / scheme.ry, j % scheme.ry].Id == id) {
                        count++;
                    } else {
                        break;
                    }
                }

                if (count > 2 || (count > 1 && id.Length > 1)) {
                    sw.Write(string.Format("!{0}!{1} ", id, count));
                } else {
                    for (int k = 0; k < count; k++) {
                        sw.Write(id + " ");
                    }
                }
                i += count - 1;
            }
            sw.Write(scheme.block[scheme.rx - 1, scheme.ry - 1].Id.Trim());

            sw.Write("\n");
            sw.Close();
            sw.Dispose();
        }

        private int schemesSelected = 0;
        void LOnLeftPressed(object sender, EventArgs e) {
            int i = (int) ((Label) sender).Tag;

            schemesSelected = i;
        }

        private bool schemesReady;
        private void SchemeEditorInit() {
            for (int i = 0; i < scheme.block.GetLength(0); i++) {
                for (int j = 0; j < scheme.block.GetLength(1); j++) {
                    scheme.block[i, j] = new Block() {
                        Id = "0",
                        MTex = "none"
                    };
                }
            }

            SchemesContainer.Clear();
            for (int i = 0; i < BlockDataBase.Data.Count; i++) {
                var l = new Label(Vector2.Zero,
                          BlockDataBase.Data.ElementAt(i).Key + " " + BlockDataBase.Data.ElementAt(i).Value.Name,
                          Color.White, SchemesContainer);
                l.Tag = i;
                l.OnLeftPressed += LOnLeftPressed;
            }

            SchemesExist.Clear();
            foreach (var sch in SchemesDataBase.Data) {
                var l = new Label(Vector2.Zero, sch.filename, SchemesExist);
                l.Tag = sch;
                l.OnLeftPressed += l_OnLeftPressed;
            }

            schemesReady = true;
        }

        void l_OnLeftPressed(object sender, LabelPressEventArgs e) {
            Schemes s = (Schemes) ((Label) sender).Tag;
            scheme = new SchemesMap(s.x, s.y);
            for (int i = 0; i < scheme.rx; i++)
            {
                for (int j = 0; j < scheme.ry; j++)
                {
                    scheme.block[i, j] = new Block()
                    {
                        Id = s.data[i * s.y + j],
                        MTex = BlockDataBase.Data[s.data[i * s.y + j]].MTex
                    };
                }
            }
        }

        void JargMain_OnMouseDown(object sender, Image.MouseStateEventArgs e) {
            Point p = (Point) ((Image) sender).Tag;

            if(scheme.rx > schemesOffset.X + p.X && scheme.ry > schemesOffset.Y + p.Y && schemesOffset.X + p.X > 0 && schemesOffset.Y + p.Y > 0 && e.Ms.LeftButton == ButtonState.Pressed) {
                string i = BlockDataBase.Data.ElementAt(schemesSelected).Key;
                scheme.block[(int)schemesOffset.X + p.X, (int)schemesOffset.Y + p.Y].Id = i;
                scheme.block[(int)schemesOffset.X + p.X, (int)schemesOffset.Y + p.Y].MTex = BlockDataBase.Data[i].MTex;
            }

            if (e.Ms.RightButton == ButtonState.Pressed) {
                schemesOffset = new Vector2(schemesOffset.X - (ms_.X - lms_.X) / 32f, schemesOffset.Y - (ms_.Y - lms_.Y) / 32f);
            }
        }

        private void InitCraft(WindowSystem ws) {
            CraftWindow = new Window(Settings.Resolution/4*3, "Craft", true, ws) {Visible = false};
            CraftSortAll = new Button(new Vector2(10, 10), "All recipes", CraftWindow);
            CraftSortAll.OnPressed += CraftSortAll_OnPressed;
            CraftItems = new ListContainer(
                new Rectangle(0, 40, (int) (CraftWindow.Width/5*2), (int) (CraftWindow.Height - 60)), CraftWindow);
            CraftMoreInfo = new LabelFixed(new Vector2(CraftItems.Width + 10, 40), string.Empty, CraftWindow);
            CraftThisButton = new Button(new Vector2(CraftItems.Width + 10, CraftWindow.Height - 60), "Craft", CraftWindow);
            CraftThisButton.OnPressed += CraftThisButton_OnPressed;
        }

        private void InitCaracter(WindowSystem ws) {
            int ii = 0;
            CaracterWindow =
                new Window(new Vector2(Settings.Resolution.X/3*2, Settings.Resolution.Y - Settings.Resolution.Y/10),
                           "Caracter info", true, ws) {Visible = false};
            ii++;
            LabelCaracterGun = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Ranged Weapon : ", CaracterWindow);
            ii++;
            LabelCaracterMeele = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Meele Weapon : ", CaracterWindow);
            ii++;
            LabelCaracterAmmo = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Ammo : ", CaracterWindow);
            ii += 2;
            LabelWearCaption = new Label(new Vector2(10, 10 + 15*ii), "Wear", CaracterWindow);
            ii += 2;
            ContainerWearList =
                new ListContainer(
                    new Rectangle(0, 10 + 15*ii, (int) CaracterWindow.Width/2,
                                  (int) CaracterWindow.Height/2 - (10 + 15*ii)), CaracterWindow);
            ii = 0;
            LabelCaracterHp = new DoubleLabel(new Vector2(10 + 300, 10 + 15*ii), "HP : ", CaracterWindow);
            LabelsAbilities = new List<Label>();
            for (int i = 0; i < 11; i++) {
                var temp = new Label(new Vector2(10, 400 + 15*ii), "", CaracterWindow);
                ii++;
                LabelsAbilities.Add(temp);
            }
            EffectsContainer =
                new ListContainer(
                    new Rectangle((int)CaracterWindow.Width / 2, (int)(CaracterWindow.Height / 2),
                                  (int)(CaracterWindow.Width / 2), (int)(CaracterWindow.Height / 2) - 19),
                    CaracterWindow);
        }

        private void InitEventLog(WindowSystem ws)
        {
            EventLogWindow =
                new Window(
                    new Rectangle(3, (int)(Settings.Resolution.Y - Settings.Resolution.Y / 4 - 3), (int)Settings.Resolution.X / 3,
                                  (int)Settings.Resolution.Y / 4), "Log",
                    true, ws) { Visible = false, Closable = false, NoBorder = true, Moveable = false };
            ContainerEventLog =
                new ListContainer(
                    new Rectangle(0, 0, (int)Settings.Resolution.X / 3, (int)Settings.Resolution.Y / 4 - 20),
                    EventLogWindow);
            ContainerEventLog.Clear();
            EventLog.OnLogUpdate += EventLog_onLogUpdate;
            ShowBigLogWindow = new Button(new Vector2(0, -20), "big", EventLogWindow);
            ShowBigLogWindow.OnPressed += ShowBigLogWindow_OnPressed;
        }

        void ShowBigLogWindow_OnPressed(object sender, EventArgs e)
        {
            BigLogWindow.Visible = true;
            BigLogContainer.Clear();

            for (int i = 0; i < EventLog.log.Count; i++) {
                var logEntity = EventLog.log[i];
                new LabelFixed(Vector2.Zero, logEntity.message, logEntity.col, BigLogContainer);
            }
            BigLogContainer.ScrollBottom();
        }

        private void InitContainer(WindowSystem ws) {
            WindowContainer =
                new Window(new Vector2(Settings.Resolution.X/2, Settings.Resolution.Y/3*2),
                           "Container", true, ws) {Visible = false};
            WindowContainer.SetPosition(new Vector2(Settings.Resolution.X/2, 0));
            ContainerContainer =
                new ListContainer(
                    new Rectangle(10, 10, (int)WindowContainer.Width / 2, (int)WindowContainer.Height - 40),
                    WindowContainer);
            LabelContainer = new LabelFixed(new Vector2(WindowContainer.Width - 200, 40), "",
                                            WindowContainer);
            ButtonContainerTakeAll =
                new Button(new Vector2(WindowContainer.Width - 200, WindowContainer.Height - 200 + 30 * 2),
                           "Take All (R)", WindowContainer);
            ButtonContainerTakeAll.OnPressed += ButtonContainerTakeAll_onPressed;
        }

        private void InitInventory(WindowSystem ws) {
            InventoryWindow =
                new Window(new Vector2(Settings.Resolution.X/2, Settings.Resolution.Y - Settings.Resolution.Y/10),
                           "Inventory", true, ws) {Visible = false};
            ContainerInventoryItems =
                new ListContainer(
                    new Rectangle(10, 10, InventoryWindow.Locate.Width/2, InventoryWindow.Locate.Height - 40),
                    InventoryWindow);

            InventoryMoreInfo = new LabelFixed(new Vector2(ContainerInventoryItems.GetPosition().X + 10, 40), "", 26,
                                               InventoryWindow);
            InventorySortAll =
                new Button(new Vector2(InventoryWindow.Locate.Width - 200, InventoryWindow.Locate.Height - 200), "All",
                           InventoryWindow);
            InventorySortAll.OnPressed += InventorySortAll_onPressed;
            InventorySortMedicine =
                new Button(new Vector2(InventoryWindow.Locate.Width - 200, InventoryWindow.Locate.Height - 200 + 30),
                           "Medicine", InventoryWindow);
            InventorySortMedicine.OnPressed += InventorySortMedicine_onPressed;
            InventorySortFood =
                new Button(new Vector2(InventoryWindow.Locate.Width - 200, InventoryWindow.Locate.Height - 200 + 30*2),
                           "Food", InventoryWindow);
            InventorySortFood.OnPressed += InventorySortFood_onPressed;
            IntentoryEquip =
                new Button(new Vector2(InventoryWindow.Locate.Width - 100, InventoryWindow.Locate.Height - 200 + 30*2),
                           "Equip", InventoryWindow);
            IntentoryEquip.OnPressed += IntentoryEquip_onPressed;
            InventoryTotalWV =
                new Label(new Vector2(InventoryWindow.Locate.Width - 200, InventoryWindow.Locate.Height - 200 + 30*3),
                          "some weight" + Environment.NewLine + "some volume", InventoryWindow);

            InventoryDropDownWindow = new Window(new Vector2(150, 220), "Select action", false, ws) {Visible = false};
            InventoryDropDownWindow.OnLooseAim += OnLooseAim;
            InventoryDropDownContainer = new ListContainer( new Rectangle(0,0,150, 200), InventoryDropDownWindow);
        }

        private void OnLooseAim(object sender, EventArgs eventArgs) {
            InventoryDropDownWindow.Visible = false;
        }

        private void InitCaracterCration(WindowSystem ws) {
            WindowCaracterCration = new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y),
                                               "CARACTER CREATION",
                                               false, ws) {NoBorder = true, Moveable = false, Visible = false};
            ButtonCaracterConfirm =
                new Button(new Vector2(Settings.Resolution.X/2 + Settings.Resolution.X/4, Settings.Resolution.Y/5*4),
                           "Continue", WindowCaracterCration);
            ButtonCaracterConfirm.OnPressed += ButtonCaracterConfirm_onPressed;
            ButtonCaracterCancel =
                new Button(new Vector2(Settings.Resolution.X/2 - Settings.Resolution.X/4, Settings.Resolution.Y/5*4),
                           "Cancel", WindowCaracterCration);
            ButtonCaracterCancel.OnPressed += ButtonCaracterCancel_onPressed;
            PerksContainer = new ListContainer(new Rectangle(40, 40, 250, (int) (Settings.Resolution.Y/3*2)),
                                               WindowCaracterCration);
            for (int i = 0; i < PerkDataBase.Perks.Count; i++) {
                KeyValuePair<string, PerkData> keyValuePair = PerkDataBase.Perks.ElementAt(i);
                if (keyValuePair.Value.Initial) {
                    var t = new CheckBox(Vector2.Zero, keyValuePair.Value.Name, PerksContainer)
                            {Cheked = player_.Perks.IsSelected(keyValuePair.Key), Tag = keyValuePair.Key};
                    t.OnPressed += Game1_onPressed;
                }
            }
        }

        private void InitIngameMenu(WindowSystem ws) {
            WindowIngameMenu = new Window(new Vector2(300, 400), "Pause", true, ws) {Visible = false};
            ButtonIngameMenuSettings = new Button(new Vector2(20, 100), "Settings", WindowIngameMenu);
            ButtonIngameMenuSettings.OnPressed += ButtonIngameMenuSettings_onPressed;
            ButtonIngameExit = new Button(new Vector2(20, 100 + 30*3), "Exit game", WindowIngameMenu);
            ButtonIngameExit.OnPressed += ButtonIngameExit_onPressed;

            WindowMainMenu = new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y), "MAIN MENU",
                                        false, ws) {NoBorder = true, Moveable = false};
            LabelMainMenu = new Label(new Vector2(10, 10),
                                      @"     __                      
    |__|____ _______  ____  
    |  \__  \\_  __ \/ ___\ 
    |  |/ __ \|  | \/ /_/  >
/\__|  (____  /__|  \___  / 
\______|    \/     /_____/"
                                      , WindowMainMenu);
            labelMainVer_ = new Label(new Vector2(10, LabelMainMenu.Height + 10), Version.GetShort(), Color.Gray,
                                      WindowMainMenu);
            WindowMainMenu.CenterComponentHor(LabelMainMenu);
            WindowMainMenu.CenterComponentHor(labelMainVer_);
            ButtonNewGame = new Button(new Vector2(10, 120 + 40*1), "New game", WindowMainMenu);
            ButtonNewGame.OnPressed += ButtonNewGame_onPressed;
            WindowMainMenu.CenterComponentHor(ButtonNewGame);
            DeleteLastWorldButton = new Button(new Vector2(ButtonNewGame.GetPosition().X + ButtonNewGame.Width + 20, ButtonNewGame.GetPosition().Y), "Delete Last World", WindowMainMenu);
            DeleteLastWorldButton.OnPressed += new EventHandler(DeleteLastWorldButton_OnPressed);

            ModLoaderButton = new Button(new Vector2(10, 100 + 40*4), "ModLoader", WindowMainMenu);
            WindowMainMenu.CenterComponentHor(ModLoaderButton);
            ModLoaderButton.OnPressed += ModLoaderButton_OnPressed;
            ButtonSettings = new Button(new Vector2(10, 100 + 40*5), "Settings", WindowMainMenu);
            WindowMainMenu.CenterComponentHor(ButtonSettings);
            ButtonSettings.OnPressed += ButtonIngameMenuSettings_onPressed;
            RunningMotd = new RunningLabel(new Vector2(10, Settings.Resolution.Y/2 - 50),
                                           "Jarg now in early development. It's tottaly free and opensource. Please send your suggestions to ishellstrike@gmail.com or github.com/ishellstrike/roguelikeworkname/issues.",
                                           50, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(RunningMotd);
            LabelControls = new Label(new Vector2(10, Settings.Resolution.Y/2 + 10),
                                      "I-inventory C-caracter page L-event log M-map WASD-moving LMB-shooting F1-debug info" +
                                      Environment.NewLine +
                                      "O-statistic P-achievements", WindowMainMenu);
            WindowMainMenu.CenterComponentHor(LabelControls);
            ButtonOpenGit = new Button(new Vector2(10, Settings.Resolution.Y/2 - 20), "Open in browser",
                                       WindowMainMenu);
            ButtonOpenGit.OnPressed += ButtonOpenGit_onPressed;
            WindowMainMenu.CenterComponentHor(ButtonOpenGit);
        }

        void DeleteLastWorldButton_OnPressed(object sender, EventArgs e) {
            if (Directory.Exists(Settings.GetWorldsDirectory())) {
                Directory.Delete(Settings.GetWorldsDirectory(), true);
                if (!Directory.Exists(Settings.GetWorldsDirectory())) {
                    Directory.CreateDirectory(Settings.GetWorldsDirectory());
                }
                var a = new Action(InitialGeneration);
                a.BeginInvoke(null, null);
            }
        }

        private void InitWindowSettings(WindowSystem ws) {
            WindowSettings =
                new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y),
                           "Settings", true, ws) {Visible = false, Moveable = false};
            LabelHudColor = new Label(new Vector2(10, 10), "HUD color", WindowSettings);
            ButtonHudColor1 = new Button(new Vector2(10 + 50 + 40*1, 10), "1", WindowSettings);
            ButtonHudColor1.OnPressed += ButtonHudColor1_onPressed;
            ButtonHudColor2 = new Button(new Vector2(10 + 50 + 40*2, 10), "2", WindowSettings);
            ButtonHudColor2.OnPressed += ButtonHudColor2_onPressed;
            ButtonHudColor3 = new Button(new Vector2(10 + 50 + 40*3, 10), "3", WindowSettings);
            ButtonHudColor3.OnPressed += ButtonHudColor3_onPressed;
            ButtonHudColor4 = new Button(new Vector2(10 + 50 + 40*4, 10), "4", WindowSettings);
            ButtonHudColor4.OnPressed += ButtonHudColor4_onPressed;
            ButtonHudColor5 = new Button(new Vector2(10 + 50 + 40*5, 10), "5", WindowSettings);
            ButtonHudColor5.OnPressed += ButtonHudColor5_onPressed;
            LabelHudColor = new Label(new Vector2(10, 10 + 40*1), "Time format", WindowSettings);
            Button12h = new Button(new Vector2(10 + 50 + 40*2, 10 + 40*1), "12h", WindowSettings);
            Button12h.OnPressed += Button12h_onPressed;
            Button24h = new Button(new Vector2(10 + 50 + 40*3, 10 + 40*1), "24h", WindowSettings);
            Button24h.OnPressed += Button24h_onPressed;
            LabelFullscreen = new Label(new Vector2(10, 10 + 40*2), "Fullscreen", WindowSettings);
            ButtonFullscreenOn = new Button(new Vector2(10 + 50 + 40*2, 10 + 40*2), "On", WindowSettings);
            ButtonFullscreenOn.OnPressed += ButtonFullscreenOn_onPressed;
            ButtonFullscreenOff = new Button(new Vector2(10 + 50 + 40*3, 10 + 40*2), "Off", WindowSettings);
            ButtonFullscreenOff.OnPressed += ButtonFullscreenOff_onPressed;
            LabelFramelimit = new Label(new Vector2(10, 10 + 40*3), "Framelimit", WindowSettings);
            ButtonFramelimitOn = new Button(new Vector2(10 + 50 + 40*2, 10 + 40*3), "On", WindowSettings);
            ButtonFramelimitOn.OnPressed += ButtonFramelimitOn_onPressed;
            ButtonFramelimitOff = new Button(new Vector2(10 + 50 + 40*3, 10 + 40*3), "Off", WindowSettings);
            ButtonFramelimitOff.OnPressed += ButtonFramelimitOff_onPressed;
            LabelLight = new Label(new Vector2(10, 10 + 40*4), "Enable lighting", WindowSettings);
            ButtonLightOn1 = new Button(new Vector2(LabelLight.GetPosition().X + LabelLight.Width + 10, 10 + 40*4), "Extra",
                                        WindowSettings);
            ButtonLightOn1.OnPressed += ButtonLightOn1On1Pressed;
            ButtonLightOn2 = new Button(new Vector2(ButtonLightOn1.GetPosition().X + ButtonLightOn1.Width + 10, 10 + 40*4),
                                        "Hight", WindowSettings);
            ButtonLightOn2.OnPressed += ButtonLightOn2On1Pressed;
            ButtonLightOn4 = new Button(new Vector2(ButtonLightOn2.GetPosition().X + ButtonLightOn2.Width + 10, 10 + 40*4),
                                        "Medium", WindowSettings);
            ButtonLightOn4.OnPressed += ButtonLightOn4On1Pressed;
            ButtonLightOn8 = new Button(new Vector2(ButtonLightOn4.GetPosition().X + ButtonLightOn4.Width + 10, 10 + 40*4),
                                        "Low", WindowSettings);
            ButtonLightOn8.OnPressed += ButtonLightOn8On1Pressed;
            ButtonLightOn12 = new Button(new Vector2(ButtonLightOn8.GetPosition().X + ButtonLightOn8.Width + 10, 10 + 40*4),
                                         "Minimum", WindowSettings);
            ButtonLightOn12.OnPressed += ButtonLightOn12On1Pressed;
            ButtonLightOff = new Button(new Vector2(ButtonLightOn12.GetPosition().X + ButtonLightOn12.Width + 10, 10 + 40*4),
                                        "Disabled", WindowSettings);
            ButtonLightOff.OnPressed += ButtonLightOff_onPressed;
            LabelResolution = new Label(new Vector2(10, 10 + 40*5), "Resolution", WindowSettings);
            ButtonResolution800600 = new Button(new Vector2(10 + 50 + 95*1, 10 + 40*5), " 800x600 ",
                                                WindowSettings);
            ButtonResolution800600.OnPressed += ButtonResolution800600_onPressed;
            ButtonResolution1024768 = new Button(new Vector2(10 + 50 + 95*2, 10 + 40*5), "1024x768 ",
                                                 WindowSettings);
            ButtonResolution1024768.OnPressed += ButtonResolution1024768_onPressed;
            ButtonResolution1280800 = new Button(new Vector2(10 + 50 + 95*3, 10 + 40*5), "1280x800 ",
                                                 WindowSettings);
            ButtonResolution1280800.OnPressed += ButtonResolution1280800_onPressed;
            ButtonResolution19201024 = new Button(new Vector2(10 + 50 + 95*4, 10 + 40*5), "1920x1024",
                                                  WindowSettings);
            ButtonResolution19201024.OnPressed += ButtonResolution19201024_onPressed;
            LabelOnlineRadio = new Label(new Vector2(10, 10 + 40*6), "Radio", WindowSettings);
            ButtonRadioGB = new Button(new Vector2(10 + 50 + 95*1, 10 + 40*6), "GhostBox", WindowSettings);
            ButtonRadioGB.OnPressed += ButtonRadioGB_onPressed;
            ButtonRadioOff = new Button(new Vector2(10 + 50 + 95*2, 10 + 40*6), "  Off   ", WindowSettings);
            ButtonRadioOff.OnPressed += ButtonRadioOff_onPressed;
        }

        private void InitWindowUI(WindowSystem ws) {
            WindowUIButtons =
                new Window(
                    new Rectangle((int) Settings.Resolution.X - 32, (int) Settings.Resolution.Y/2 - 32, 32, 32*3 + 20),
                    "", false, ws) {NoBorder = true, Moveable = false};
            IBBag = new ImageButton(new Vector2(0, 0), "", bag, WindowUIButtons);
            IBBag.OnPressed += IBBag_onPressed;
            IBCaracter = new ImageButton(new Vector2(0, 32), "", caracter, WindowUIButtons);
            IBCaracter.OnPressed += IBCaracter_onPressed;
            IBInv = new ImageButton(new Vector2(0, 64), "", map, WindowUIButtons);
            IBInv.OnPressed += IBInv_onPressed;
        }

        private void ButtonLightOff_onPressed(object sender, EventArgs e)
        {
            Settings.Lighting = false;
        }

        private void ButtonLightOn1On1Pressed(object sender, EventArgs e)
        {
            Settings.Lighting = true;
            LightQ = 1;
            ResolutionChanging();
        }

        private void ButtonLightOn12On1Pressed(object sender, EventArgs e) {
            Settings.Lighting = true;
            LightQ = 12;
            ResolutionChanging();
        }

        private void ButtonLightOn2On1Pressed(object sender, EventArgs e) {
            Settings.Lighting = true;
            LightQ = 2;
            ResolutionChanging();
        }

        private void ButtonLightOn4On1Pressed(object sender, EventArgs e) {
            Settings.Lighting = true;
            LightQ = 4;
            ResolutionChanging();
        }

        private void ButtonLightOn8On1Pressed(object sender, EventArgs e) {
            Settings.Lighting = true;
            LightQ = 8;
            ResolutionChanging();
        }

        void CraftThisButton_OnPressed(object sender, EventArgs e)
        {
            if (selectedCraft != null) {
                inventory_.Craft(selectedCraft);
            }
        }

        private void ImageGlobalOnOnMouseMove(object sender, Image.MouseStateEventArgs mouseStateEventArgs)
        {
            if (mousemappress_)
            {
                mousemapoffset -= new Vector2((mouseStateEventArgs.Ms.X - mouseStateEventArgs.Lms.X) / 10f,
                                              (mouseStateEventArgs.Ms.Y - mouseStateEventArgs.Lms.Y) / 10f);
                currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_, mousemapoffset);
            }
        }

        private void ImageGlobalOnOnMouseUp(object sender, Image.MouseStateEventArgs mouseStateEventArgs) {
            mousemappress_ = false;
        }

        private bool mousemappress_;
        private Vector2 mousemapoffset;
        void ImageGlobal_OnMouseDown(object sender, Image.MouseStateEventArgs e) {
            mousemappress_ = true;
        }

        private CraftData selectedCraft;
        void Update_Craft_Items() {
            CraftItems.Clear();
            selectedCraft = null;
            CraftMoreInfo.Text = string.Empty;

            foreach (var craftData in CraftDataBase.Data) {
                var a = new LabelFixed(Vector2.Zero,  craftData.OutputCount[0] + " " + ItemDataBase.Data[craftData.Output[0]].Name,
                                       CraftItems) { Tag = craftData };
                a.OnLeftPressed += CraftItemsLabelOnLeftPressed;
            }
        }

        void CraftItemsLabelOnLeftPressed(object sender, EventArgs e) {
            selectedCraft = (CraftData)((IGameComponent) sender).Tag;
            CraftMoreInfo.Text = selectedCraft.ToString();
        }

        void CraftSortAll_OnPressed(object sender, EventArgs e)
        {
            
        }

        private void ModLoaderButton_OnPressed(object sender, EventArgs e) {
            ModLoaderWindow.Visible = !ModLoaderWindow.Visible;
            if (ModLoaderWindow.Visible) {
                ModLoaderWindow.OnTop();
            }
        }

        private void UpdateModLoader() {
            ModLoaderContainer.Clear();

            new LabelFixed(Vector2.Zero, "Units", Color.Cyan, ModLoaderContainer);
            string[] f = Directory.GetFiles(Settings.GetDataDirectory() + @"\Units", "*.txt");
            foreach (string s in f) {
                new CheckBox(Vector2.Zero, s, ModLoaderContainer);
            }

            new LabelFixed(Vector2.Zero, "Blocks", Color.Cyan, ModLoaderContainer);
            f = Directory.GetFiles(Settings.GetObjectDataDirectory(), "*.txt");
            foreach (string s in f) {
                new CheckBox(Vector2.Zero, s, ModLoaderContainer);
            }

            new LabelFixed(Vector2.Zero, "Items", Color.Cyan, ModLoaderContainer);
            f = Directory.GetFiles(Settings.GetItemDataDirectory(), "*.txt");
            foreach (string s in f) {
                new CheckBox(Vector2.Zero, s, ModLoaderContainer);
            }

            new LabelFixed(Vector2.Zero, "Buffs", Color.Cyan, ModLoaderContainer);
            f = Directory.GetFiles(Settings.GetEffectDataDirectory(), "*.txt");
            foreach (string s in f) {
                new CheckBox(Vector2.Zero, s, ModLoaderContainer);
            }
        }

        private void IBInv_onPressed(object sender, EventArgs e) {
            WindowGlobal.Visible = !WindowGlobal.Visible;
            if (WindowGlobal.Visible) {
                WindowGlobal.OnTop();
            }
        }

        private void IBCaracter_onPressed(object sender, EventArgs e) {
            CaracterWindow.Visible = !CaracterWindow.Visible;
            if (CaracterWindow.Visible) {
                CaracterWindow.OnTop();
            }
        }

        private void IBBag_onPressed(object sender, EventArgs e) {
            InventoryWindow.Visible = !InventoryWindow.Visible;
            if (InventoryWindow.Visible) {
                InventoryWindow.OnTop();
            }
        }

        private void Game1_onPressed(object sender, EventArgs e) {
            var t = (string) ((IGameComponent) sender).Tag;

            player_.Perks.SetPerk(t);
        }

        private void ButtonRadioOff_onPressed(object sender, EventArgs e) {
            WMPs.controls.stop();
            WindowRadio.Visible = false;
        }

        private void ButtonRadioGB_onPressed(object sender, EventArgs e) {
            WMPs.controls.play();
            WindowRadio.Visible = true;
        }

        private void ButtonFramelimitOff_onPressed(object sender, EventArgs e) {
            IsFixedTimeStep = false;
        }

        private void ButtonFramelimitOn_onPressed(object sender, EventArgs e) {
            IsFixedTimeStep = true;
        }

        private void ButtonResolution19201024_onPressed(object sender, EventArgs e) {
            Settings.Resolution = new Vector2(1920, 1024);
            ResolutionChanging();
        }

        private void ButtonResolution1280800_onPressed(object sender, EventArgs e) {
            Settings.Resolution = new Vector2(1280, 800);
            ResolutionChanging();
        }

        private void ButtonResolution1024768_onPressed(object sender, EventArgs e) {
            Settings.Resolution = new Vector2(1024, 768);
            ResolutionChanging();
        }

        private void ButtonResolution800600_onPressed(object sender, EventArgs e) {
            Settings.Resolution = new Vector2(800, 600);
            ResolutionChanging();
        }

        private void ButtonFullscreenOff_onPressed(object sender, EventArgs e) {
            if (graphics_.IsFullScreen) {
                graphics_.ToggleFullScreen();
            }
            ResolutionChanging();
        }

        private void ButtonFullscreenOn_onPressed(object sender, EventArgs e) {
            if (!graphics_.IsFullScreen) {
                graphics_.ToggleFullScreen();
            }
            ResolutionChanging();
        }

        Action rlg = RandomLogFill;
        IAsyncResult rldrslt;
        private void ConsoleTB_onEnter(object sender, EventArgs e) {
            string s = ConsoleTB.Text;
            ConsoleTB.Tag = "";

            if (s.Contains("spawn c ")) {
                string ss = s.Substring(8);
                if (CreatureDataBase.Data.ContainsKey(ss)) {
                    Vector2 pp = player_.GetWorldPositionInBlocks();
                    pp.X = (int) pp.X;
                    pp.Y = (int) pp.Y;
                    Vector2 ppp = currentFloor_.GetInSectorPosition(pp);
                    var i = (int) (player_.Position.X - (int) ppp.X*MapSector.Rx*32);
                    var i1 = (int) (player_.Position.Y - (int) ppp.Y*MapSector.Ry*32);
                    currentFloor_.GetSector((int) ppp.X, (int) ppp.Y).Spawn(ss, i/MapSector.Rx, i1/MapSector.Ry);
                    EventLog.Add(
                        string.Format("Creature {0} spawn at ({1}, {2}), in ({3}, {4})", ss, i, i1, (int) ppp.X,
                                      (int) ppp.Y), GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
                }
                else {
                    EventLog.Add(string.Format("Creature {0} not found", ss), GlobalWorldLogic.CurrentTime, Color.Cyan,
                                 LogEntityType.Console);
                }
            }
            if (s.Contains("mypos")) {
                Vector2 pp = player_.GetWorldPositionInBlocks();
                pp.X = (int) pp.X;
                pp.Y = (int) pp.Y;
                Vector2 ppp = currentFloor_.GetInSectorPosition(pp);
                var i = (int) (player_.Position.X - (int) ppp.X*MapSector.Rx*32);
                var i1 = (int) (player_.Position.Y - (int) ppp.Y*MapSector.Ry*32);
                EventLog.Add(
                    string.Format("Player position {4} or ({0}, {1}) in sector ({2}, {3})", i, i1, (int) ppp.X,
                                  (int) ppp.Y, player_.Position), GlobalWorldLogic.CurrentTime, Color.Cyan,
                    LogEntityType.Console);
            }

            if (s.Contains("time ")) {
                string ss = s.Substring(5);
                int res;
                if(int.TryParse(ss, out res)) {
                    EventLog.Add(
                        string.Format("Old time"), GlobalWorldLogic.CurrentTime, Color.Cyan,
                        LogEntityType.Console);
                    GlobalWorldLogic.CurrentTime = GlobalWorldLogic.CurrentTime.AddHours(res - GlobalWorldLogic.CurrentTime.Hour);
                    EventLog.Add(
                        string.Format("New time"), GlobalWorldLogic.CurrentTime, Color.Cyan,
                        LogEntityType.Console);
                } else {
                    EventLog.Add(string.Format("Wrong number to time <hour>"),
                                 GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
                }
            }

            if (s.Contains("randomlog"))
            {
                if (rldrslt == null || rldrslt.IsCompleted) {
                    rldrslt = rlg.BeginInvoke(null, null);
                }
            }
            if (s.Contains("fastwalk")) {
                acmodifer = acmodifer == 1 ? 4 : 1;
                EventLog.Add(string.Format("walk x{0}", acmodifer), GlobalWorldLogic.CurrentTime, Color.Cyan,
                             LogEntityType.Console);
            }
            if (s.Contains("noclip")) {
                Settings.Noclip = !Settings.Noclip;
                EventLog.Add(string.Format("Noclip = {0}", Settings.Noclip), GlobalWorldLogic.CurrentTime, Color.Cyan,
                             LogEntityType.Console);
            }
            if (s.Contains("killall")) {
                int t = currentFloor_.KillAllCreatures();
                EventLog.Add(string.Format("{0} creatures in active sector killed!", t), GlobalWorldLogic.CurrentTime,
                             Color.Cyan, LogEntityType.Console);
            }
            if (s.Contains("spawn i ")) {
                string ss = s.Substring(8);
                if (ItemDataBase.Data.ContainsKey(ss))
                {
                    var a = ItemFactory.GetInstance(ss, 1);
                    inventory_.AddItem(a);
                    inventory_.StackSimilar();
                    EventLog.Add(
                        string.Format("Item {0} spawn in inventory", a.Id), GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
                    UpdateInventoryContainer();
                }
                else
                {
                    EventLog.Add(string.Format("Item {0} not found", ss), GlobalWorldLogic.CurrentTime, Color.Cyan,
                                 LogEntityType.Console);
                }
            }
            if (s.Contains("tp ")) {
                string ss = s.Substring(3);
                string[] parts = ss.Split(' ');
                int x, y;
                if (parts.Length == 2) {
                    if (int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y)) {
                        player_.Position = new Vector2(x*MapSector.Rx*32, y*MapSector.Ry*32);
                        mousemapoffset = Vector2.Zero;
                        EventLog.Add(string.Format("Teleported to sector ({0}, {1})", x, y),
                                     GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
                    }
                    else {
                        EventLog.Add(string.Format("Wrong number to tp <x> <y>"),
                                     GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
                    }
                }
                else {
                    EventLog.Add(string.Format("Wrong parameters for tp <x> <y>"),
                                 GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
                }
            }
            if (s.Contains("testitems1")) {
                inventory_.AddItem(new Item{Id = "testhat",Count = 1});
                inventory_.AddItem(new Item{Id = "testhat2",Count = 1});
                inventory_.AddItem(new Item{Id = "ak47",Count = 1});
                inventory_.AddItem(new Item{Id = "a762",Count = 100000});
                UpdateInventoryContainer();
            }
            if (s.Contains("testitems2")) {
                inventory_.AddItem(new Item{Id = "colacan", Count = 1});
                inventory_.AddItem(new Item{Id = "colacan", Count = 1});
                inventory_.AddItem(new Item{Id = "colacan", Count = 1});
                inventory_.AddItem(new Item{Id = "colacan", Count = 1});
                inventory_.AddItem(new Item{Id = "colacan", Count = 1});
                inventory_.AddItem(new Item{Id = "meatcan1", Count = 1});
                inventory_.AddItem(new Item {Id = "meatcan1", Count = 1});
                inventory_.AddItem(new Item{Id = "meatcan1", Count = 1});
                inventory_.AddItem(new Item{Id = "meatcan1", Count = 1});                inventory_.AddItem(new Item{Id = "meatcan1", Count = 1});
               UpdateInventoryContainer();
            }
            ConsoleTB.Text = string.Empty;
        }

        private static void RandomLogFill() {
            for (int i = 0; i <= 128; i++) {
                Thread.Sleep(50);
                EventLog.Add(Settings.rnd.Next().ToString()+string.Format(" ({0}/{1})",i+1,128), new Color(Settings.rnd.Next(0, 256), Settings.rnd.Next(0, 256), Settings.rnd.Next(0, 256)), LogEntityType.Default);
            }
        }

        private void ShowInfoWindow(string s1, string s2) {
            InfoWindowLabel.Text = s1;
            InfoWindowLabel.Text2 = s2;
            // InfoWindow.CenterComponentHor(InfoWindowLabel);
            InfoWindow.Visible = true;
            InfoWindow.OnTop();
        }

        private void HideInfoWindow() {
            InfoWindow.Visible = false;
        }

        private void IntentoryEquip_onPressed(object sender, EventArgs e) {
            inventory_.UseItem(selectedItem, player_);
            UpdateInventoryContainer();
        }

        private void ButtonIngameExit_onPressed(object sender, EventArgs e) {
            currentFloor_.SaveAllAndExit(player_, inventory_);
        }

        private void ButtonContainerTakeAll_onPressed(object sender, EventArgs e) {
            inventory_.AddItemRange(inContainer_);
            inContainer_.Clear();
            inventory_.StackSimilar();
            UpdateContainerContainer(inContainer_);
            UpdateInventoryContainer();
        }

        private void Button24h_onPressed(object sender, EventArgs e) {
            Settings.IsAMDM = false;
        }

        private void Button12h_onPressed(object sender, EventArgs e) {
            Settings.IsAMDM = true;
        }

        private void EventLog_onLogUpdate(object sender, EventArgs e) {
            ContainerEventLog.Clear();
            int i = 0;
            for (int j = 0; j < EventLog.log.Count; j++)
            {
                LogEntity ss = EventLog.log[j];
                new LabelFixed(Vector2.Zero, ss.message, ss.col, ContainerEventLog);
                i++;
            }
            ContainerEventLog.ScrollBottom();
        }

        private void InventorySortFood_onPressed(object sender, EventArgs e) {
            nowSort_ = ItemType.Food;
            UpdateInventoryContainer();
        }

        private void InventorySortMedicine_onPressed(object sender, EventArgs e) {
            nowSort_ = ItemType.Medicine;
            UpdateInventoryContainer();
        }

        private void InventorySortAll_onPressed(object sender, EventArgs e) {
            nowSort_ = ItemType.Nothing;
            UpdateInventoryContainer();
        }

        private void UpdateInventoryContainer() {
            selectedItem = null;
            InventoryMoreInfo.Text = "";

            List<IItem> a = inventory_.FilterByType(nowSort_);
            inInv_ = a;

            ContainerInventoryItems.Clear();

            var weap = new List<Item>();
            var ammo = new List<Item>();
            var wear = new List<Item>();
            var med = new List<Item>();
            var food = new List<Item>();
            var other = new List<Item>();
            foreach (Item x in a) {
                switch (x.Data.SortType) {
                    case ItemType.Gun:
                        weap.Add(x);
                        break;
                    case ItemType.Ammo:
                        ammo.Add(x);
                        break;
                    case ItemType.Wear:
                        wear.Add(x);
                        break;
                    case ItemType.Medicine:
                        med.Add(x);
                        break;
                    case ItemType.Food:
                        food.Add(x);
                        break;
                    default:
                        other.Add(x);
                        break;
                }
            }


            if (weap.Count > 0) {
                new LabelFixed(Vector2.Zero, "Weapons", Color.Cyan, ContainerInventoryItems);
                foreach (Item item in weap) {
                    AddInventoryItemString(item);
                }
            }


            if (ammo.Count > 0) {
                new LabelFixed(Vector2.Zero, "Ammo", Color.Cyan, ContainerInventoryItems);
                foreach (Item item in ammo) {
                    AddInventoryItemString(item);
                }
            }


            if (wear.Count > 0) {
                new LabelFixed(Vector2.Zero, "Wear", Color.Cyan, ContainerInventoryItems);
                foreach (Item item in wear) {
                    AddInventoryItemString(item);
                }
            }


            if (med.Count > 0) {
                new LabelFixed(Vector2.Zero, "Medicine", Color.Cyan, ContainerInventoryItems);
                foreach (Item item in med) {
                    AddInventoryItemString(item);
                }
            }


            if (food.Count > 0) {
                new LabelFixed(Vector2.Zero, "Food", Color.Cyan, ContainerInventoryItems);
                foreach (Item item in food) {
                    AddInventoryItemString(item);
                }
            }


            if (other.Count > 0) {
                new LabelFixed(Vector2.Zero, "Other", Color.Cyan, ContainerInventoryItems);
                foreach (Item item in other) {
                    AddInventoryItemString(item);
                }
            }

            //foreach (var item in a) {
            //    var i = new LabelFixed(Vector2.Zero, item.ToString(), 22, ContainerInventoryItems);
            //    i.Tag = cou;
            //    i.OnLeftPressed += PressInInventory;
            //    cou++;
            //}
            InventoryTotalWV.Text = string.Format("Weight {0}/{1}{2}Volume {3}/{4}", inventory_.TotalWeight,
                                                  player_.MaxWeight, Environment.NewLine,
                                                  inventory_.TotalVolume, player_.MaxVolume);
        }

        private void AddInventoryItemString(IItem item) {
            Color col = item.Data.Prototype != typeof(Item) ? Color.LightGoldenrodYellow : Color.LightGray;
            var i = new LabelFixed(Vector2.Zero, item.ToString(), col, ContainerInventoryItems) {Tag = item};
            i.OnLeftPressed += PressInInventory;
            i.OnRightPressed += RightPressInInventory;
        }

        private void RightPressInInventory(object sender, LabelPressEventArgs labelPressEventArgs)
        {
            IItem i = (IItem)((LabelFixed)sender).Tag;
            if (i.Actions == null || i.Actions.Count == 0) {
                return;
            }
            InventoryDropDownContainer.Clear();
            foreach (var action in i.Actions) {
                var a = new LabelFixed(Vector2.Zero, action.Name, InventoryDropDownContainer);
                a.Tag = action;
                a.OnLeftPressed += AOnOnLeftPressed;
            }

            InventoryDropDownWindow.Visible = true;
            InventoryDropDownWindow.SetPosition(new Vector2(labelPressEventArgs.Ms.X - 10, labelPressEventArgs.Ms.Y - 10));
            InventoryDropDownWindow.OnTop();
        }

        private void AOnOnLeftPressed(object sender, LabelPressEventArgs labelPressEventArgs) {
            var action = (ItemAction)((LabelFixed) sender).Tag;
            action.Action(player_);
            InventoryDropDownWindow.Visible = false;
        }

        private void UpdateContainerContainer(List<IItem> a) {
            inContainer_ = a;

            ContainerContainer.Clear();

            int cou = 0;
            foreach (Item item in a) {
                var i = new LabelFixed(Vector2.Zero, item.ToString(), ContainerContainer);
                i.Tag = cou;
                i.OnLeftPressed += PressInContainer;
                cou++;
            }
        }

        private void PressInInventory(object sender, EventArgs e) {
            var label = sender as Label;
            if (label != null) {
                var a = (Item) label.Tag;
                selectedItem = a;

                if (!doubleclick_) {
                    InventoryMoreInfo.Text = ItemDataBase.GetItemFullDescription(a);
                }
                else {
                    IntentoryEquip_onPressed(null, null);
                }
            }
        }

        private void PressInContainer(object sender, EventArgs e) {
            var a = (int) (sender as Label).Tag;
            if (inInv_.Count > a) {
                ContainerSelected = inContainer_[a];
                LabelContainer.Text = ItemDataBase.GetItemFullDescription(ContainerSelected);
                if (doubleclick_) {
                    if (inContainer_.Contains(ContainerSelected)) {
                        inventory_.AddItem(ContainerSelected);
                        inContainer_.Remove(ContainerSelected);
                        inventory_.StackSimilar();
                        UpdateInventoryContainer();
                        UpdateContainerContainer(inContainer_);
                    }
                }
            }
        }

        private void ButtonCaracterCancel_onPressed(object sender, EventArgs e) {
            WindowMainMenu.Visible = true;
            WindowCaracterCration.Visible = false;
        }

        private void ButtonCaracterConfirm_onPressed(object sender, EventArgs e) {
            WindowCaracterCration.Visible = false;
            drawAction_ = GameDraw;
            UpdateAction = GameUpdate;
            EventLogWindow.Visible = true;
        }

        private void ButtonNewGame_onPressed(object sender, EventArgs e) {
            WindowMainMenu.Visible = false;
            WindowCaracterCration.Visible = true;
        }

        private void ButtonOpenGit_onPressed(object sender, EventArgs e) {
            Process.Start("https://github.com/ishellstrike/roguelikeworkname/issues");
        }

        private void ButtonIngameMenuSettings_onPressed(object sender, EventArgs e) {
            WindowSettings.Visible = true;
            WindowSettings.OnTop();
        }

        private void ButtonHudColor3_onPressed(object sender, EventArgs e) {
            Settings.HudСolor = Color.DarkGray;
        }

        private void ButtonHudColor5_onPressed(object sender, EventArgs e) {
            Settings.HudСolor = Color.LightGreen;
        }

        private void ButtonHudColor4_onPressed(object sender, EventArgs e) {
            Settings.HudСolor = Color.DarkOrange;
        }

        private void ButtonHudColor2_onPressed(object sender, EventArgs e) {
            Settings.HudСolor = Color.LightGray;
        }

        private void ButtonHudColor1_onPressed(object sender, EventArgs e) {
            Settings.HudСolor = Color.White;
        }

        private void CloseAllTestButton_onPressed(object sender, EventArgs e) {
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

            if (currentFloor_ != null && WindowMinimap.Visible) {
                ImageMinimap.image = currentFloor_.GetMinimap();
            }

            if(SchemesEditorWindow.Visible && schemesReady) {
                for (int i = 0; i < SchemesImages.GetLength(0); i++) {
                    for (int j = 0; j < SchemesImages.GetLength(1); j++) {
                        if (scheme.rx > (int)schemesOffset.X + i && scheme.ry > (int)schemesOffset.Y + j && (int)schemesOffset.X + i >= 0 && (int)schemesOffset.Y + j >= 0)
                        {
                            SchemesImages[i, j].image = Atlases.BlockArray[scheme.block[i + (int)schemesOffset.X, j + (int)schemesOffset.Y].MTex];
                            if (scheme.block[i + (int)schemesOffset.X, j + (int)schemesOffset.Y].Id == "0")
                            {
                                scheme.block[i + (int)schemesOffset.X, j + (int)schemesOffset.Y].MTex = "notex";
                                SchemesImages[i, j].col_ = Color.Red;
                            } else {
                                SchemesImages[i, j].col_ = Color.White;
                            }
                        } else {
                            SchemesImages[i, j].image = Atlases.BlockArray["bush1"];
                            SchemesImages[i, j].col_ = Color.White;
                        }
                        
                    }
                }
            }

            if (MoraleWindow.Visible) {
                MoraleContainer.Clear();
                new LabelFixed(Vector2.Zero, string.Format("Общая: {0}", player_.Morale.Current - player_.Morale.Max / 2), Color.White, MoraleContainer);
            }

            if (Settings.InventoryUpdate) {
                inventory_.StackSimilar();
                UpdateInventoryContainer();
                Settings.InventoryUpdate = false;
            }

            if (SecondTimespan.TotalSeconds >= 1) {
                StatistList.Clear();
                foreach (var statist in AchievementDataBase.Stat) {
                    if (statist.Value.Count != 0) {
                        new Label(Vector2.Zero, statist.Value.Name + ": " + statist.Value.Count, StatistList);
                    }
                }

                if (CaracterWindow.Visible) {
                    UpdateCaracterWindowItems(null, null);
                }
            }
            if (LookWindow.Visible && looklPos != null) {
                lineBatch_.AddLine(player_.Position - camera_, new Vector2(looklPos.Value.X*32, looklPos.Value.Y*32) - camera_, Color.LimeGreen, 5);
            }
        }

        private void UpdateCaracterWindowItems(object sender, EventArgs eventArgs) {
            LabelCaracterHp.Text2 = string.Format("{0}/{1}", player_.Hp.Current, player_.Hp.Max);

            for (int i = 0; i < LabelsAbilities.Count; i++) {
                LabelsAbilities[i].Text = string.Format("{0} {1} ({2}/{3})", player_.Abilities.ToShow[i].Name,
                                                        player_.Abilities.ToShow[i],
                                                        (int) player_.Abilities.ToShow[i].XpCurrent,
                                                        Ability.XpNeeds[player_.Abilities.ToShow[i].XpLevel]);
            }

            LabelCaracterGun.Text2 = player_.ItemGun != null ? ItemDataBase.Data[player_.ItemGun.Id].Name : "";
            LabelCaracterAmmo.Text2 = player_.ItemAmmo != null
                                          ? ItemDataBase.Data[player_.ItemAmmo.Id].Name + " x" +
                                            player_.ItemAmmo.Count
                                          : "";
            LabelCaracterMeele.Text2 = player_.ItemMeele != null
                                           ? ItemDataBase.Data[player_.ItemMeele.Id].Name
                                           : "";

            EffectsContainer.Clear();
            for (int i = 0; i < player_.Buffs.Count; i++) {
                LabelFixed label;
                if (player_.Buffs[i].Expiring) {
                    label = new LabelFixed(Vector2.Zero,
                                           string.Format("{0} {1}", BuffDataBase.Data[player_.Buffs[i].Id].Name,
                                                         player_.Buffs[i].Expire), EffectsContainer);
                }
                else {
                    label = new LabelFixed(Vector2.Zero,
                                           string.Format("{0}", BuffDataBase.Data[player_.Buffs[i].Id].Name),
                                           EffectsContainer);
                }
            }

            ContainerWearList.Clear();
            foreach (Item item in player_.Weared) {
                var label = new LabelFixed(Vector2.Zero, string.Format("{0}", item.Data.Name), ContainerWearList);
            }
        }

        private void Update_Look_Items() {
            LookContainer.Clear();
            looklPos = null;

            var t = currentFloor_.GetVisibleStorages();
            StringBuilder sb = new StringBuilder();
            //sb.Append("Вы видите вокруг: ");
            //List<string> allit = new List<string>();

            foreach (var tuple in t) {
                string format;
                if (tuple.Item2.Count == 0 && tuple.Item2.Doses != 0) {
                    format = string.Format("{0} ({1})", tuple.Item2.Data.Name, tuple.Item2.Doses);
                }
                else {
                    format = string.Format("{0} x{1}", tuple.Item2.Data.Name, tuple.Item2.Count);
                }

                Color col = tuple.Item2.Data.Prototype != typeof(Item) ? Color.LightGoldenrodYellow : Color.LightGray;
                var lookl = new LabelFixed(Vector2.Zero, format, col, LookContainer);
                lookl.Tag = tuple;
                lookl.OnLeftPressed += lookl_OnLeftPressed;
                //allit.Add(format);
            }
            //sb.Append(String.Join(", ", allit));

            //EventLog.Add(sb.ToString(), Color.LightGreen, LogEntityType.SeeSomething);
        }

        private Vector2? looklPos;
        void lookl_OnLeftPressed(object sender, LabelPressEventArgs e) {
            var t = (Tuple<Vector2, IItem>)(((Label)sender).Tag);
            looklPos = t.Item1;
            foreach (var gameComponent in LookContainer.GetItems()) {
                ((Label)gameComponent).Color = ((Tuple<Vector2, IItem>)(((Label)gameComponent).Tag)).Item2.Data.Prototype != typeof(Item) ? Color.LightGoldenrodYellow : Color.LightGray;
            }
            ((Label) sender).Color = Color.LimeGreen;
        }
    }

    public class SchemesMap
    {
        public Block[,] block;
        public int rx, ry;

        public SchemesMap(int x, int y)
        {
            rx = x;
            ry = y;
            block = new Block[x, y];

            //for (int i = 0; i < x; i++) {
            //    for (int j = 0; j < y; j++) {
            //        block[i, j] = new Block();
            //    }
            //}
        }

        public void CreateAllMapFromArray(string[] ints)
        {
            for (int i = 0; i < ints.Length; i++) {
                block[i / ry, i % ry].Id = ints[i];
                block[i / ry, i % ry].MTex = BlockDataBase.Data[ints[i]].MTex;
            }
        }
    }
}