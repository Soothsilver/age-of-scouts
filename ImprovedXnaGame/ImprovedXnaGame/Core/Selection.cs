using Age.Core;
using Age.HUD;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Age.Phases
{
    class Selection
    {
        public List<Unit> SelectedUnits = new List<Unit>();

        public bool SelectionInProgress = false;
        public Vector2 StandardCoordinatesSelectionStart = Vector2.Zero;
        public LeaderPowerInstance SelectedGodPower;

        internal void Update(LevelPhase levelPhase, float elapsedSeconds)
        {
            if (UI.MouseOverOnClickAction != null)
            {
                return;
            }
            if (SelectedGodPower == null)
            {
                if (Root.Mouse_NewState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    if (Root.Mouse_OldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        SelectionInProgress = true;
                        StandardCoordinatesSelectionStart = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, levelPhase.Session);
                    }
                }
                else if (Root.Mouse_NewState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    if (Root.Mouse_OldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && Root.WasMouseLeftClick)
                    {
                        SelectionInProgress = false;
                        if (UI.MouseOverOnClickAction == null)
                        {
                            Vector2 CurrentStandard = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, levelPhase.Session);

                            Rectangle rectSelectionBox = GetSelectionRectangle(StandardCoordinatesSelectionStart, CurrentStandard, levelPhase.Session);
                            List<Unit> inBox = new List<Unit>();
                            foreach (var unit in levelPhase.Session.AllUnits)
                            {
                                if (unit.Hitbox.Intersects(rectSelectionBox))
                                {
                                    inBox.Add(unit);
                                }
                            }
                            SelectedUnits.Clear();
                            if (inBox.Count > 0)
                            {
                                if (inBox.Any(unt => unt.Controller == levelPhase.Session.PlayerTroop))
                                {
                                    inBox[0].UnitTemplate.PlaySelectionSound(inBox[0]);
                                    SelectUnits(inBox.Where(unt => unt.Controller == levelPhase.Session.PlayerTroop));
                                }
                                else
                                {
                                    SelectUnits(inBox.Where(unt => unt.Controller == inBox[0].Controller));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Root.WasMouseRightClick)
                {
                    Root.WasMouseRightClick = false;
                    SelectedGodPower = null;
                }
                if (Root.WasMouseLeftClick)
                {
                    Vector2 where = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, levelPhase.Session);
                    Tile tile = levelPhase.Session.Map.GetTileFromStandardCoordinates(where);
                    if (tile != null)
                    {
                        SelectedGodPower.Use(where, levelPhase.Session);
                    }
                    SelectedGodPower = null;
                    Root.WasMouseLeftClick = false;
                }
            }
        }

        public void SelectUnits(IEnumerable<Unit> enumerable)
        {
            SelectedGodPower = null;
            SelectedUnits.Clear();
            SelectedUnits.AddRange(enumerable);
        }

        public void Draw(LevelPhase levelPhase, float elapsedSeconds)
        {
            if (SelectionInProgress)
            {
                Vector2 CurrentStandard = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, levelPhase.Session);
                Rectangle rectSelectionBox = Isomath.StandardToScreen(GetSelectionRectangle(StandardCoordinatesSelectionStart, CurrentStandard, levelPhase.Session), levelPhase.Session);
                Primitives.DrawRectangle(rectSelectionBox, Color.White);
                Primitives.DrawRectangle(rectSelectionBox.Extend(1,1), Color.Black);
            }
        }

        private Rectangle GetSelectionRectangle(Vector2 standardStart, Vector2 standardEnd, Session session)
        {
            Vector2 topLeft = new Vector2(Math.Min(standardStart.X, standardEnd.X), Math.Min(standardStart.Y, standardEnd.Y));
            Vector2 bottomRight = new Vector2(Math.Max(standardStart.X, standardEnd.X), Math.Max(standardStart.Y, standardEnd.Y));
            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y));
        }
    }
}