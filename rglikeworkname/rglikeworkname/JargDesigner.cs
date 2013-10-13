using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Window;
using EventLog = rglikeworknamelib.EventLog;
using IGameComponent = rglikeworknamelib.Window.IGameComponent;

namespace jarg {
    public partial class JargMain {
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
        private Button ButtonHudColor1, ButtonHudColor2, ButtonHudColor3, ButtonHudColor4, ButtonHudColor5;
        private Label LabelTimeType;
        private Button Button12h, Button24h;
        private Label LabelFullscreen;
        private Button ButtonFullscreenOn, ButtonFullscreenOff;
        private Label LabelFramelimit;
        private Button ButtonFramelimitOn, ButtonFramelimitOff;
        private Label LabelLight;
        private Button ButtonLightOn, ButtonLightOff;
        private Label LabelResolution;

        private Button ButtonResolution800600,
                       ButtonResolution1024768,
                       ButtonResolution1280800,
                       ButtonResolution19201024;

        private Label LabelOnlineRadio;
        private Button ButtonRadioGB, ButtonRadioOff;

        private Window WindowIngameMenu;
        private Label LabelIngameMenu1;
        private Button ButtonIngameMenuSettings;
        private Button ButtonIngameExit;

        private Window WindowUIButtons;
        private ImageButton IBInv, IBCaracter, IBBag;

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
        private ListContainer PerksContainer;
        private List<CheckBox> CheckBoxesPerks;

        private Window WindowInventory;
        private ListContainer ContainerInventoryItems;
        private LabelFixed InventoryMoreInfo;
        private Button InventorySortAll;
        private Button InventorySortMedicine;
        private Button InventorySortFood;
        private Button IntentoryEquip;
        private Label InventoryTotalWV;

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
        private DoubleLabel LabelCaracterGun;
        private DoubleLabel LabelCaracterMeele;
        private DoubleLabel LabelCaracterAmmo;
        private DoubleLabel LabelCaracterHp;
        private ListContainer ContainerWearList;
        private Label LabelWearCaption;
        private List<Label> LabelsWeared; 
        private List<Label> LabelsAbilities;
        private ListContainer EffectsContainer;
        private List<LabelFixed> LabelsEffect;

        private Window InfoWindow;
        private DoubleLabel InfoWindowLabel;

        private Window WindowStatist;
        private ListContainer ListStatist;

        private Window ConsoleWindow;
        private TextBox ConsoleTB;

        private Window WindowRadio;
        private RunningLabel LabelRadio;

        #endregion

