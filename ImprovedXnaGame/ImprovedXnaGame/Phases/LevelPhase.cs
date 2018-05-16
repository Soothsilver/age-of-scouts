using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;
using Age.HUD;
using Age.Music;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Phases
{
    class LevelPhase : DoorPhase
    {
        public Session Session { get; set; }
        public Selection Selection = new Selection();
        public Minimap Minimap = new Minimap();

        public LevelPhase(Session session)
        {
            Session = session;
            var unit = session.AllUnits.FirstOrDefault(unt => unt.Controller == session.PlayerTroop);
            if (unit != null)
            {
                session.SmartCenterTo(unit.Occupies.X, unit.Occupies.Y);
            }
        }

        protected override void Initialize(Game game)
        {
            BackgroundMusicPlayer.Play(BackgroundMusicPlayer.LevelMusic);
            SFX.PlaySound(SoundEffectName.QuestSound);
        }
        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            elapsedSeconds *= Settings.Instance.TimeFactor;
            Primitives.FillRectangle(Root.Screen, ColorScheme.LevelBackground);
          //  Session.Map.Draw(Session, elapsedSeconds, Selection);
            Selection.Draw(this, elapsedSeconds);
            DrawHUD.Draw(this, Session, topmost, elapsedSeconds);
            Cheats.Draw(this);
            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            elapsedSeconds *= Settings.Instance.TimeFactor;
            base.Update(game, elapsedSeconds);
            MoveViewport.UpdateMove(Session, elapsedSeconds);

            Waterflow.Flow(elapsedSeconds, Session.Map);
            Session.Map.ForEachTile((x, y, tile) =>
            {
                if (tile.Fog == FogOfWarStatus.Clear && tile.SecondsUntilFogStatusCanChange <= 0)
                {
                    tile.Fog = FogOfWarStatus.Grey;
                }
                tile.SecondsUntilFogStatusCanChange -= elapsedSeconds;
            });
            PerformanceCounter.AddUPSData(Session.Projectiles.Count + " projectiles; " + Session.AllUnits.Count + " units; " + (Session.Map.Width * Session.Map.Height) + " tiles");
            foreach(var unit in Session.AllUnits.Where(unt => unt.Controller == Session.PlayerTroop))
            {
                FogOfWarMechanics.RevealFogOfWar(unit.FeetStdPosition, Tile.HEIGHT * 5, Session.Map, Selection.SelectedUnits.Contains(unit));
            }
            foreach (var unit in Session.AllBuildings.Where(unt => unt.Controller == Session.PlayerTroop))
            {
                FogOfWarMechanics.RevealFogOfWar(unit.FeetStdPosition, Tile.HEIGHT * 7, Session.Map, createDebugPoints: false, fromAir: true);
            }
            Session.Revealers.ForEach(revealer => revealer.Update(Session.Map));
            foreach (var unit in Session.AllUnits)
            {
                unit.Activity.AttackingInProgress = false;
                unit.Autoaction(Session, elapsedSeconds);
                unit.Movement(Session, elapsedSeconds);
                if (unit.Controller.Convertible)
                {
                    foreach(var otherUnit in Session.AllUnits.Where(unt => unt.Controller == Session.PlayerTroop))
                    {
                        if (unit.FeetStdPosition.WithinDistance(otherUnit.FeetStdPosition, 3 * Tile.HEIGHT))
                        {
                            unit.SwitchControllerTo(otherUnit.Controller);
                            SFX.PlaySoundUnlessPlaying(SoundEffectName.Harp);
                        }
                    }
                }
            }
            foreach(var projectile in Session.Projectiles)
            {
                projectile.Update(this.Session, elapsedSeconds);
            }
            Session.AllBuildings.ForEach(build => build.Update(elapsedSeconds));
            Session.AllUnits.RemoveAll(unt => unt.Broken);
            Session.Projectiles.RemoveAll(pr => pr.Lost);
            foreach (var objective in Session.Objectives)
            {
                if (objective.Visible && !objective.Complete)
                {
                    if (objective.StateTrigger?.Invoke(this) ?? false)
                    {
                        objective.Complete = true;
                        SFX.PlaySound(SoundEffectName.QuestSound);
                        Session.ObjectivesChanged = true;
                        objective.OnComplete?.Invoke(this.Session);
                    }
                }
            }
            for(int ti = Session.OtherTriggers.Count - 1; ti >= 0; ti--)
            {
                Trigger trigger = Session.OtherTriggers[ti];

                if (trigger.StateTrigger?.Invoke(this) ?? false)
                {
                    trigger.OnComplete(this.Session);
                    Session.OtherTriggers.RemoveAt(ti);
                }
                
            }

            // Order matters, unfortunately:
            Selection.Update(this, elapsedSeconds);
            Minimap.Update(Selection, Session);
            if (Root.WasMouseRightClick)
            {
                Vector2 standardTarget = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, Session);
                Session.RightClickOn(Selection, standardTarget);
            }
            Cheats.Update(this);
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PushPhase(new IngameMenuPhase(this));
            }
        }

        public string WarningMessage;
        public float WarningMessageDisappearsInSeconds = 0;
        internal void EmitWarningMessage(string message)
        {
            WarningMessage = message;
            WarningMessageDisappearsInSeconds = 7;
        }
    }
}
