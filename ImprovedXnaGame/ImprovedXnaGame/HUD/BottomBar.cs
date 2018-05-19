using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;
using Age.Phases;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.HUD
{
    class BottomBar
    {
        private const int Width = 1440;
        private const int Height = 200;

        internal void Draw(Session session, LevelPhase levelPhase, Selection selection, bool topmost)
        {

            // Bottom bar
            Rectangle rectBottomBar = new Rectangle(Root.ScreenWidth / 2 - Width / 2, Root.ScreenHeight - Height, Width, Height);
            Primitives.DrawAndFillRectangle(rectBottomBar, ColorScheme.Background, ColorScheme.Foreground);

            // Selection   
            DrawSelection(session, levelPhase, selection, topmost, rectBottomBar);

            // Minimap
            PerformanceCounter.StartMeasurement(PerformanceGroup.Minimap);
            levelPhase.Minimap.Draw(session, new Rectangle(rectBottomBar.Right - Height * 2, rectBottomBar.Y - Height, 2 * Height, 2 * Height));
            PerformanceCounter.EndMeasurement(PerformanceGroup.Minimap);

            // Tooltip
            UI.MajorTooltip?.Draw(new Rectangle(rectBottomBar.X, rectBottomBar.Y - 155, 400, 150));
        }

        private void DrawSelection(Session session, LevelPhase levelPhase, Selection selection, bool topmost, Rectangle rectBottomBar)
        {
            Rectangle rectAllSelectedUnits = new Rectangle(rectBottomBar.X + 5 + Height + 5, rectBottomBar.Y + 5, 64 * 6 + 4, Height);


            if (selection.IsSomethingSelected)
            {
                Entity primaryEntity = selection.GetPrimaryEntity();
                Unit primaryUnit = (primaryEntity is Unit ? (Unit)primaryEntity : null);
                Building asBuilding = (primaryEntity is Building ? (Building)primaryEntity : null);
                NaturalObject asNaturalObject = (primaryEntity is NaturalObject ? (NaturalObject)primaryEntity : null);

                // Flag
                Rectangle rectFlagBar = new Rectangle(rectBottomBar.X + rectBottomBar.Width / 2 - 300, rectBottomBar.Y - 30, 600, 30);
                Primitives.DrawAndFillRectangle(rectFlagBar, ColorScheme.Background, ColorScheme.Foreground);
                var flagString = primaryEntity.Controller.Name + " - " + primaryEntity.Controller.Era.ToCzech(Gender.Boy);
                BasicStringDrawer.DrawMultiLineText(flagString, rectFlagBar, primaryEntity.Controller.StrongColor, Library.FontTinyBold, Primitives.TextAlignment.Middle, shadowed: true);

                // Primary entity
                Rectangle rectPrimaryIcon = new Rectangle(rectBottomBar.X + 5, rectBottomBar.Y + 5, Height - 10, Height - 10);
                Primitives.DrawRectangle(rectPrimaryIcon, ColorScheme.Foreground);
                Primitives.DrawSingleLineText(primaryEntity.Name, new Vector2(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 5), Color.Black, Library.FontNormalBold);
                if (primaryUnit != null)
                {
                    Primitives.DrawSingleLineText(primaryUnit.UnitTemplate.Name, new Vector2(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 30), Color.Black, Library.FontNormal);
                    Primitives.DrawSingleLineText(primaryUnit.DebugActivityDescription, new Vector2(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 108), Color.Black, Library.FontTiny);
                }
                Primitives.DrawImage(primaryEntity.BottomBarTexture, new Rectangle(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 50, 64, 64));

                // All selected units and/or action currently executed by building
                Primitives.DrawRectangle(rectAllSelectedUnits, ColorScheme.Foreground);
                if (primaryUnit != null)
                {
                    DisplayAllSelectedUnits(selection, rectAllSelectedUnits);
                }
                else if (asBuilding != null)
                {
                    asBuilding.DrawActionInProgress(rectAllSelectedUnits, topmost);
                }
                else if (asNaturalObject != null)
                {
                    asNaturalObject.DrawActionInProgress(rectAllSelectedUnits);
                }

                // Actions
                if (primaryEntity.Controller == session.PlayerTroop)
                {
                    Rectangle rectOptions = new Rectangle(rectAllSelectedUnits.Right + 2, rectAllSelectedUnits.Y, rectAllSelectedUnits.Width, rectAllSelectedUnits.Height);
                    Primitives.DrawRectangle(rectOptions, Color.Black);
                    if (primaryUnit != null)
                    {
                        // Row 1: Stances
                        int y = 0;
                        int x = 0;
                        foreach (Stance stance in StaticData.AllStances)
                        {
                            var r = new Rectangle(rectOptions.X + x, rectOptions.Y + y, 64, 64);
                            UI.DrawIconButton(r, topmost, Library.Get(stance.ToTexture()), stance.ToTooltip().Caption, stance.ToTooltip().Text, () =>
                            {
                                foreach (var unt in selection.SelectedUnits)
                                {
                                    unt.Stance = stance;
                                }
                            });
                            x += 64;
                        }
                        if (primaryUnit.CarryingHowMuchResource != 0)
                        {
                            Primitives.DrawImage(Library.Get(primaryUnit.CarryingResource.ToTextureName()), new Rectangle(rectOptions.X + 10, rectOptions.Bottom - 42, 32, 32));
                            Primitives.DrawSingleLineText(primaryUnit.CarryingHowMuchResource.ToString(), new Vector2(rectOptions.X + 50, rectOptions.Bottom - 38),
                                Color.Black, Library.FontTinyBold);
                        }
                    }
                    {

                        int x = 0;
                        int y = 64;
                        // Row 2+: Building options
                        foreach (ConstructionOption building in primaryEntity.ConstructionOptions)
                        {
                            var r = new Rectangle(rectOptions.X + x, rectOptions.Y + y, 64, 64);
                            UI.DrawIconButton(r, topmost, building.Icon.Color(primaryEntity.Controller), building.TooltipCaption, building.Description, () =>
                            {
                                if (building.AffordableBy(primaryEntity.Controller))
                                {
                                    building.OnClick(asBuilding, selection);
                                }
                                else
                                {
                                    levelPhase.EmitWarningMessage("Na tuto budovu nebo jednotku nemáš dost surovin nebo přesahuješ populační limit.");
                                }
                            });
                            x += 64;
                        }
                    }
                    if (primaryUnit != null)
                    {
                        int x = 0;
                        int y = 0;
                        Stance? commonStance = primaryUnit.Stance;
                        if (selection.SelectedUnits.Any(unt => unt.Stance != commonStance))
                        {
                            commonStance = null;
                        }
                        foreach (Stance stance in StaticData.AllStances)
                        {
                            var r = new Rectangle(rectOptions.X + x, rectOptions.Y + y, 64, 64);
                            if (commonStance == stance)
                            {
                                Primitives.DrawRectangle(r.Extend(2, 2), Color.Red, 2);
                            }
                            x += 64;
                        }
                    }
                }
            }
        }

        private static void DisplayAllSelectedUnits(Selection selection, Rectangle rectAllSelectedUnits)
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < selection.SelectedUnits.Count; i++)
            {
                Unit unit = selection.SelectedUnits[i];
                Rectangle rectOneIcon = new Rectangle(rectAllSelectedUnits.X + 2 + x, rectAllSelectedUnits.Y + 2 + y, 64, 64);
                bool mo = Root.IsMouseOver(rectOneIcon);
                Primitives.FillRectangle(rectOneIcon, unit.Controller.LightColor);
                if (mo)
                {
                    Primitives.FillRectangle(rectOneIcon, Color.White.Alpha(50));
                    UI.MajorTooltip = unit.GetTooltip();
                    UI.MouseOverOnClickAction = () =>
                    {
                        selection.SelectUnits(new Unit[] { unit });
                    };
                }
                Primitives.DrawImage(Library.Get(unit.UnitTemplate.Icon), rectOneIcon);
                Primitives.DrawRectangle(rectOneIcon, Color.Black);
                Primitives.DrawHealthBar(new Rectangle(rectOneIcon.X, rectOneIcon.Bottom - 5, rectOneIcon.Width, 5), unit.Controller.StrongColor, unit.HP, unit.MaxHP);


                if ((i + 1) % 6 == 0)
                {
                    x = 0;
                    y += 64;
                }
                else
                {
                    x += 64;
                }
            }
        }
    }
}
