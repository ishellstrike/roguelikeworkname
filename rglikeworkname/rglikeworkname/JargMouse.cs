using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace jarg {
    public partial class JargMain {
        private bool doubleclick_;
        private TimeSpan doubleclicktimer_ = TimeSpan.Zero;
        private bool firstclick_;
        private bool rememberShoot_;
        //private TimeSpan sec20glitch;
        private int rotater;

        private void UpdateMouse(GameTime gameTime) {
            lms_ = ms_;
            ms_ = Mouse.GetState();

            doubleclicktimer_ += gameTime.ElapsedGameTime;

            doubleclick_ = false;
            if (firstclick_ && ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released &&
                doubleclicktimer_.TotalMilliseconds < 300) {
                doubleclick_ = true;
                firstclick_ = false;
            }
            if (!firstclick_ && ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                firstclick_ = true;
                doubleclicktimer_ = TimeSpan.Zero;
            }
            if (doubleclicktimer_.TotalMilliseconds > 300) {
                firstclick_ = false;
                doubleclick_ = false;
            }

            //sec20glitch += gameTime.ElapsedGameTime;
            //if(sec20glitch.TotalSeconds > 1 && (ms_.X != lms_.X || ms_.Y != lms_.Y)) {
            //    //ws_.DoGlitch();
            //    sec20glitch = TimeSpan.Zero;
            //}

            if (ms_.LeftButton == ButtonState.Released) {
                rememberShoot_ = false;
            }

            if (ms_.ScrollWheelValue != lms_.ScrollWheelValue && !ws_.Mopusehook) {
                cam.Zoom += (ms_.ScrollWheelValue - lms_.ScrollWheelValue)/(float)gameTime.ElapsedGameTime.TotalMilliseconds/10;
            }

            if (ms_.MiddleButton == ButtonState.Pressed) {
                cam.Yaw = 0;
                cam.Pitch = 0;
                cam.Zoom = 30;
            }

            if (ks_.IsKeyDown(Keys.LeftAlt))
            {
                Mouse.SetPosition((int)(Settings.Resolution.X / 2), (int)(Settings.Resolution.Y / 2));
                lms_ = Mouse.GetState();
                var b = lms_.Y - ms_.Y;

                cam.Pitch += b / 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (ms_.RightButton == ButtonState.Pressed || ks_.IsKeyDown(Keys.LeftAlt))
            {
                var a = lms_.X - ms_.X;

                cam.Yaw += a / 10f * (float)gameTime.ElapsedGameTime.TotalSeconds ;
            }

            if (player_ != null && currentFloor_ != null && !currentFloor_.IsCreatureMeele((int)ContainerOn.X, (int)ContainerOn.Y, player_))
            {
                WindowContainer.Visible = false;
            }

            if (!ws_.Mopusehook) {
                Vector3 nP = new Vector3(ms_.X, ms_.Y, 0);
                Vector3 fP = new Vector3(ms_.X, ms_.Y, 0.1f);
                Vector3 n3dP = GraphicsDevice.Viewport.Unproject(nP, cam.ProjectionMatrix, cam.ViewMatrix, Matrix.Identity);
                Vector3 f3dP = GraphicsDevice.Viewport.Unproject(fP, cam.ProjectionMatrix, cam.ViewMatrix, Matrix.Identity);
                Vector3 dir = f3dP - n3dP; dir.Normalize();
                Ray carRay = new Ray(n3dP, dir); 
                float? f = carRay.Intersects(new Plane(Vector3.Forward, 0));

                if (f.HasValue) {
                    Vector3 p = carRay.Position + carRay.Direction*f.Value;
                    UndermouseX = (int) (p.X);
                    UndermouseY = (int) (p.Y);

                    UndermouseX = p.X < 0 ? UndermouseX - 1 : UndermouseX;
                    UndermouseY = p.Y < 0 ? UndermouseY - 1 : UndermouseY;
                }

                WindowIngameHint.Visible = false;

                if (currentFloor_ != null) {
                    Block nxny = currentFloor_.GetBlock(UndermouseX, UndermouseY);
                    bool nothingUndermouse = true;
                    if (nxny != null && !rememberShoot_)
                        // currentFloor_.IsExplored(aa))
                    {
                        Block a = currentFloor_.GetBlock(UndermouseX, UndermouseY);
                        if (a != null) {
                            BlockData b = Registry.Instance.Blocks[a.Id];
                            string s = Block.GetSmartActionName(b.SmartAction) + " " + b.Name;
                            if (Settings.DebugInfo) {
                                s += " id:" + a.Id + " tex:" + b.MTex + " t:" +b.Type ?? string.Empty;
                            }

                            if (currentFloor_.IsCreatureMeele(UndermouseX, UndermouseY, player_)) {
                                if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                                    BlockData undermouseblock = Registry.Instance.Blocks[a.Id];
                                    var storage = a as IItemStorage;
                                    if(storage != null) {
                                        WindowContainer.Visible = true;
                                        WindowContainer.SetPosition(new Vector2(Settings.Resolution.X / 2, 0));
                                        UpdateContainerContainer(storage.ItemList);
                                        ContainerOn = new Vector2(UndermouseX, UndermouseY);
                                    } else
                                    switch (undermouseblock.SmartAction) {
                                        case SmartAction.ActionSee:
                                            EventLog.Add("Вы видите " + undermouseblock.Name,
                                                         GlobalWorldLogic.CurrentTime,
                                                         Color.Gray, LogEntityType.SeeSomething);
                                            break;
                                        case SmartAction.ActionOpenClose:
                                            currentFloor_.OpenCloseDoor(UndermouseX, UndermouseY);
                                            break;
                                    }
                                }
                            }
                            else {
                                s += " (далеко)";
                            }

                            WindowIngameHint.Visible = a.Id != "0";
                            if (WindowIngameHint.Visible) {
                                LabelIngameHint.Text = s;
                                WindowIngameHint.Locate.Width = (int) LabelIngameHint.Width + 20;
                                WindowIngameHint.SetPosition(new Vector2(ms_.X + 10, ms_.Y + 10));
                                nothingUndermouse = false;
                            }
                        }
                    }

                    if (!currentFloor_.IsCreatureMeele((int) ContainerOn.X, (int) ContainerOn.Y, player_)) {
                        WindowContainer.Visible = false;
                    }

                    if ((nothingUndermouse && ms_.LeftButton == ButtonState.Pressed) || rememberShoot_) {
                        player_.TryShoot(PlayerSeeAngle);
                        rememberShoot_ = true;
                    }
                }
            }
        }
    }
}