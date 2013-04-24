using System.Collections.Generic;
using System.Collections.ObjectModel;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mork;
using System;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Parser;

namespace jarg
{
    public class Game1 : Game
    {
        private BlockDataBase bdb_;
        private FloorDataBase fdb_;
        private MonsterDataBase mdb_;
        private SchemesDataBase sdb_;
        private ItemDataBase idb_;

        private GraphicsDeviceManager graphics_;
        private Vector2 camera_;
        private GameLevel currentFloor_;
        private SpriteFont font1_;
        private KeyboardState ks_;
        private Vector3 lastPos_;
        private LineBatch lineBatch_;
        private KeyboardState lks_;
        private MouseState lms_;
        private MonsterSystem monsterSystem_;
        private MouseState ms_;
        private Vector2 pivotpoint_;
        private Player player_;
        private SpriteBatch spriteBatch_;

        private Manager manager_;

        private RenderTarget2D screenShadows_;
        private LightArea lightArea_;
        private ShadowmapResolver shadowmapResolver_;
        private QuadRenderComponent quadRender_;

        private GamePhase gamePhase_ = GamePhase.testgame;

        enum GamePhase
        {
            testgame
        }

        public Game1()
        {
            graphics_ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Log.Init();

            Settings.Resolution = new Vector2(1024, 768);
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
            graphics_.PreferredBackBufferWidth = (int)Settings.Resolution.X;
            graphics_.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = true;
            IsMouseVisible = true;
            graphics_.ApplyChanges();

            lightArea_ = new LightArea(GraphicsDevice, ShadowmapSize.Size512);
            screenShadows_ = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                                                GraphicsDevice.Viewport.Height);
            quadRender_ = new QuadRenderComponent(this);
            this.Components.Add(quadRender_);


            base.Initialize();
        }

        #region WindowsDesiner

        private Window mainmenu_;
        private Button mainmenucloseB_;

        private Window ingameUiPartOne_;

        private Window ingameMainInventory_;
        private ImageBox[] ingameMainInventoryImages_;
        private ImageBox ingameMainInventoryItemSelectBox_;
        private StorageBlock playerInventory_ = new StorageBlock();

        private Window ingameOpenContainer_;
        private ListBox ingameOpenContainerLixtBox_;
        private ImageBox[] ingameOpenContainerImages_;
        private ImageBox ingameOpenContainerItemSelectBox_;
        private Button ingameOpenContainerTakeAll_;
        private Button ingameOpenContainerTakeSelected_;

        private Window hintWindow_;
        private Label hintLabel_;

        private void ImageGridFromItemArray(ImageBox[] boxes, StorageBlock container, Window parent)
        {
            for (int i = 0; i < container.storedItems.Count; i++) {
                if (i < boxes.Length) {
                    boxes[i].Image = idb_.texatlas[idb_.data[container.storedItems[i].id].mtex];
                    boxes[i].Text = container.storedItems[i].count.ToString();
                    boxes[i].Tag = container.storedItems[i];
                    boxes[i].ToolTip.Text = "    " + idb_.data[container.storedItems[i].id].name + "\n" + idb_.data[container.storedItems[i].id].description;
                } else {
                    string s = "";
                    foreach (var item in container.storedItems) {
                        s += idb_.data[item.id].name + " x" + item.count + " \n";
                    }
                    Log.LogError("itemcount > boxcount \n\n"+ s);
                }
            }
            parent.Tag = container;
        }

