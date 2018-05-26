using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;
using Age.HUD;
using Age.Internet;
using Age.Music;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Phases
{
    class LevelPhase : DoorPhase
    {
        public Session Session;
        public OtherThreads OtherThreads;
        public Selection Selection = new Selection();
        public Minimap Minimap = new Minimap();

        public LevelPhase(Session session)
        {
            OtherThreads = new OtherThreads(this);
            Session = session;
            var unit = session.AllUnits.FirstOrDefault(unt => unt.Controller == session.PlayerTroop);
            if (unit != null)
            {
                session.SmartCenterTo(unit.Occupies.X, unit.Occupies.Y);
            }
        }

        public override void Destruct(Game game)
        {
            base.Destruct(game);
            OtherThreads.EndWorking();
        }

        protected override void Initialize(Game game)
        {
            BackgroundMusicPlayer.Play(BackgroundMusicPlayer.LevelMusic);
            Eqatec.ScheduleSendMessage("LEVEL START", Session.Map.Width + "x" + Session.Map.Height);
            SFX.PlaySound(SoundEffectName.QuestSound);
            OtherThreads.StartWorking(this.Session);
        }
        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            elapsedSeconds *= Settings.Instance.TimeFactor;
            Primitives.FillRectangle(Root.Screen, ColorScheme.LevelBackground);
            Session.Map.Draw(Session, elapsedSeconds, Selection);
            Selection.Draw(this, elapsedSeconds);
            DrawHUD.Draw(this, Session, topmost, elapsedSeconds);
            Cheats.Draw(this);
            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            OtherThreads.UpdateCycleBegins(this.Session);
            elapsedSeconds *= Settings.Instance.TimeFactor;
            base.Update(game, elapsedSeconds);

            MoveViewport.UpdateMove(Session, elapsedSeconds);
            Waterflow.Flow(elapsedSeconds, Session.Map);
            Session.Update(elapsedSeconds);
            PerformanceCounter.AddUPSData(Session.Projectiles.Count + " projectiles; " + Session.AllUnits.Count + " units; " + (Session.Map.Width * Session.Map.Height) + " tiles");
          
            // Some tutorial-level objectives require knowing about selection, so this is here for now, rather than in Session:
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

            // First, see if you clicked on map.
            Minimap.Update(Selection, Session);
            // Next, update selection but note that minimap may have overridden it.
            Selection.Update(this, elapsedSeconds);
            // Next, if right-click wasn't already used, perform it to change the strategy of selected units or do something else with selection:
            if (Root.WasMouseRightClick)
            {
                Vector2 standardTarget = Isomath.ScreenToStandard(Root.Mouse_NewState.X, Root.Mouse_NewState.Y, Session);
                Session.RightClickOn(Selection, standardTarget);
            }
            PerformanceCounter.StartMeasurement(PerformanceGroup.AI);
            for(int pl = 0; pl < Session.Troops.Count; pl++)
            {
                Troop troop = Session.Troops[pl];
                troop.AI.Update(Session);
            }
            PerformanceCounter.EndMeasurement(PerformanceGroup.AI);
            Cheats.Update(this);
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PushPhase(new IngameMenuPhase(this));
            }

            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F12))
            {
                Root.PushPhase(new SettingsPhase());
            }
        }

        public string WarningMessage;
        public float WarningMessageDisappearsInSeconds = 0;
        internal void EmitWarningMessage(string message)
        {
            WarningMessage = message;
            WarningMessageDisappearsInSeconds = 7;
        }

        internal void EmitInsufficientResourcesFor(IHasCosts building, Troop playerTroop)
        {
            if (building.FoodCost > playerTroop.Food)
            {
                EmitWarningMessage("Nemáš dost jídla. Pošli pracanty sbírat bobule nebo sklízet kukuřici.");
                SFX.PlaySoundUnlessPlaying(SoundEffectName.NotEnoughFood);
            }
            else if (building.WoodCost > playerTroop.Wood)
            {
                EmitWarningMessage("Nemáš dost dřeva. Pošli pracanty kácet stromy.");
                SFX.PlaySoundUnlessPlaying(SoundEffectName.NotEnoughWood);
            }
            else if (building.ClayCost > playerTroop.Clay)
            {
                EmitWarningMessage("Nemáš dost turbojílu. Pošli pracanty do bahenních polí.");
                SFX.PlaySoundUnlessPlaying(SoundEffectName.NotEnoughClay);
            }
            else if (building.PopulationCost > playerTroop.PopulationLimit- playerTroop.PopulationUsed && building.PopulationCost > 0)
            {
                EmitWarningMessage("Postav více stanů, abys mohl nabrat další skauty.");
                SFX.PlaySoundUnlessPlaying(SoundEffectName.NotEnoughPopulationLimit);
            }
        }
    }
}
