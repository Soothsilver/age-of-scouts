using Age.Animation;
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
            }
            else
            {
                SFX.PlaySound(SoundEffectName.ItIsOver);
                SFX.PlaySound(SoundEffectName.MiseNebylaSplnena);
            }
        }

        public void SmartCenterTo(int tileX, int tileY)
        {
            var target = (Vector2) Isomath.TileToStandard(tileX, tileY);
            MoveViewport.SmartCenterTo(this, target);
        }

        public void SmartCenterTo(Vector2 standardCoordinates)
        {
            MoveViewport.SmartCenterTo(this, standardCoordinates);
        }


        public Session()
        {
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
                // Move/attack/gather/build...
                foreach (var target in AllUnits)
                {
                    if (AreEnemies(selection.SelectedUnits[0], target) &&
                        target.Hitbox.Contains((int) standardTarget.X, (int) standardTarget.Y))
                    {
                        selection.SelectedUnits[0].UnitTemplate.PlayAttackSound();
                        selection.SelectedUnits.ForEach((unit) => { unit.Strategy.ResetTo(target); });
                        return;
                    }
                }

                if (tile.BuildingOccupant != null && selection.SelectedUnits[0].CanContributeToBuilding(tile.BuildingOccupant))
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayBuildSound();
                }
                else
                {
                    selection.SelectedUnits[0].UnitTemplate.PlayMovementSound();
                }
                selection.SelectedUnits.ForEach((unit) =>
                {
                    if (tile.BuildingOccupant != null && unit.CanContributeToBuilding(tile.BuildingOccupant))
                    {
                        unit.Strategy.ResetTo(tile.BuildingOccupant);
                    }
                    else
                    {
                        unit.Strategy.ResetTo(standardTarget);
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

        public bool AreEnemies(Unit unit, Unit target)
        {
            return unit.Controller != target.Controller && !unit.Controller.Convertible &&
                   !target.Controller.Convertible;
        }

        public void EnqueueVoice(ChatLine chatLine)
        {
            this.VoiceQueue.Add(chatLine);
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
            for (int y = 0; y < kitchen.TileHeight; y++)
            {
                for (int x = 0; x < kitchen.TileWidth; x++)
                {
                    int tileX = tile.X - x;
                    int tileY = tile.Y - y;
                    Tile tl = Map.Tiles[tileX, tileY];
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
        }

        public void Update(float elapsedSeconds)
        {
            Map.Update(elapsedSeconds);

            foreach (var unit in AllUnits.Where(unt => unt.Controller == PlayerTroop || Settings.Instance.EnemyUnitsRevealFogOfWar))
            {
                FogOfWarMechanics.RevealFogOfWar(unit.FeetStdPosition, Tile.HEIGHT * 5, Map);
            }
            foreach (var unit in AllBuildings.Where(unt => unt.Controller == PlayerTroop))
            {
                FogOfWarMechanics.RevealFogOfWar(unit.FeetStdPosition, Tile.HEIGHT * 7, Map, fromAir: true);
            }

            foreach (FogRevealer revealer in Revealers)
            {
                revealer.Update(Map);
            }

            for (int ui = AllUnits.Count - 1; ui >= 0; ui--)
            {
                Unit unit = AllUnits[ui];
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