        /// <summary>
        /// Код дизайнера всех базовых окон
        /// </summary>
        private void WindowsDesigner()
        {
            manager_ = new Manager(this, graphics_, "Default");
            manager_.Initialize();

            #region mainmenu

            mainmenu_ = new Window(manager_) {
                BackColor = Color.Black
            };
            mainmenu_.Init();
            mainmenu_.Text = "";
            mainmenu_.SetPosition((int)Settings.Resolution.X / 3, (int)Settings.Resolution.Y / 4);
            mainmenu_.Width = (int)Settings.Resolution.X / 3;
            mainmenu_.Height = (int)Settings.Resolution.Y / 2;
            mainmenu_.Visible = false;
            mainmenu_.BorderVisible = false;
            mainmenu_.Movable = false;
            mainmenu_.Resizable = false;

            mainmenucloseB_ = new Button(manager_);
            mainmenucloseB_.Init();
            mainmenucloseB_.Text = "Quit";
            mainmenucloseB_.Width = (int)Settings.Resolution.X / 5;
            mainmenucloseB_.Height = 25;
            mainmenucloseB_.Left = ((int)Settings.Resolution.X / 3 - (int)Settings.Resolution.X / 5) / 2;
            mainmenucloseB_.Top = mainmenu_.ClientHeight - mainmenucloseB_.Height - 8;
            mainmenucloseB_.Anchor = Anchors.Bottom;
            mainmenucloseB_.Parent = mainmenu_;
            mainmenucloseB_.Click += mainmenucloseB_Click;

            manager_.Add(mainmenu_);

            #endregion

            #region ingameUiPartOne

            ingameUiPartOne_ = new Window(manager_) {
                BackColor = Color.Black
            };
            ingameUiPartOne_.Init();
            ingameUiPartOne_.Text = "";
            ingameUiPartOne_.SetPosition((int)Settings.Resolution.X - 200, 0);
            ingameUiPartOne_.Width = 200;
            ingameUiPartOne_.Height = (int)Settings.Resolution.Y;
            ingameUiPartOne_.Visible = true;
            ingameUiPartOne_.BorderVisible = false;
            ingameUiPartOne_.Movable = false;
            ingameUiPartOne_.Resizable = false;

            mainmenucloseB_ = new Button(manager_);
            mainmenucloseB_.Init();
            mainmenucloseB_.Text = "Quit";
            mainmenucloseB_.Width = (int)Settings.Resolution.X / 5;
            mainmenucloseB_.Height = 25;
            mainmenucloseB_.Left = ((int)Settings.Resolution.X / 3 - (int)Settings.Resolution.X / 5) / 2;
            mainmenucloseB_.Top = mainmenu_.ClientHeight - mainmenucloseB_.Height - 8;
            mainmenucloseB_.Anchor = Anchors.Bottom;
            mainmenucloseB_.Parent = mainmenu_;
            mainmenucloseB_.Click += mainmenucloseB_Click;

            manager_.Add(ingameUiPartOne_);


            #endregion

            #region ingameMainInventory

            ingameMainInventory_ = new Window(manager_) {
                BackColor = Color.Black
            };
            ingameMainInventory_.Init();
            ingameMainInventory_.Text = "";
            ingameMainInventory_.SetPosition(50, 50);
            ingameMainInventory_.Width = (int)Settings.Resolution.X / 2 - 100;
            ingameMainInventory_.Height = (int)Settings.Resolution.Y - 100;
            ingameMainInventory_.Visible = false;
            ingameMainInventory_.BorderVisible = true;
            ingameMainInventory_.Movable = false;
            ingameMainInventory_.Resizable = false;

            playerInventory_.storedItems = new List<Item>();

            ingameMainInventoryItemSelectBox_ = new ImageBox(manager_);
            ingameMainInventoryItemSelectBox_.Init();
            ingameMainInventoryItemSelectBox_.SetSize(36, 36);
            ingameMainInventoryItemSelectBox_.SetPosition(-100, -100);
            ingameMainInventoryItemSelectBox_.Parent = ingameMainInventory_;
            ingameMainInventoryItemSelectBox_.Image = itemSelectTex_;

            ingameMainInventoryImages_ = new ImageBox[64];
            for (int i = 0; i < ingameMainInventoryImages_.Length; i++) {
                ingameMainInventoryImages_[i] = new ImageBox(manager_);
                ingameMainInventoryImages_[i].Init();
                ingameMainInventoryImages_[i].SetSize(32, 32);
                ingameMainInventoryImages_[i].SetPosition(10 + i % 8 * 36, 10 + i / 8 * 36);
                ingameMainInventoryImages_[i].Parent = ingameMainInventory_;
                ingameMainInventoryImages_[i].Click += Game2_Click;
                ingameMainInventoryImages_[i].DoubleClick += new TomShane.Neoforce.Controls.EventHandler(Game2_DoubleClick);
            }

            manager_.Add(ingameMainInventory_);

            #endregion

            #region hintWindow

            hintWindow_ = new Window(manager_)
            {
                BackColor = Color.Black
            };
            hintWindow_.Init();
            hintWindow_.Text = "";
            hintWindow_.SetPosition(50, 50);
            hintWindow_.Width = 100;
            hintWindow_.Height = 30;
            hintWindow_.Visible = false;
            hintWindow_.BorderVisible = false;
            hintWindow_.Movable = false;
            hintWindow_.Resizable = false;

            hintLabel_ = new Label(manager_);
            hintLabel_.Init();
            hintLabel_.Text = "";
            hintLabel_.SetPosition(5,5);
            hintLabel_.Parent = hintWindow_;

            manager_.Add(hintWindow_);

            #endregion

            #region ingameOpenContainer

            ingameOpenContainer_ = new Window(manager_)
            {
                BackColor = Color.Black
            };
            ingameOpenContainer_.Init();
            ingameOpenContainer_.Text = "";
            ingameOpenContainer_.SetPosition(50 + (int)Settings.Resolution.X / 2 - 100, 50);
            ingameOpenContainer_.Width = (int)Settings.Resolution.X / 2 - 100;
            ingameOpenContainer_.Height = (int)Settings.Resolution.Y - 100;
            ingameOpenContainer_.Visible = false;
            ingameOpenContainer_.BorderVisible = true;
            ingameOpenContainer_.Movable = false;
            ingameOpenContainer_.Resizable = false;

            ingameOpenContainerLixtBox_ = new ListBox(manager_);
            ingameOpenContainerLixtBox_.Init();
            ingameOpenContainerLixtBox_.Width = ingameOpenContainer_.Width - 30;
            ingameOpenContainerLixtBox_.Height = ingameOpenContainer_.Height/3*2;
            ingameOpenContainerLixtBox_.Left = 10;
            ingameOpenContainerLixtBox_.Top = 10;
            ingameOpenContainerLixtBox_.Parent = ingameOpenContainer_;

            ingameOpenContainerItemSelectBox_ = new ImageBox(manager_);
            ingameOpenContainerItemSelectBox_.Init();
            ingameOpenContainerItemSelectBox_.SetSize(36, 36);
            ingameOpenContainerItemSelectBox_.SetPosition(-100, -100);
            ingameOpenContainerItemSelectBox_.Parent = ingameOpenContainer_;
            ingameOpenContainerItemSelectBox_.Image = itemSelectTex_;

            ingameOpenContainerTakeAll_ = new Button(manager_);
            ingameOpenContainerTakeAll_.Parent = ingameOpenContainer_;
            ingameOpenContainerTakeAll_.SetSize(100,20);
            ingameOpenContainerTakeAll_.SetPosition(ingameOpenContainer_.Width / 2 - ingameOpenContainerTakeAll_.Width / 2, ingameOpenContainer_.Height - ingameOpenContainerTakeAll_.Height*2 - 20);
            ingameOpenContainerTakeAll_.Text = "<< Взять все";
            ingameOpenContainerTakeAll_.Click += new TomShane.Neoforce.Controls.EventHandler(ingameOpenContainerTakeAll__Click);

            ingameOpenContainerImages_ = new ImageBox[16];
            for (int i = 0; i < ingameOpenContainerImages_.Length; i++) {
                ingameOpenContainerImages_[i] = new ImageBox(manager_);
                ingameOpenContainerImages_[i].Init();
                ingameOpenContainerImages_[i].SetSize(32, 32);
                ingameOpenContainerImages_[i].SetPosition(10 + i % 8 * 36, 10 + i / 8 * 36);
                ingameOpenContainerImages_[i].Parent = ingameOpenContainer_;
                ingameOpenContainerImages_[i].Click += Game1_Click;
                ingameOpenContainerImages_[i].DoubleClick += new TomShane.Neoforce.Controls.EventHandler(TakeItemFromTaggedContainer);
            }

            manager_.Add(ingameOpenContainer_);
            #endregion
        }

