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
        public Building SelectedBuilding;
        public NaturalObject SelectedNaturalObject;

        public bool IsSomethingSelected => SelectedNaturalObject != null || SelectedBuilding != null || SelectedUnits.Count > 0;

        internal void Update(LevelPhase levelPhase, float elapsedSeconds)
        {
            Tile mouseOverTile = levelPhase.Session.Map.GetTileFromStandardCoordinates(
                    Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, levelPhase.Session)
                );
            if (UI.MouseOverOnClickAction != null)
            {
                return;
            }
            if (SelectedBuildingToPlace != null)
            {
                if (Root.WasMouseLeftClick)
                {
                    if (SelectedBuildingToPlace.PlaceableOn(levelPhase.Session, mouseOverTile))
                    {
                        if (SelectedBuildingToPlace.ApplyCost(levelPhase.Session.PlayerTroop))
                        {
                            levelPhase.Session.SpawnBuildingAsConstruction(SelectedBuildingToPlace, levelPhase.Session.PlayerTroop, mouseOverTile);
                            if (!Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            {
                                SelectedBuildingToPlace = null;
                            }
                        }
                        else
                        {
                            levelPhase.EmitWarningMessage("Na tuto budovu nemáš dost surovin.");
                        }
                    }
                    else
                    {
                        levelPhase.EmitWarningMessage("Sem budovu nemůžeš postavit.");
                    }
                    Root.WasMouseLeftClick = false;
                }
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

        internal Entity GetPrimaryEntity()
        {
            if (SelectedUnits.Count > 0)
            {
                return SelectedUnits[0];
            }
            else if (SelectedBuilding != null)
            {
                return SelectedBuilding;
            }
            else if (SelectedNaturalObject != null)
            {
                return SelectedNaturalObject;
            }
            throw new Exception("Nothing is selected.");
        }

        private void SelectThingsInRectangle(LevelPhase levelPhase, Rectangle rectSelectionBox)
        {
            List<Unit> inBox = new List<Unit>();

            foreach (var unit in levelPhase.Session.AllUnits)
            {
                if (unit.Hitbox.Intersects(rectSelectionBox))
                {
                    inBox.Add(unit);
                }
            }
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
                Building toBeSelectedBuilding = null;
                NaturalObject toBeSelectedObject = null;
                DeselectAll();
                List<Tile> selectedTiles = DetermineTilesInRectangle(rectSelectionBox, levelPhase.Session.Map);
                foreach (var tile in selectedTiles)
                {
                    if (tile.BuildingOccupant != null)
                    {
                        toBeSelectedBuilding = tile.BuildingOccupant;
                    }
                    if (tile.NaturalObjectOccupant != null)
                    {
                        toBeSelectedObject = tile.NaturalObjectOccupant;
                    }
                }
                if (toBeSelectedBuilding != null)
                {
                    SelectedBuilding = toBeSelectedBuilding;
                }
                else if (toBeSelectedObject != null)
                {
                    SelectedNaturalObject = toBeSelectedObject;
                }
            }

        }

        public static List<Tile> DetermineTilesInRectangle(Rectangle rectStandard, Map map)
        {
            IEnumerable<IntVector> tileCoordinates = MiaAlgorithm.GetTilesInsideRectangle(rectStandard, map);
            List<Tile> values = new List<Tile>();
            foreach(IntVector coordinates in tileCoordinates) {
                int x = coordinates.X;
                int y = coordinates.Y;
                if (x >= 0 && y >= 0 && x < map.Width && y < map.Height)
                {
                    values.Add(map.Tiles[x, y]);
                }     
            }
            return values;

        }

        public void SelectUnits(IEnumerable<Unit> enumerable)
        {
            DeselectAll();
            SelectedUnits.AddRange(enumerable);
        }

        private void DeselectAll()
        {
            SelectedUnits.Clear();
            SelectedNaturalObject = null;
            SelectedGodPower = null;
            SelectedBuilding = null;
        }

        public void Draw(LevelPhase levelPhase, float elapsedSeconds)
        {
            Session session = levelPhase.Session;
           // Debug.DebugPoints.Tiles = null;
            if (SelectionInProgress)
            {
                Vector2 currentStandard = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, session);
               // Debug.DebugPoints.Tiles = DetermineTilesInRectangle(GetSelectionRectangle(StandardCoordinatesSelectionStart, CurrentStandard, levelPhase.Session), levelPhase.Session.Map);
                Rectangle rectSelectionBox = Isomath.StandardToScreen(GetSelectionRectangle(StandardCoordinatesSelectionStart, currentStandard, session), session);
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