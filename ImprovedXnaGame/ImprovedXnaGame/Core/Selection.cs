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
        public BuildingTemplate SelectedBuildingToPlace;

        internal void Update(LevelPhase levelPhase, float elapsedSeconds)
        {
            if (UI.MouseOverOnClickAction != null)
            {
                return;
            }
            if (SelectedBuildingToPlace != null)
            {
                if (Root.WasMouseRightClick)
                {
                    Root.WasMouseRightClick = false;
                    SelectedBuildingToPlace = null;
                }
            }
            else if (SelectedGodPower == null)
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
                            SelectThingsInRectangle(levelPhase, rectSelectionBox);
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

        private void SelectThingsInRectangle(LevelPhase levelPhase, Rectangle rectSelectionBox)
        {
            List<Unit> inBox = new List<Unit>();
            List<NaturalObject> natObjects = new List<NaturalObject>();
            List<Building> buildings = new List<Building>();

            List<Tile> selectedTiles = DetermineTilesInRectangle(rectSelectionBox, levelPhase.Session.Map);

            foreach (var unit in levelPhase.Session.AllUnits)
            {
                if (unit.Hitbox.Intersects(rectSelectionBox))
                {
                    inBox.Add(unit);
                }
            }
            /*
            var topleftSelectionBox = Isomath.StandardToTile(new Vector2(rectSelectionBox.X, rectSelectionBox.Y));
            var bottomRightSelectionBox = Isomath.StandardToTile(new Vector2(rectSelectionBox.Right, rectSelectionBox.Bottom));

            levelPhase.Session.Map.ForEachTile((x, y, tile) =>
            {
                if (x >= topleftSelectionBox.X && y >= topleftSelectionBox.Y )
            });



            foreach (var bld in levelPhase.Session.AllBuildings)
            {
                if (bld.Hitbox.Intersects(rectSelectionBox))
                {
                    inBox.Add(unit);
                }
            }*/
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
            else
            {
                SelectedUnits.Clear();
            }

        }

        private List<Tile> DetermineTilesInRectangle(Rectangle rectSelectionBox, Map map)
        {
            IntVector topLeft = Isomath.StandardToTile(rectSelectionBox.X, rectSelectionBox.Y);
            IntVector topRight = Isomath.StandardToTile(rectSelectionBox.Right, rectSelectionBox.Y);
            IntVector bottomLeft = Isomath.StandardToTile(rectSelectionBox.X, rectSelectionBox.Bottom);
            IntVector bottomRight = Isomath.StandardToTile(rectSelectionBox.Right, rectSelectionBox.Bottom);
            List<Tile> values = new List<Tile>();
            int startingY = topLeft.Y - 1;
            int endingY = topLeft.Y + 1;
            bool doNotChangeStartAndEndNow = true;
            bool beforeTopRightCorner = true;
            bool noYIncreaseOnce = false;
            for (int x = topLeft.X; x <= bottomRight.X; x++)
            {
                if (beforeTopRightCorner)
                {
                    startingY -= 1;
                    if (x == topRight.X)
                    {
                        startingY++;
                        beforeTopRightCorner = false;
                        noYIncreaseOnce = true;
                    }
                }
                else
                {
                    if (noYIncreaseOnce)
                    {
                        noYIncreaseOnce = false;
                    }
                    else
                    {
                        startingY++;   
                    }
                }
                
                for (int y = startingY; y <= endingY; y++)
                {
                    if (x >= 0 && y >= 0 && x < map.Width && y < map.Height)
                    {
                        values.Add(map.Tiles[x, y]);
                    }
                }

            }
            return values;

        }

        public void SelectUnits(IEnumerable<Unit> enumerable)
        {
            SelectedGodPower = null;
            SelectedUnits.Clear();
            SelectedUnits.AddRange(enumerable);
        }

        public void Draw(LevelPhase levelPhase, float elapsedSeconds)
        {
            Debug.DebugPoints.Tiles = null;
            if (SelectionInProgress)
            {
                Vector2 CurrentStandard = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, levelPhase.Session);
                Debug.DebugPoints.Tiles = DetermineTilesInRectangle(GetSelectionRectangle(StandardCoordinatesSelectionStart, CurrentStandard, levelPhase.Session), levelPhase.Session.Map);
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