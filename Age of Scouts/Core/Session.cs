using Age.Animation;
using Age.Internet;
using Age.Phases;
using Age.Voice;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Core
{
    class Session : IScreenInformation
    {
        public Troop PlayerTroop
        {
            get { return Troops[0]; }
        }

        public string LevelName;
        public List<Troop> Troops = new List<Troop>();
        public List<Unit> AllUnits = new List<Unit>();
        public List<Building> AllBuildings = new List<Building>();
        public List<Objective> Objectives = new List<Objective>();
        public List<Trigger> OtherTriggers = new List<Trigger>();
        public Ending Ending = null;
        public List<FogRevealer> Revealers = new List<FogRevealer>();
        public List<ChatLine> VoiceQueue = new List<ChatLine>();
        public List<Projectile> Projectiles = new List<Projectile>();
        public ChatLine CurrentVoiceLine;
        public Map Map;
        internal bool ObjectivesChanged;
        internal SessionFlags Flags = new SessionFlags();
        internal MinimapWarnings MinimapWarnings;

        /// <summary>
        /// Greater zoom level means more close-up. Lesser zoom level means looking from a distance. A zoom-level of 1 is basic 1:1 display. A zoom-level of 2 is
        /// where each tile is twice as big as normal.
        /// </summary>
        public float ZoomLevel { get; set; } = 1;
        public Vector2 CenterOfScreenInStandardPixels { get; set; }
        public Troop GaiaTroop { get; }


        internal void AchieveEnding(Ending ending)
        {
            if (Ending != null) return;
            this.Ending = ending;
            if (this.Ending.IsVictory)
            {
                SFX.PlaySound(SoundEffectName.Fanfare);
                SFX.PlaySound(SoundEffectName.MiseBylaUspesneSplnena);
                Eqatec.ScheduleSendMessage("END:VICTORY", Eqatec.Identify(this));
            }
            else
            {
                SFX.PlaySound(SoundEffectName.ItIsOver);
                SFX.PlaySound(SoundEffectName.MiseNebylaSplnena);
                Eqatec.ScheduleSendMessage("END:DEFEAT", Eqatec.Identify(this));
            }
        }

        public void SmartCenterTo(int tileX, int tileY)
        {
            var target = (Vector2) Isomath.TileToStandard(tileX, tileY);
            MoveViewport.SmartCenterTo(this, target);
        }

        internal void DestroyBuilding(Building bld)
        {
            // TODO optimization
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y< Map.Height; y++)
                {
                    if (Map.Tiles[x,y].BuildingOccupant == bld)
                    {
                        Map.Tiles[x, y].BuildingOccupant = null;
                    }
                }
            }
            SFX.PlaySound(SoundEffectName.BLAST10);
            AllBuildings.Remove(bld);   
        }

        public void SmartCenterTo(Vector2 standardCoordinates)
        {
            MoveViewport.SmartCenterTo(this, standardCoordinates);
        }


        public Session()
        {
            this.MinimapWarnings = new MinimapWarnings(this);
            GaiaTroop = new Troop("Příroda", this, Era.EraNacelniku, Color.DarkGreen, Color.LightGreen);
            // Generic death trigger
            this.ObjectivesChanged = true;
            this.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) =>
                    levelPhase.Session.AllUnits.Count(unt => unt.Controller == levelPhase.Session.PlayerTroop) == 0,
                OnComplete = (session) => { session.AchieveEnding(Ending.Defeat); }
            });
        }

        internal void SpawnProjectile(Projectile projectile)
        {
            this.Projectiles.Add(projectile);
        }

        internal void RightClickOn(Selection selection, Vector2 standardTarget)
        {
            Tile tile = Map.GetTileFromStandardCoordinates(standardTarget);
            if (tile != null && selection.SelectedUnits.Count > 0 &&
                selection.SelectedUnits[0].Controller == PlayerTroop)
            {
                if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) ||
                    Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl))
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayAttackSound();
                    selection.SelectedUnits.ForEach((unit) => { unit.Strategy.ResetToAttackMove((IntVector)standardTarget); });
                    return;
                }
                // Move/attack/gather/build...
                foreach (var target in AllUnits)
                {
                    if (AreEnemies(selection.SelectedUnits[0], target) &&
                        target.Hitbox.Contains((int) standardTarget.X, (int) standardTarget.Y))
                    {
                        selection.SelectedUnits[0].UnitTemplate.PlayAttackSound();
                        selection.SelectedUnits.ForEach((unit) => { unit.Strategy.ResetToAttack(target); });
                        return;
                    }
                }
                if (tile.BuildingOccupant != null && selection.SelectedUnits[0].CanContributeToBuilding(tile.BuildingOccupant))
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayBuildSound();
                }
                else if (tile.BuildingOccupant != null && selection.SelectedUnits.Any(unt => unt.CanAttack) &&
                         AreEnemies(selection.SelectedUnits[0], tile.BuildingOccupant))
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayAttackSound();
                    selection.SelectedUnits.ForEach((unit) => { unit.Strategy.ResetToAttack(tile.BuildingOccupant); });
                    return;
                }
                else if (tile.NaturalObjectOccupant != null && selection.SelectedUnits[0].CanBeOrderedToGatherFrom(tile.NaturalObjectOccupant))
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayGatherSound(tile.NaturalObjectOccupant.EntityKind);
                }
                else
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayMovementSound();
                }
                int maxRangeExpected = ((selection.SelectedUnits.Count / 6) + 1) * Tile.HALF_WIDTH;
                bool scatter = false;
                selection.SelectedUnits.ForEach((unit) =>
                {
                    if (tile.BuildingOccupant != null && unit.CanContributeToBuilding(tile.BuildingOccupant))
                    {
                        unit.Strategy.ResetTo(tile.BuildingOccupant);
                    }
                    else if (tile.NaturalObjectOccupant != null && unit.CanBeOrderedToGatherFrom(tile.NaturalObjectOccupant))
                    {
                        unit.Strategy.ResetTo(tile.NaturalObjectOccupant);
                    }
                    else if (tile.BuildingOccupant != null && tile.BuildingOccupant.CanAcceptResourcesFrom(unit))
                    {
                        unit.Strategy.ResetTo(tile.BuildingOccupant);
                    }
                    else
                    {
                        if (scatter)
                        {
                            Vector2 actualTarget = standardTarget + R.RandomUnitVector() * maxRangeExpected * R.Float();
                            unit.Strategy.ResetTo(actualTarget);
                        }
                        else
                        {
                            scatter = true;
                            unit.Strategy.ResetTo(standardTarget);
                        }
                    }
                });
            }
            else if (tile != null && selection.SelectedBuilding != null &&
                     selection.SelectedBuilding.Controller == PlayerTroop)
            {
                // Set rally point
                selection.SelectedBuilding.RallyPointInStandardCoordinates = standardTarget;
            }
        }

        public bool AreEnemies(Entity unit, Entity target)
        {
            return unit.Controller != target.Controller &&
                   !unit.Controller.IsAlliedWith(target.Controller) &&
                   !unit.Controller.Convertible &&
                   !target.Controller.Convertible;
        }

        public void EnqueueVoice(ChatLine chatLine)
        {
            this.VoiceQueue.Add(chatLine);
        }
        public void EnqueueVoice(string line, SoundEffectName sfx)
        {
            this.VoiceQueue.Add(new ChatLine(line, sfx));
        }

        public void SpawnUnit(Unit unit)
        {
            this.AllUnits.Add(unit);
            Tile tile = Map.GetTileFromStandardCoordinates(unit.FeetStdPosition);
            tile.Occupants.Add(unit);
            unit.Occupies = tile;
        }

        internal Building SpawnBuilding(BuildingTemplate kitchen, Troop controller, Tile tile)
        {
    
            var b = new Building(kitchen, controller, Isomath.TileToStandard(tile.X + 1, tile.Y + 1), tile);
            b.HP = b.MaxHP = kitchen.MaxHP;
            for (int y = 0; y < kitchen.TileHeight; y++)
            {
                for (int x = 0; x < kitchen.TileWidth; x++)
                {
                    int tileX = tile.X - x;
                    int tileY = tile.Y - y;
                    Tile tl = Map.Tiles[tileX, tileY];
                    if (tl.BuildingOccupant != null)
                    {

                    }
                    tl.BuildingOccupant = b;
                }
            }

            if (b.Template.Id == BuildingId.Kitchen)
            {
                b.RallyPointInStandardCoordinates = b.FeetStdPosition + new Vector2(0, 10);
            }

            this.AllBuildings.Add(b);
            return b;
        }

        internal Building SpawnBuildingAsConstruction(BuildingTemplate template, Troop controller, Tile tile)
        {
            Building b = SpawnBuilding(template, controller, tile);
            b.SelfConstructionInProgress = true;
            b.SelfConstructionProgress = 0.1f;
            return b;
        }

        public void CopyValuesFrom(Session session)
        {
            if (this.Map == null)
            {
                this.Map = new Map();
            }

            this.Map.CopyValuesFrom(session.Map);
            this.CenterOfScreenInStandardPixels = session.CenterOfScreenInStandardPixels;
            this.ZoomLevel = session.ZoomLevel;
            this.AllBuildings.Clear();
            this.AllUnits.Clear();
            this.Revealers.Clear();
            this.AllUnits.AddRange(session.AllUnits);
            this.AllBuildings.AddRange(session.AllBuildings);
            this.Troops.Clear();
            this.Troops.AddRange(session.Troops);
            this.Revealers.AddRange(session.Revealers);
        }

        public void Update(float elapsedSeconds)
        {
            Map.Update(elapsedSeconds);


            for (int ui = AllUnits.Count - 1; ui >= 0; ui--)
            {
                Unit unit = AllUnits[ui];
                if (unit.Broken) continue;
                unit.Activity.AttackingInProgress = false;
                unit.Autoaction(this, elapsedSeconds);
                unit.DoAssignedActivity(this, elapsedSeconds);
                if (unit.Controller.Convertible)
                {
                    foreach (var otherUnit in AllUnits.Where(unt => unt.Controller == PlayerTroop))
                    {
                        if (unit.FeetStdPosition.WithinDistance(otherUnit.FeetStdPosition, 3 * Tile.HEIGHT))
                        {
                            unit.SwitchControllerTo(otherUnit.Controller);
                            SFX.PlaySoundUnlessPlaying(SoundEffectName.Harp);
                        }
                    }
                }
            }

            for (int pi = Projectiles.Count - 1; pi >=0;pi--)
            {
                Projectile projectile = Projectiles[pi];
                projectile.Update(this, elapsedSeconds);
                if (projectile.Lost)
                {
                    Projectiles.RemoveAt(pi);
                }
            }
            // Break from projectiles
            for (int ui = AllUnits.Count - 1; ui >= 0; ui--)
            {
                Unit unit = AllUnits[ui];
                if (unit.Broken)
                {
                    unit.Occupies.BrokenOccupants.Add(new Corpse(unit));
                    unit.Occupies.Occupants.Remove(unit);
                    unit.Occupies = null;
                    AllUnits.RemoveAt(ui);
                }
            }

            foreach (Building building in AllBuildings)
            {
                building.Update(elapsedSeconds);
            }


        }
    }
}