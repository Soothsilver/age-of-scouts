using Age.Core;
using Age.Phases;
using Age.Voice;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.HUD
{
    class DrawHUD
    {
        public static void Draw(LevelPhase mainPhase, Session session, bool topmost, float elapsedSeconds)
        {
            DrawTopBar(mainPhase, session, topmost);
            session.Ending?.Draw(topmost);
            DrawBottomBar(session, mainPhase.Selection);
            DrawChatLine(session, elapsedSeconds);
            DrawInstructionalLine(mainPhase, session);
        }

        private static void DrawInstructionalLine(LevelPhase levelPhase, Session session)
        {
            if (levelPhase.Selection.SelectedGodPower != null)
            {
                var bnds = Primitives.GetMultiLineTextBounds("{b}" + levelPhase.Selection.SelectedGodPower.Power.InstructionalLine + "{/b}", new Rectangle(0, 0, Root.ScreenWidth, 100), FontFamily.Mid);
                Primitives.FillRectangle(new Rectangle(Root.ScreenWidth / 2 - bnds.Width / 2 - 30, Root.ScreenHeight - 400 - 20, bnds.Width + 60, bnds.Height + 40), Color.Brown.Alpha(200));
                Primitives.DrawMultiLineText("{b}" + levelPhase.Selection.SelectedGodPower.Power.InstructionalLine + "{/b}", new Rectangle(0, Root.ScreenHeight - 400, Root.ScreenWidth, 100), Color.White, FontFamily.Mid, Primitives.TextAlignment.Top, false);
            }
        }

        private static void DrawChatLine(Session session, float elapsedSeconds)
        {
            ChatLine current = session.CurrentVoiceLine;
            if (current == null)
            {
                if (session.VoiceQueue.Count > 0)
                {
                    current = session.VoiceQueue[0];
                    session.CurrentVoiceLine = current;
                    session.VoiceQueue.RemoveAt(0);
                    SFX.PlaySound(current.Sfx);
                }
            }
            if (current != null)
            {
                Rectangle bounds = BasicStringDrawer.GetMultiLineTextBounds(current.Text, new Rectangle(10, 200, 400, 300), Library.FontTinyBold);
                Primitives.FillRectangle(new Rectangle(5, 195, 410, Math.Max(bounds.Height, 30) + 10), Color.Brown.Alpha(200));
                BasicStringDrawer.DrawMultiLineText(current.Text, new Rectangle(10, 200, 400, 300), Color.White, Library.FontTinyBold, Primitives.TextAlignment.TopLeft);
                current.SecondsRemaining -= elapsedSeconds;
                if (current.SecondsRemaining <= 0)
                {
                    session.CurrentVoiceLine = null;
                }
            }
        }

        private static void DrawBottomBar(Session session, Selection selection)
        {
            int width = 1440;
            int height = 200;

            // Bottom bar
            Rectangle rectBottomBar = new Rectangle(Root.ScreenWidth / 2 - width / 2, Root.ScreenHeight - height, width, height);
            Primitives.DrawAndFillRectangle(rectBottomBar, ColorScheme.Background, ColorScheme.Foreground);

            // Selection
            if (selection.SelectedUnits.Count > 0)
            {
                Unit primaryUnit = selection.SelectedUnits[0];
                Rectangle rectFlagBar = new Rectangle(rectBottomBar.X + rectBottomBar.Width / 2 - 300, rectBottomBar.Y - 30, 600, 30);
                Primitives.DrawAndFillRectangle(rectFlagBar, ColorScheme.Background, ColorScheme.Foreground);
                var flagString = primaryUnit.Controller.Name + " - " + primaryUnit.Controller.Era.ToCzech(Gender.Boy);
                BasicStringDrawer.DrawMultiLineText(flagString, rectFlagBar, primaryUnit.Controller.StrongColor, Library.FontTinyBold, Primitives.TextAlignment.Middle, shadowed: true);

                Rectangle rectPrimaryIcon = new Rectangle(rectBottomBar.X + 5, rectBottomBar.Y + 5, height - 10, height - 10);
                Primitives.DrawRectangle(rectPrimaryIcon, ColorScheme.Foreground);
                Primitives.DrawSingleLineText(primaryUnit.Name, new Vector2(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 5), Color.Black, Library.FontNormalBold);
                Primitives.DrawSingleLineText(primaryUnit.UnitTemplate.Name, new Vector2(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 30), Color.Black, Library.FontNormal);
                Primitives.DrawImage(Library.Get(primaryUnit.UnitTemplate.Icon), new Rectangle(rectPrimaryIcon.X + 5, rectPrimaryIcon.Y + 50, height - 60, height - 60));

                Rectangle rectAllIcons = new Rectangle(rectBottomBar.X + 5 + height + 5, rectBottomBar.Y + 5, 64*6+4, height);
                Primitives.DrawRectangle(rectAllIcons, ColorScheme.Foreground);
                int x = 0;
                int y = 0;
                for (int i = 0; i < selection.SelectedUnits.Count; i++)
                {
                    Unit unit = selection.SelectedUnits[i];
                    Rectangle rectOneIcon = new Rectangle(rectAllIcons.X + 2 + x, rectAllIcons.Y + 2 + y, 64, 64);
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
                    Primitives.DrawHealthbar(new Rectangle(rectOneIcon.X, rectOneIcon.Bottom - 5, rectOneIcon.Width, 5), unit.Controller.StrongColor, unit.HP, unit.MaxHP);


                    if ((i + 1) % 6 == 0) {
                        x = 0;
                        y += 64;
                    }
                    else
                    {
                        x += 64;
                    }
                }
            }

            // Minimap
            Minimap.Draw(session, new Rectangle(rectBottomBar.Right - height * 2, rectBottomBar.Y - height, 2 * height, 2 * height));

            // Tooltip
            UI.MajorTooltip?.Draw(new Rectangle(rectBottomBar.X, rectBottomBar.Y - 155, 400, 150));
        }

        private static void DrawTopBar(LevelPhase mainPhase, Session session, bool topmost)
        {
            Troop you = session.PlayerTroop;
            Rectangle rectTopBar = new Rectangle(0, 0, Root.ScreenWidth, 40);
            Primitives.FillRectangle(rectTopBar, ColorScheme.Background);
            Primitives.DrawLine(new Vector2(0, rectTopBar.Height), new Vector2(Root.ScreenWidth, rectTopBar.Height), ColorScheme.Foreground, 1);

            // Resources
            DrawResourceBox("Jídlo", you.Food.ToString(), TextureName.MeatIcon, new Rectangle(20, 2, 120, 36), "Jídlo sbíráš z {b}kukuřičných polí{/b} a {b}lesních plodů{/b}. Jídlo potřebuješ k nabírání nových skautů, ke cvičení skautů, k některým vylepšením a pro postup do dalšího věku. Skauti nepotřebují jídlo, aby přežili: jakmile jednou skauta nabereš, už pro něj další jídlo není třeba.");
            DrawResourceBox("Dřevo", you.Wood.ToString(), TextureName.WoodIcon, new Rectangle(150, 2, 120, 36), "Dřevo sbíráš kácením {b}stromů{/b}. Dřevo potřebuješ ke stavění táborových staveb, k cvičení některých skautů, k některým vylepšením a pro postup do dalšího věku.");
            DrawResourceBox("Turbojíl", you.Clay.ToString(), TextureName.MudIcon, new Rectangle(280, 2, 120, 36), "Turbojíl sbíráš z {b}bahenních polí{/b}. Turbojíl potřebuješ ke stavění pokročilých staveb a k cvičení některých skautů.");
            DrawResourceBox("Skauti", you.PopulationUsed + " / " + you.PopulationLimit, TextureName.PopulationIcon, new Rectangle(410, 2, 120, 36), "Číslo vlevo je počet populačních jednotek, které zabírají tvoji skauti. Číslo vpravo je kapacita tvého tábora v populačních jednotkách. Nemůžeš nabírat další skauty, pokud už nemáš volnou kapacitu. Kapacitu můžeš zvýšit stavěním stanů.");

            // Age and god powers
            int agesWidth = 320;
            Rectangle rectAges = new Rectangle(Root.ScreenWidth / 2 - agesWidth / 2, 0, agesWidth, 110);
            Primitives.DrawAndFillRectangle(rectAges, ColorScheme.Background, ColorScheme.Foreground);
            BasicStringDrawer.DrawMultiLineText(you.Era.ToCzech(you.Gender), new Rectangle(rectAges.X, rectAges.Y, rectAges.Width, 30), Color.Black, Library.FontTinyBold, Primitives.TextAlignment.Middle);
            for (int i = 0; i < 4; i++)
            {
                Rectangle rectGodPower = new Rectangle(rectAges.X + 80 * i, rectAges.Y + 30, 80, 80);
                Primitives.DrawImage(Library.Get(TextureName.GodPowerBackground), rectGodPower, Color.White.Alpha(150));
                if (you.LeaderPowers.Count > i)
                {
                    bool mopower = Root.IsMouseOver(rectGodPower);
                    Primitives.DrawImage(Library.Get(TextureName.GodPowerBackground), rectGodPower, Color.White);
                    LeaderPowerInstance leaderPowerInstance = you.LeaderPowers[i];
                    Primitives.DrawImage(Library.Get(leaderPowerInstance.Power.Icon), rectGodPower);
                    if (leaderPowerInstance.Used)
                    {
                        Primitives.DrawImage(Library.Get(TextureName.GodPowerUsed), rectGodPower);
                    }
                    if (!mopower)
                    {
                        Primitives.FillRectangle(rectGodPower, Color.Black.Alpha(15));
                    }
                    
                    Primitives.DrawImage(Library.Get(TextureName.GodPowerBorder), rectGodPower);     
                    if (mopower && !leaderPowerInstance.Used)
                    {
                        UI.MajorTooltip = new Tooltip(leaderPowerInstance.Power.Name + " (vůdcovská schopnost)", leaderPowerInstance.Power.Description);
                        UI.MouseOverOnClickAction = () =>
                        {
                            mainPhase.Selection.SelectUnits(new Unit[] { });
                            mainPhase.Selection.SelectedGodPower = leaderPowerInstance;
                        };
                    }
                }
                else
                {
                    Primitives.FillRectangle(rectGodPower, Color.Black.Alpha(180));
                }
            }

            // Buttons
            UI.DrawButton(new Rectangle(rectTopBar.Right - 410, 0, 300, 40), topmost, "Úkoly", () => Root.PushPhase(new ViewObjectivesPhase(mainPhase.Session)), "Zobrazí úkoly. Splněním úkolů vyhrajete úroveň.");
            UI.DrawButton(new Rectangle(rectTopBar.Right - 110, 0, 100, 40), topmost, "Menu", () => Root.PushPhase(new IngameMenuPhase(mainPhase)), "Zobrazí menu, ze kterého můžete hru ukončit.");
        }
        

        private static void DrawResourceBox(string caption, string contents, TextureName icon, Rectangle rectangle, string description)
        {
            Primitives.DrawAndFillRectangle(rectangle, ColorScheme.DarkBackground, Color.White);
            Primitives.DrawRectangle(rectangle.Extend(1, 1), Color.Black);
            Rectangle rectIcon = new Rectangle(rectangle.X + 3, rectangle.Y + rectangle.Height / 2 - 16, 32, 32);
            Primitives.DrawImage(Library.Get(icon), rectIcon);
            Primitives.DrawSingleLineText(caption, new Vector2(rectangle.X + 40, rectangle.Y + 2), Color.White, Library.FontTiny);
            Primitives.DrawSingleLineText(contents, new Vector2(rectangle.X + 45, rectangle.Y + 15), Color.White, Library.FontTinyBold);
            if (Root.IsMouseOver(rectangle))
            {
                UI.MajorTooltip = new Tooltip(caption + " (" + contents + ")", description);
            }
        }
    }
}
