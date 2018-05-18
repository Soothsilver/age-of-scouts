﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Animation;
using Age.HUD;
using Age.Phases;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    class Building : Entity
    {
        public Tile PrimaryTile;
        public BuildingTemplate Template;
        public Construction ConstructionInProgress;
        public LinkedList<Construction> ConstructionQueue = new LinkedList<Construction>();
        internal bool SelfConstructionInProgress;
        internal float SelfConstructionProgress;
        internal Vector2 RallyPointInStandardCoordinates;

        public Building(BuildingTemplate buildingTemplate, Troop controller, Vector2 feetPosition, Tile primaryTile) : base(buildingTemplate.Icon, feetPosition)
        {
            this.Template = buildingTemplate;
            Texture2D texture = Library.Get(buildingTemplate.Icon);
            this.PixelHeight = texture.Height;
            this.PixelWidth = texture.Width;
            this.Controller = controller;
            this.Session = Controller.Session;
            this.PrimaryTile = primaryTile;
        }

        internal Vector2 FindExitPoint()
        {
            return Isomath.TileToStandard(this.PrimaryTile.Neighbours.BottomRight.X + 0.5f, this.PrimaryTile.Neighbours.BottomRight.Y + 0.5f);
        }

        public override string Name => this.Template.Name;

        public override Texture2D BottomBarTexture => SpriteCache.GetColoredTexture(Template.Icon, Controller.LightColor);

        public override List<ConstructionOption> ConstructionOptions
        {
            get
            {
                if (this.SelfConstructionInProgress)
                {
                    return ConstructionOption.None;
                }
                else if (this.Template.Id == BuildingId.Kitchen)
                {
                    return ConstructionOption.KitchenOptions;
                }
                else
                {
                    return ConstructionOption.None;
                }
            }
        }

        public bool NoUnitsOnThisBuilding
        {
            get
            {
                for (int x = 0; x < Template.TileWidth; x++)
                {
                    for (int y = 0; y < Template.TileHeight; y++)
                    {
                        Tile tile = Session.Map.Tiles[x + this.PrimaryTile.X, y + this.PrimaryTile.Y];
                        if (tile.Occupants.Count > 0) return false;
                    }
                }
                return true;
            }
        }

        internal void EnqueueConstruction(UnitTemplate unitTemplate)
        {
            this.Controller.Food -= unitTemplate.FoodCost;
            this.Controller.Wood -= unitTemplate.WoodCost;
            ConstructionQueue.AddLast(new Construction(unitTemplate));
        }

        public void Draw(IScreenInformation screen, Selection selection)
        {
            Rectangle rectWhere = Isomath.StandardPersonToScreen(this.FeetStdPosition, this.PixelWidth, this.PixelHeight, screen);
            if (SelfConstructionInProgress)
            {
                Texture2D what = SpriteCache.GetColoredTexture(this.Template.Icon, this.Controller.LightColor);
                int screenHeight = (int)(rectWhere.Height * SelfConstructionProgress);
                Rectangle destinationRectangle = new Rectangle(rectWhere.X, rectWhere.Bottom - screenHeight, rectWhere.Width, screenHeight);
                int textureHeight = (int)(what.Height * SelfConstructionProgress);
                Rectangle srcRect = new Rectangle(0, what.Height - textureHeight, what.Width, textureHeight);
                Primitives.SpriteBatch.Draw(what, destinationRectangle, srcRect, Color.White.Alpha(200));
                Primitives.DrawRectangle(rectWhere.Extend(1, 1), this.Controller.StrongColor);
            }
            else
            {
                Primitives.DrawImage(SpriteCache.GetColoredTexture(this.Template.Icon, this.Controller.LightColor), rectWhere);
                if (selection.SelectedBuilding == this)
                {
                    Primitives.DrawRectangle(rectWhere.Extend(1, 1), this.Controller.StrongColor);
                }
            }
            
        }

        internal HashSet<Vector2> GetFreeSpotsForBuilders()
        {
            HashSet<Vector2> allocation = new HashSet<Vector2>();
            for (int x = 0; x < Template.TileWidth; x++)
            {
                for (int y = 0; y< Template.TileHeight; y++)
                {
                    Tile tile = Session.Map.Tiles[x + this.PrimaryTile.X, y + this.PrimaryTile.Y];
                    foreach(var neighbour in tile.Neighbours.All)
                    {
                        if (neighbour.Occupants.Count == 0)
                        {
                            allocation.Add(Isomath.TileToStandard(neighbour.X + 0.5f, neighbour.Y + 0.5f));
                        }
                    }
                }
            }
            return allocation;
        }

        internal void DrawActionInProgress(Rectangle rectAction, bool topmost)
        {
            // Queue
            int x = rectAction.X + 5;
            foreach(Construction n in ConstructionQueue)
            {
                Rectangle rectQueued = new Rectangle(x, rectAction.Bottom - 69, 64, 64);
                UI.DrawIconButton(rectQueued, topmost, n.ConstructingWhat.Icon.Color(this.Controller),
                    "Odstranit z fronty tuto položku", n.ConstructingWhat.Description,
                    () =>
                    {
                        ConstructionQueue.Remove(n);
                    });
                x += 64;
                if (x + 64 >= rectAction.Right)
                {
                    break;
                }
            }
            // Main construction
            if (ConstructionInProgress != null)
            {
                Rectangle rectProgressBar = new Rectangle(rectAction.X + 20, rectAction.Y + 70, rectAction.Width - 40, 40);
                Primitives.DrawHealthbar(rectProgressBar, Color.Gold, (int)(1000 * ConstructionInProgress.WorkDoneInSeconds), (1000 * (int)ConstructionInProgress.TotalWorkNeeded));
                Primitives.DrawSingleLineText(ConstructionInProgress.Subcaption, new Vector2(rectAction.X + 20, rectAction.Y + 30), Color.Black, Library.FontTinyBold);
                Primitives.DrawSingleLineText(ConstructionInProgress.Caption + " (" + ((int)(100 * ConstructionInProgress.WorkDoneInSeconds) / (int)ConstructionInProgress.TotalWorkNeeded) + "%)", new Vector2(rectAction.X + 20, rectAction.Y + 45), Color.Black, Library.FontMid);
                Primitives.DrawImage(ConstructionInProgress.ConstructingWhat.Icon.Color(this.Controller), new Rectangle(rectAction.Right - 68, rectAction.Y + 4, 64, 64));
            }
            // Self construction
            if (SelfConstructionInProgress)
            {
                Rectangle rectProgressBar = new Rectangle(rectAction.X + 20, rectAction.Y + 70, rectAction.Width - 40, 40);
                Primitives.DrawHealthbar(rectProgressBar, Color.Lime, (int)(SelfConstructionProgress * 10000), 10000);
                Primitives.DrawSingleLineText("Tato stavba se staví...", new Vector2(rectAction.X + 20, rectAction.Y + 30), Color.Black, Library.FontTinyBold);
                Primitives.DrawSingleLineText(this.Name + " (" + (int)(100 * SelfConstructionProgress) + "%)", new Vector2(rectAction.X + 20, rectAction.Y + 45), Color.Black, Library.FontMid);
                Primitives.DrawImage(this.Icon.Color(this.Controller), new Rectangle(rectAction.Right - 68, rectAction.Y + 4, 64, 64));
            }
        }

        public void Update(float elapsedSeconds)
        {
            if (SelfConstructionInProgress)
            {
                if (!NoUnitsOnThisBuilding)
                {
                    for (int x = 0; x < Template.TileWidth; x++)
                    {
                        for (int y = 0; y < Template.TileHeight; y++)
                        {
                            Tile tile = Session.Map.Tiles[x + this.PrimaryTile.X, y + this.PrimaryTile.Y];
                            foreach(Unit unit in tile.Occupants)
                            {
                                if (unit.Controller == this.Controller)
                                {
                                    unit.AttemptToExitConstructionSite(this);
                                }
                            }
                        }
                    }
                }
                if (SelfConstructionProgress >= 1)
                {
                    SelfConstructionProgress = 1;
                    SelfConstructionInProgress = false;
                    SFX.PlaySoundUnlessPlaying(SoundEffectName.Harp);
                }
            }
            else
            {
                if (ConstructionInProgress == null && ConstructionQueue.Count > 0)
                {
                    Construction first = ConstructionQueue.First.Value;
                    ConstructionQueue.RemoveFirst();
                    ConstructionInProgress = first;
                }
                if (ConstructionInProgress != null)
                {
                    ConstructionInProgress.WorkDoneInSeconds += elapsedSeconds;
                    if (ConstructionInProgress.WorkDoneInSeconds >= ConstructionInProgress.TotalWorkNeeded)
                    {
                        ConstructionInProgress.WorkDoneInSeconds = ConstructionInProgress.TotalWorkNeeded;
                        if (ConstructionInProgress.Completed(this))
                        {
                            ConstructionInProgress = null;
                        }
                    }
                }
            }
        }
    }
}