        private void CreateWindows(WindowSystem ws) {
            Random rnd = new Random();

            WindowStats = new Window(new Rectangle(50, 50, 400, 400), "Stats", true, ws) {Visible = false};
            StatsHeat = new ProgressBar(new Rectangle(50, 50, 100, 20), "", WindowStats);
            StatsJajda = new ProgressBar(new Rectangle(50, 50 + 30, 100, 20), "", WindowStats);
            StatsHunger = new ProgressBar(new Rectangle(50, 50 + 30*2, 100, 20), "", WindowStats);
            CloseAllTestButton = new Button(new Vector2(10, 100), "Close all", WindowStats);
            CloseAllTestButton.OnPressed += CloseAllTestButton_onPressed;
            contaiter1 = new ListContainer(new Rectangle(200, 200, 100, 200), WindowStats);
            for (int i = 1; i < 20; i++) {
                contaiter1.AddItem(new Button(Vector2.Zero, rnd.Next(1, 1000).ToString(), WindowStats));
            }

            WindowMinimap = new Window(new Rectangle((int) Settings.Resolution.X - 180, 10, 128 + 20, 128 + 40),
                                       "minimap", false, ws) {NoBorder = true, Moveable = false};
            ImageMinimap = new Image(new Vector2(10, 10), new Texture2D(GraphicsDevice, 88, 88), Color.White,
                                     WindowMinimap);

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
            ButtonLightOn = new Button(new Vector2(10 + 50 + 40*2, 10 + 40*4), "On", WindowSettings);
            ButtonLightOn.OnPressed += ButtonLightOn_onPressed;
            ButtonLightOff = new Button(new Vector2(10 + 50 + 40*3, 10 + 40*4), "Off", WindowSettings);
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
            LabelOnlineRadio = new Label(new Vector2(10, 10 + 40*6), "Resolution", WindowSettings);
            ButtonRadioGB = new Button(new Vector2(10 + 50 + 95*1, 10 + 40*6), "GhostBox", WindowSettings);
            ButtonRadioGB.OnPressed += ButtonRadioGB_onPressed;
            ButtonRadioOff = new Button(new Vector2(10 + 50 + 95*2, 10 + 40*6), "  Off   ", WindowSettings);
            ButtonRadioOff.OnPressed += new EventHandler(ButtonRadioOff_onPressed);

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
            CheckBoxesPerks = new List<CheckBox>();
            for (int i = 0; i < PerkDataBase.Perks.Count; i++) {
                var keyValuePair = PerkDataBase.Perks.ElementAt(i);
                if (keyValuePair.Value.Initial) {
                    var t = new CheckBox(Vector2.Zero, keyValuePair.Value.Name, PerksContainer)
                            {Cheked = player_.Perks.IsSelected(keyValuePair.Key), Tag = keyValuePair.Key};
                    PerksContainer.AddItem(t);
                    t.OnPressed += Game1_onPressed;

                    CheckBoxesPerks.Add(t);
                }
            }

            WindowInventory =
                new Window(new Vector2(Settings.Resolution.X/2, Settings.Resolution.Y - Settings.Resolution.Y/10),
                           "Inventory", true, ws) {Visible = false};
            ContainerInventoryItems =
                new ListContainer(
                    new Rectangle(10, 10, WindowInventory.Locate.Width/2, WindowInventory.Locate.Height - 40),
                    WindowInventory);
            InventoryMoreInfo = new LabelFixed(new Vector2(WindowInventory.Locate.Width - 200, 40), "", 20,
                                               WindowInventory);
            InventorySortAll =
                new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200), "All", WindowInventory);
            InventorySortAll.OnPressed += InventorySortAll_onPressed;
            InventorySortMedicine =
                new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30),
                           "Medicine", WindowInventory);
            InventorySortMedicine.OnPressed += InventorySortMedicine_onPressed;
            InventorySortFood =
                new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30*2),
                           "Food", WindowInventory);
            InventorySortFood.OnPressed += InventorySortFood_onPressed;
            IntentoryEquip =
                new Button(new Vector2(WindowInventory.Locate.Width - 100, WindowInventory.Locate.Height - 200 + 30*2),
                           "Equip", WindowInventory);
            IntentoryEquip.OnPressed += IntentoryEquip_onPressed;
            InventoryTotalWV =
                new Label(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30*3),
                          "some weight" + Environment.NewLine + "some volume", WindowInventory);

            WindowContainer =
                new Window(new Vector2(Settings.Resolution.X/2, Settings.Resolution.Y - Settings.Resolution.Y/10),
                           "Container", true, ws) {Visible = false};
            WindowContainer.SetPosition(new Vector2(Settings.Resolution.X/2, 0));
            ContainerContainer =
                new ListContainer(
                    new Rectangle(10, 10, WindowInventory.Locate.Width/2, WindowInventory.Locate.Height - 40),
                    WindowContainer);
            LabelContainer = new LabelFixed(new Vector2(WindowInventory.Locate.Width - 200, 40), "", 20,
                                            WindowContainer);
            ButtonContainerTakeAll =
                new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30*2),
                           "Take All (R)", WindowContainer);
            ButtonContainerTakeAll.OnPressed += ButtonContainerTakeAll_onPressed;

            WindowEventLog =
                new Window(new Rectangle(3, 570, (int) Settings.Resolution.X/3, (int) Settings.Resolution.Y/4), "Log",
                           true, ws_) {Visible = false, Closable = false, hides = true};
            ContainerEventLog =
                new ListContainer(
                    new Rectangle(0, 0, (int) Settings.Resolution.X/3, (int) Settings.Resolution.Y/4 - 20),
                    WindowEventLog);
            EventLog.OnLogUpdate += EventLog_onLogUpdate;

            WindowIngameHint = new Window(new Vector2(50, 50), "HINT", false, ws)
                               {NoBorder = true, Visible = false};
            LabelIngameHint = new Label(new Vector2(10, 3), "a-ha", WindowIngameHint);

            WindowGlobal = new Window(new Vector2(Settings.Resolution.X - 100, Settings.Resolution.Y - 50), "Map", true, ws) {Visible = false};
            ImageGlobal = new Image(new Vector2(10, 10), new Texture2D(GraphicsDevice, 10, 10), Color.White,
                                    WindowGlobal);

            int ii = 0;
            WindowCaracter =
                new Window(new Vector2(Settings.Resolution.X/3*2, Settings.Resolution.Y - Settings.Resolution.Y/10),
                           "Caracter info", true, ws) {Visible = false};
            ii++;
            LabelCaracterGun = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Ranged Weapon : ", WindowCaracter);
            ii++;
            LabelCaracterMeele = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Meele Weapon : ", WindowCaracter);
            ii++;
            LabelCaracterAmmo = new DoubleLabel(new Vector2(10, 10 + 15*ii), "Ammo : ", WindowCaracter);
            ii+=2;
            LabelWearCaption = new Label(new Vector2(10, 10 + 15 * ii), "Wear", WindowCaracter);
            ii+=2;
            ContainerWearList = new ListContainer(new Rectangle(0, 10 + 15 * ii, (int)WindowCaracter.Width / 2, (int)WindowCaracter.Height / 2 - (10 + 15 * ii)), WindowCaracter);
            LabelsWeared = new List<Label>();
            ii = 0;
            LabelCaracterHp = new DoubleLabel(new Vector2(10 + 300, 10 + 15*ii), "HP : ", WindowCaracter);
            LabelsAbilities = new List<Label>();
            for (int i = 0; i < 11; i++) {
                Label temp = new Label(new Vector2(10, 400 + 15*ii), "", WindowCaracter);
                ii++;
                LabelsAbilities.Add(temp);
            }
            EffectsContainer =
                new ListContainer(
                    new Rectangle((int) WindowCaracter.Width/2, (int) (WindowCaracter.Height/2),
                                  (int) (WindowCaracter.Width/2), (int) (WindowCaracter.Height/2) - 19),
                    WindowCaracter);
            LabelsEffect = new List<LabelFixed>();
            

            InfoWindow = new Window(new Vector2(200, 100), "Info", true, ws) {Visible = false};
            InfoWindowLabel = new DoubleLabel(new Vector2(20, 20), "some info", InfoWindow);

            WindowStatist = new Window(new Vector2(Settings.Resolution.X/3, Settings.Resolution.Y/3), "Statistic", true, ws) {Visible = false};
            ListStatist =
                new ListContainer(
                    new Rectangle(0, 0, (int) Settings.Resolution.X/3, (int) Settings.Resolution.Y/3 - 20),
                    WindowStatist);

            ConsoleWindow = new Window(new Vector2(Settings.Resolution.X/3, Settings.Resolution.Y/3), "Concole", true, ws) {Visible = false};
            ConsoleTB = new TextBox(new Vector2(10, 100), 200, ConsoleWindow);
            ConsoleTB.OnEnter += ConsoleTB_onEnter;
            ConsoleWindow.CenterComponentHor(ConsoleTB);

            WindowRadio =
                new Window(
                    new Rectangle((int) (Settings.Resolution.X/2 - Settings.Resolution.X/6), -23,
                                  (int) (Settings.Resolution.X/6*2), (int) (Settings.Resolution.Y/15)), "radio", false, ws) {Moveable = false};
            LabelRadio = new RunningLabel(new Vector2(0, 8), "radio string radio string radio string radio string",
                                          ((int) (Settings.Resolution.X/6*2) - 10)/9, WindowRadio);
            WindowRadio.CenterComponentHor(LabelRadio);
        }

        private void IBInv_onPressed(object sender, EventArgs e) {
            WindowGlobal.Visible = !WindowGlobal.Visible;
            if (WindowGlobal.Visible) {
                WindowGlobal.OnTop();
            }
        }

        private void IBCaracter_onPressed(object sender, EventArgs e) {
            WindowCaracter.Visible = !WindowCaracter.Visible;
            if (WindowCaracter.Visible) {
                WindowCaracter.OnTop();
            }
        }

        private void IBBag_onPressed(object sender, EventArgs e) {
            WindowInventory.Visible = !WindowInventory.Visible;
            if (WindowInventory.Visible) {
                WindowInventory.OnTop();
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


        private void ButtonLightOff_onPressed(object sender, EventArgs e) {
            Settings.Lighting = false;
        }

        private void ButtonLightOn_onPressed(object sender, EventArgs e) {
            Settings.Lighting = true;
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

        private void ConsoleTB_onEnter(object sender, EventArgs e) {
            string s = ConsoleTB.Text;
            ConsoleTB.Tag = "";

            if (s.Contains("spawn c ")) {
                string ss = s.Substring(8);
                if (MonsterDataBase.Data.ContainsKey(ss)) {
                    var pp = player_.GetWorldPositionInBlocks();
                    pp.X = (int) pp.X;
                    pp.Y = (int) pp.Y;
                    var ppp = currentFloor_.GetInSectorPosition(pp);
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
                var pp = player_.GetWorldPositionInBlocks();
                pp.X = (int) pp.X;
                pp.Y = (int) pp.Y;
                var ppp = currentFloor_.GetInSectorPosition(pp);
                var i = (int)(player_.Position.X - (int)ppp.X * MapSector.Rx * 32);
                var i1 = (int)(player_.Position.Y - (int)ppp.Y * MapSector.Ry * 32);
                EventLog.Add(
                    string.Format("Player position {4} or ({0}, {1}) in sector ({2}, {3})", i, i1, (int) ppp.X,
                                  (int) ppp.Y, player_.Position), GlobalWorldLogic.CurrentTime, Color.Cyan,
                    LogEntityType.Console);
            }
            if (s.Contains("fastwalk")) {
                acmodifer = acmodifer == 1 ? 4 : 1;
                EventLog.Add(string.Format("walk x{0}", acmodifer), GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
            }
            if (s.Contains("noclip")) {
                Settings.Noclip = !Settings.Noclip;
                EventLog.Add(string.Format("Noclip = {0}", Settings.Noclip), GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
            }
            if (s.Contains("killall"))
            {
                var t = currentFloor_.KillAllCreatures();
                EventLog.Add(string.Format("{0} creatures in active sector killed!", t), GlobalWorldLogic.CurrentTime, Color.Cyan, LogEntityType.Console);
            }
            if(s.Contains("spawn i ")) {
                string ss = s.Substring(8);
                if (ItemDataBase.Data.ContainsKey(ss)) {
                    inventory_.AddItem(new Item(ss, 1));
                    inventory_.StackSimilar();
                    UpdateInventoryContainer();
                } else {
                    EventLog.Add(string.Format("Item {0} not found", ss), GlobalWorldLogic.CurrentTime, Color.Cyan,
                                 LogEntityType.Console);
                }
            }
            ConsoleTB.Text = string.Empty;
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
            foreach (var ss in EventLog.log) {
                ContainerEventLog.AddItem(new LabelFixed(Vector2.Zero, ss.message, ss.col, 35,
                                                         ContainerEventLog));
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

        private ItemType nowSort_ = ItemType.Nothing;
        private List<Item> inInv_ = new List<Item>();

        private void UpdateInventoryContainer() {
            selectedItem = null;
            InventoryMoreInfo.Text = "";

            var a = inventory_.FilterByType(nowSort_);
            inInv_ = a;

            ContainerInventoryItems.Clear();

            int cou = 0;
            foreach (var item in a) {
                var i = new LabelFixed(Vector2.Zero, item.ToString(), 22, ContainerInventoryItems);
                i.Tag = cou;
                i.OnPressed += PressInInventory;
                cou++;
                ContainerInventoryItems.AddItem(i);
            }
            InventoryTotalWV.Text = string.Format("Weight {0}/{1}{2}Volume {3}/{4}", inventory_.TotalWeight,
                                                  player_.MaxWeight, Environment.NewLine,
                                                  inventory_.TotalVolume, player_.MaxVolume);
        }

        private List<Item> inContainer_ = new List<Item>();
        private Vector2 containerOn ;

        private void UpdateContainerContainer(List<Item> a) {
            inContainer_ = a;

            ContainerContainer.Clear();

            int cou = 0;
            foreach (var item in a) {
                var i = new LabelFixed(Vector2.Zero, item.ToString(), 22, ContainerContainer);
                i.Tag = cou;
                i.OnPressed += PressInContainer;
                cou++;
                ContainerContainer.AddItem(i);
            }
        }

        private Item selectedItem;

        private void PressInInventory(object sender, EventArgs e) {
            var a = (int) (sender as Label).Tag;
            selectedItem = inInv_[a];

            if (!doubleclick_) {
                InventoryMoreInfo.Text = ItemDataBase.GetItemFullDescription(inInv_[a]);
            }
            else {
                IntentoryEquip_onPressed(null, null);
            }
        }

        private Item ContainerSelected;

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
            WindowEventLog.Visible = true;
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

        private TimeSpan SecondTimespan;

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

            if (Settings.InventoryUpdate) {
                UpdateInventoryContainer();
                Settings.InventoryUpdate = false;
            }

            if (SecondTimespan.TotalSeconds >= 1) {
                ListStatist.Clear();
                foreach (var statist in Achievements.Stat) {
                    if (statist.Value.Count != 0) {
                        ListStatist.AddItem(new Label(Vector2.Zero, statist.Value.Name + ": " + statist.Value.Count, ListStatist));
                    }
                }

                if (WindowCaracter.Visible)
                {
                    UpdateCaracterWindowItems(null, null);
                }
                
            }
        }

        private void UpdateCaracterWindowItems(object sender, EventArgs eventArgs) {
            LabelCaracterHp.Text2 = string.Format("{0}/{1}", player_.Hp.Current, player_.Hp.Max);

            for (int i = 0; i < LabelsAbilities.Count; i++)
            {
                LabelsAbilities[i].Text = string.Format("{0} {1} ({2}/{3})", player_.Abilities.ToShow[i].Name, player_.Abilities.ToShow[i], (int)player_.Abilities.ToShow[i].XpCurrent, Ability.XpNeeds[player_.Abilities.ToShow[i].XpLevel]);
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
            LabelsEffect.Clear();
            for (int i = 0; i < player_.Buffs.Count; i++) {
                LabelFixed label;
                if (player_.Buffs[i].Expiring) {
                    label = new LabelFixed(Vector2.Zero,
                                           string.Format("{0} {1}", BuffDataBase.Data[player_.Buffs[i].Id].Name,
                                                         player_.Buffs[i].Expire), 20, EffectsContainer);
                }
                else {
                    label = new LabelFixed(Vector2.Zero,
                                           string.Format("{0}", BuffDataBase.Data[player_.Buffs[i].Id].Name), 20, EffectsContainer);
                }
                LabelsEffect.Add(label);
                EffectsContainer.AddItem(label);
            }

            ContainerWearList.Clear();
            LabelsWeared.Clear();
            foreach (var item in player_.Weared) {
                var label = new LabelFixed(Vector2.Zero, string.Format("{0}", item.Data.Name), 20, ContainerWearList);
                LabelsWeared.Add(label);
                ContainerWearList.AddItem(label);
            }
        }
    }
}