        void ingameOpenContainerTakeAll__Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            foreach (var box in ingameOpenContainerImages_) {
                if(box.Tag != null) {
                TakeItemFromTaggedContainer(box, e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">(Image Box).Tag = Item (Image Box).Parent.Parent.Tag = Container</param>
        /// <param name="e"></param>
        void TakeItemFromTaggedContainer(object sender, TomShane.Neoforce.Controls.EventArgs e) {
            if (playerInventory_.storedItems.Count < ingameMainInventoryImages_.Length - 1) {
                var box = (ImageBox) sender;
                box.Image = transparentPixel_;
                var ite = (Item) (box).Tag;

                var sblock = ((StorageBlock) box.Parent.Parent.Tag);
                playerInventory_.storedItems.Add(ite);
                sblock.storedItems.Remove(ite);
                box.Tag = null;
                ImageGridFromItemArray(ingameMainInventoryImages_, playerInventory_, ingameMainInventory_);
            }
        }

        void Game1_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ingameOpenContainerItemSelectBox_.SetPosition(((ImageBox)sender).Left - 2, ((ImageBox)sender).Top - 2);
            ingameOpenContainerItemSelectBox_.Tag = ((ImageBox) sender).Tag;
        }

        void Game2_DoubleClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
        }

        void Game2_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ingameMainInventoryItemSelectBox_.SetPosition(((ImageBox)sender).Left - 2, ((ImageBox)sender).Top - 2);
            ingameMainInventoryItemSelectBox_.Tag = ((ImageBox)sender).Tag;
        }

