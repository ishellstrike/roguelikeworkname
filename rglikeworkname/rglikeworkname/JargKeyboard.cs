using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Vehicles;

namespace jarg {
    public partial class JargMain {
        private readonly Vector2 acA = new Vector2(-10, 0);
        private readonly Vector2 acD = new Vector2(10, 0);
        private readonly Vector2 acS = new Vector2(0, 10);
        private readonly Vector2 acW = new Vector2(0, -10);
        private float acmodifer = 1;
        private Vehicle car;
        private int iii;
        private Vehicle veh;

        private void UpdateKeyboard(GameTime gameTime) {
            lks_ = ks_;
            ks_ = Keyboard.GetState();
            if (!ws_.Keyboardhook) {
                if (ks_[Keys.W] == KeyState.Down) {
                    player_.Accelerate(acW*acmodifer);
                }
                if (ks_[Keys.S] == KeyState.Down) {
                    player_.Accelerate(acS*acmodifer);
                }
                if (ks_[Keys.A] == KeyState.Down) {
                    player_.Accelerate(acA*acmodifer);
                }
                if (ks_[Keys.D] == KeyState.Down) {
                    player_.Accelerate(acD*acmodifer);
                }

                if (ks_[Keys.M] == KeyState.Down && lks_[Keys.M] == KeyState.Up) {
                    WindowGlobal.Visible = !WindowGlobal.Visible;
                    if (WindowGlobal.Visible) {
                        currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_, mousemapoffset);
                        ImageGlobal.image = currentFloor_.GetMap();
                    }
                }

                if (ks_[Keys.C] == KeyState.Down && lks_[Keys.C] == KeyState.Up) {
                    CaracterWindow.Visible = !CaracterWindow.Visible;
                    if (CaracterWindow.Visible) {
                        CaracterWindow.OnTop();
                    }
                    UpdateCaracterWindowItems(null, null);
                }

                if (ks_[Keys.V] == KeyState.Down && lks_[Keys.V] == KeyState.Up)
                {
                    CraftWindow.Visible = !CraftWindow.Visible;
                    if (CraftWindow.Visible) {
                        CraftWindow.OnTop();
                    }
                    Update_Craft_Items();
                }

                if (ks_[Keys.I] == KeyState.Down && lks_[Keys.I] == KeyState.Up) {
                    InventoryWindow.Visible = !InventoryWindow.Visible;
                    if (InventoryWindow.Visible) {
                        InventoryWindow.OnTop();
                        InventoryWindow.SetPosition(Vector2.Zero);
                    }
                }

                if (ks_[Keys.L] == KeyState.Down && lks_[Keys.L] == KeyState.Up) {
                    EventLogWindow.Visible = !EventLogWindow.Visible;
                    if (EventLogWindow.Visible) {
                        EventLogWindow.OnTop();
                    }
                }

                if (ks_[Keys.R] == KeyState.Down && lks_[Keys.R] == KeyState.Up)
                {
                    MoraleWindow.Visible = !MoraleWindow.Visible;
                    if (MoraleWindow.Visible)
                    {
                        MoraleWindow.OnTop();
                    }
                }

                if (ks_[Keys.O] == KeyState.Down && lks_[Keys.O] == KeyState.Up) {
                    StatistWindow.Visible = !StatistWindow.Visible;
                    if (StatistWindow.Visible) {
                        StatistWindow.OnTop();
                    }
                }

                if (ks_[Keys.P] == KeyState.Down && lks_[Keys.P] == KeyState.Up) {
                    StatistWindow.Visible = !StatistWindow.Visible;
                    if (StatistWindow.Visible) {
                        StatistWindow.OnTop();
                    }
                }

                if (ks_[Keys.F] == KeyState.Down && lks_[Keys.F] == KeyState.Up) {
                    Flashlight = !Flashlight;
                }

                if (WindowContainer.Visible && ks_[Keys.R] == KeyState.Down && lks_[Keys.R] == KeyState.Up) {
                    ButtonContainerTakeAll_onPressed(null, null);
                    WindowContainer.Visible = false;
                }

                if (ks_[Keys.K] == KeyState.Down && lks_[Keys.K] == KeyState.Up)
                {
                    CraftWindow.Visible = !CraftWindow.Visible;
                    if (CraftWindow.Visible) {
                        CraftWindow.OnTop();
                    }
                }

                if (player_ != null) {
                    pivotpoint_ = Vector2.Subtract(player_.Position, Vector2.Divide(Settings.Resolution, 2));
                }
            }

            if (ks_[Keys.Escape] == KeyState.Down && lks_[Keys.Escape] == KeyState.Up) {
                if (!ws_.CloseTop()) {
                    WindowIngameMenu.Visible = true;
                }
            }

            if (ks_[Keys.F11] == KeyState.Down && lks_[Keys.F11] == KeyState.Up) {
                if (ks_.IsKeyDown(Keys.LeftShift) && ks_.IsKeyDown(Keys.LeftAlt)) {
                    SchemesEditorWindow.Visible = true;
                }
            }

            if (ks_[Keys.OemTilde] == KeyState.Down && lks_[Keys.OemTilde] == KeyState.Up) {
                ConsoleWindow.Visible = !ConsoleWindow.Visible;
                if (ConsoleWindow.Visible) {
                    ConsoleWindow.OnTop();
                }
            }

            if (ks_[Keys.F1] == KeyState.Down && lks_[Keys.F1] == KeyState.Up) {
                Settings.DebugInfo = !Settings.DebugInfo;
            }

            if (ks_[Keys.F5] == KeyState.Down && lks_[Keys.F5] == KeyState.Up) {
                int x = (int) player_.GetPositionInBlocks().X/16;
                int y = (int) player_.GetPositionInBlocks().Y/16;
                for (int i = -12; i < 12; i++) {
                    for (int j = -12; j < 12; j++) {
                        currentFloor_.GetSector(i + x, j + y);
                    }
                }
                currentFloor_.GenerateMap(GraphicsDevice, spriteBatch_, player_, mousemapoffset);
                ImageGlobal.image = currentFloor_.GetMap();
                currentFloor_.KillFarSectors(player_, gameTime, true);
            }

            if (ks_[Keys.F2] == KeyState.Down && lks_[Keys.F2] == KeyState.Up) {
                Settings.DebugWire = !Settings.DebugWire;
            }

            if (ks_[Keys.F3] == KeyState.Down && lks_[Keys.F3] == KeyState.Up) {
                if (drawAction_ == GameDraw) {
                    drawAction_ = DrawDebugRenderTargets;
                }
                else if (drawAction_ == DrawDebugRenderTargets) {
                    drawAction_ = GameDraw;
                }
            }

            if (ks_[Keys.F7] == KeyState.Down && lks_[Keys.F7] == KeyState.Up) {
                veh = currentFloor_.CreateTestCar();
            }

            if (ks_[Keys.F8] == KeyState.Down) {
                car = veh;
            }
        }
    }
}