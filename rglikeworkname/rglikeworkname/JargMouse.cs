﻿using System;
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

            if (ms_.ScrollWheelValue != lms_.ScrollWheelValue) {
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

            if (!ws_.Mopusehook) {
                int nx = (ms_.X + (int) camera_.X)/32;
                int ny = (ms_.Y + (int) camera_.Y)/32;

                if (ms_.X + camera_.X < 0) {
                    nx--;
                }
                if (ms_.Y + camera_.Y < 0) {
                    ny--;
                }

                WindowIngameHint.Visible = false;

                if (player_ != null && currentFloor_ != null &&
                    !currentFloor_.IsCreatureMeele((int) ContainerOn.X, (int) ContainerOn.Y, player_)) {
                    WindowContainer.Visible = false;
                }

                if (currentFloor_ != null) {
                    Block nxny = currentFloor_.GetBlock(nx, ny);
                    bool nothingUndermouse = true;
                    if (nxny != null && nxny.Lightness == Color.White && !rememberShoot_)
                        // currentFloor_.IsExplored(aa))
                    {
                        Block a = currentFloor_.GetBlock(nx, ny);
                        if (a != null) {
                            BlockData b = BlockDataBase.Data[a.Id];
                            string s = Block.GetSmartActionName(b.SmartAction) + " " + b.Name;
                            if (Settings.DebugInfo) {
                                s += " id" + a.Id + " tex" + b.MTex;
                            }

                            if (currentFloor_.IsCreatureMeele(nx, ny, player_)) {
                                if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released) {
                                    BlockData undermouseblock = BlockDataBase.Data[a.Id];
                                    if(a.StoredItems != null && a.StoredItems.Count > 0) {
                                        WindowContainer.Visible = true;
                                        WindowContainer.SetPosition(new Vector2(Settings.Resolution.X / 2, 0));
                                        UpdateContainerContainer(a.StoredItems);
                                        ContainerOn = new Vector2(nx, ny);
                                    } else
                                    switch (undermouseblock.SmartAction) {
                                        case SmartAction.ActionSee:
                                            EventLog.Add("Вы видите " + undermouseblock.Name,
                                                         GlobalWorldLogic.CurrentTime,
                                                         Color.Gray, LogEntityType.SeeSomething);
                                            break;
                                        case SmartAction.ActionOpenContainer:
                                            WindowContainer.Visible = true;
                                            WindowContainer.SetPosition(new Vector2(Settings.Resolution.X/2, 0));
                                            UpdateContainerContainer(a.StoredItems);
                                            ContainerOn = new Vector2(nx, ny);
                                            break;
                                        case SmartAction.ActionOpenClose:
                                            currentFloor_.OpenCloseDoor(nx, ny);
                                            break;
                                    }
                                }
                            }
                            else {
                                s += " (далеко)";
                            }

                            if (WindowIngameHint.Visible = a.Id != "0") {
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