        void mainmenucloseB_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ;
        }
        #endregion

        private Texture2D itemSelectTex_;
        private Texture2D transparentPixel_;
        protected override void LoadContent()
        {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            itemSelectTex_ = Content.Load<Texture2D>(@"Textures/Dungeon/Items/itemselect");
            transparentPixel_ = Content.Load<Texture2D>(@"Textures/transparent_pixel");

            bdb_ = new BlockDataBase();
            fdb_ = new FloorDataBase();
            mdb_ = new MonsterDataBase();
            sdb_ = new SchemesDataBase();
            idb_ = new ItemDataBase(ParsersCore.LoadTexturesInOrder(Settings.GetItemDataDirectory() + @"/textureloadorder.ord", Content));

            font1_ = Content.Load<SpriteFont>(@"Fonts/Font1");

            var tex = new Collection<Texture2D> {
                                           Content.Load<Texture2D>(@"Textures/transparent_pixel"),
                                           Content.Load<Texture2D>(@"Textures/Units/car")
                                        };
            monsterSystem_ = new MonsterSystem(spriteBatch_, tex);


            currentFloor_ = GameLevel.CreateGameLevel(spriteBatch_, 
                                                      ParsersCore.LoadTexturesInOrder(Settings.GetFloorDataDirectory() + @"/textureloadorder.ord", Content),
                                                      ParsersCore.LoadTexturesInOrder(Settings.GetObjectDataDirectory() + @"/textureloadorder.ord", Content), 
                                                      bdb_, 
                                                      fdb_, 
                                                      sdb_
                                                     );

            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_) {
            };

            shadowmapResolver_ = new ShadowmapResolver(GraphicsDevice, quadRender_, ShadowmapSize.Size256, ShadowmapSize.Size32);
            shadowmapResolver_.LoadContent(Content);

            WindowsDesigner();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public static bool ErrorExit;
        public float seeAngleDeg = 60;
        protected override void Update(GameTime gameTime)
        {
            if (ErrorExit) Exit();
            lineBatch_.Clear();

            base.Update(gameTime);

            KeyboardUpdate(gameTime);
            MouseUpdate(gameTime);

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

            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);

            if (Settings.DebugInfo) {
                FrameRateCounter.Update(gameTime);
            }
            manager_.Update(gameTime);
        }

        private void KeyboardUpdate(GameTime gameTime)
        {
            lks_ = ks_;
            ks_ = Keyboard.GetState();

            if (ks_[Keys.F4] == KeyState.Down && lks_[Keys.F4] == KeyState.Up) {
                Settings.DebugInfo = !Settings.DebugInfo;
            }

            if (ks_[Keys.W] == KeyState.Down) {
                player_.Accelerate(new Vector3(0, -10, 0));
            }
            if (ks_[Keys.S] == KeyState.Down) {
                player_.Accelerate(new Vector3(0, 10, 0));
            }
            if (ks_[Keys.A] == KeyState.Down) {
                player_.Accelerate(new Vector3(-10, 0, 0));
            }
            if (ks_[Keys.D] == KeyState.Down) {
                player_.Accelerate(new Vector3(10, 0, 0));
            }

            if (ks_[Keys.I] == KeyState.Down && lks_[Keys.I] == KeyState.Up) {
                ingameMainInventory_.Visible = !ingameMainInventory_.Visible;
            }

            if(ks_[Keys.T] == KeyState.Down && lks_[Keys.T] == KeyState.Up && ingameOpenContainer_.Visible) {
                ingameOpenContainerTakeAll__Click(ingameOpenContainerTakeAll_, null);
            }

            pivotpoint_ = new Vector2(player_.Position.X - (Settings.Resolution.X - 200) / 2, player_.Position.Y - Settings.Resolution.Y / 2);

            if (ks_[Keys.Escape] == KeyState.Down && lks_[Keys.Escape] == KeyState.Up) {
                if (ingameOpenContainer_.Visible) {
                    ingameOpenContainer_.Visible = false;
                } else if (ingameMainInventory_.Visible) {
                    ingameMainInventory_.Visible = false;
                    return;
                } else ; //empty operator
            }
        }

        private void MouseUpdate(GameTime gameTime)
        {
            lms_ = ms_;
            ms_ = Mouse.GetState();

            int aa = (ms_.X + (int)camera_.X) / 32 * currentFloor_.ry + (ms_.Y + (int)camera_.Y) / 32;
            if (aa >= 0 && currentFloor_.blocks_[aa].id != 0 && currentFloor_.blocks_[aa].explored)
            {
                int nx = (ms_.X + (int)camera_.X) / 32;
                int ny = (ms_.Y + (int)camera_.Y) / 32;
                var a = currentFloor_.blocks_[nx * currentFloor_.ry + ny];
                var b = bdb_.Data[a.id];
                hintWindow_.Visible = true;
                hintWindow_.Top = ms_.Y + 15;
                hintWindow_.Left = ms_.X + 15;
                string s = Block.GetSmartActionName(b.smartAction) + " " + b.name;
                if (Settings.DebugInfo) s += " id" + a.id + " tex" + b.texNo;
                hintLabel_.Text = s;
                hintLabel_.Width = (int)font1_.MeasureString(s).X;
                hintWindow_.Width = hintLabel_.Width + 10;
                hintWindow_.Height = (int)font1_.MeasureString(s).Y - 10;

                if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released && currentFloor_.IsCreatureMeele(nx, ny, player_)) {
                    var undermouseblock = bdb_.Data[a.id];
                    if (undermouseblock.smartAction == SmartAction.ActionOpenContainer) {
                        ingameOpenContainer_.Visible = true;
                        ImageGridFromItemArray(ingameOpenContainerImages_, (a as StorageBlock), ingameOpenContainer_);
                    }

                    if(undermouseblock.smartAction == SmartAction.ActionOpenClose) {
                       currentFloor_.OpenCloseDoor(nx, ny);
                    }
                }
            }
            else
            {
                hintWindow_.Visible = false;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            manager_.BeginDraw(gameTime);
            manager_.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);

            //lightArea.LightPosition = player_.InScreenPosition - camera_;
            //lightArea.BeginDrawingShadowCasters();
            //_spriteBatch.Begin();
            //_currentFloor.Draw2(gameTime, camera_ - lightArea.ToRelativePosition(Vector2.Zero));
            //_spriteBatch.End();
            //lightArea.EndDrawingShadowCasters();
            //shadowmapResolver.ResolveShadows(lightArea.RenderTarget, lightArea.RenderTarget, player_.InScreenPosition - camera_);

            //GraphicsDevice.SetRenderTarget(screenShadows);
            //GraphicsDevice.Clear(Color.Black);
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //_spriteBatch.Draw(lightArea.RenderTarget, lightArea.LightPosition - lightArea.LightAreaSize * 0.5f, Color.White);
            ////_spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(null);

            //BlendState blendState = new BlendState();
            //blendState.ColorSourceBlend = Blend.DestinationColor;
            //blendState.ColorDestinationBlend = Blend.SourceColor;

            spriteBatch_.Begin();
            currentFloor_.Draw(gameTime, camera_);
            currentFloor_.Draw2(gameTime, camera_);
            player_.Draw(gameTime, camera_);
            spriteBatch_.End();
            manager_.EndDraw();

            //_spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            //_spriteBatch.Draw(screenShadows, Vector2.Zero, Color.White);
            //_spriteBatch.End();


            base.Draw(gameTime);

            if (Settings.DebugInfo) {
                DebugInfoDraw(gameTime);
            }

            lineBatch_.Draw();
        }

        private void DebugInfoDraw(GameTime gameTime)
        {
            FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int)Settings.Resolution.X,
                                  (int)Settings.Resolution.Y);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, string.Format("{0}\n{1}\n{2}", (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X),
                                                                           (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X) + MathHelper.ToRadians(60),
                                                                           (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X) - MathHelper.ToRadians(60)), new Vector2(500, 10), Color.White);

            spriteBatch_.End();
        }
    }
}