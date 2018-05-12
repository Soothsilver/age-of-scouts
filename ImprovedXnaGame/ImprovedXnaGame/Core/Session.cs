﻿using Age.Animation;
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
            get
            {
                return Troops[0];
            }
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
            var target = (Vector2)Isomath.TileToStandard(tileX, tileY);
            MoveViewport.SmartCenterTo(this, target);
        }
        public void SmartCenterTo(Vector2 standardCoordinates)
        {
            MoveViewport.SmartCenterTo(this, standardCoordinates);
        }

        public Map Map;
        internal bool ObjectivesChanged;

        public Vector2 CenterOfScreenInStandardPixels { get; set; }
        public float ZoomLevel { get; set; } = 1;

        public Session()
        {
            // Generic death trigger
            this.ObjectivesChanged = true;
            this.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Count(unt => unt.Controller == levelPhase.Session.PlayerTroop) == 0,
                OnComplete = (session) =>
                {
                    session.AchieveEnding(Ending.Defeat);
                }
            });            
        }

        internal void SpawnProjectile(Projectile projectile)
        {
            this.Projectiles.Add(projectile);
        }

        internal void RightClickOn(Selection selection, Vector2 standardTarget)
        {
            Tile tile = Map.GetTileFromStandardCoordinates(standardTarget);
            if (tile != null && selection.SelectedUnits.Count > 0 && selection.SelectedUnits[0].Controller == PlayerTroop)
            {
                selection.SelectedUnits[0].UnitTemplate.PlayMovementSound();
                foreach (var target in AllUnits)
                {
                    if (AreEnemies(selection.SelectedUnits[0], target) && target.Hitbox.Contains((int)standardTarget.X, (int)standardTarget.Y))
                    {
                        selection.SelectedUnits.ForEach((unit) =>
                        {
                            unit.Activity.AttackTarget = target;
                            unit.Activity.MovementTarget = Vector2.Zero;
                            unit.Activity.SecondsUntilNextRecalculation = 0;
                        });
                        return;
                    }
                }
                selection.SelectedUnits.ForEach((unit) =>
                {
                    unit.Activity.AttackTarget = null;
                    unit.Activity.MovementTarget = standardTarget;
                    unit.Activity.SecondsUntilNextRecalculation = 0;
                });
            }
        }

        public bool AreEnemies(Unit unit, Unit target)
        {
            return unit.Controller != target.Controller && !unit.Controller.Convertible && !target.Controller.Convertible;
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

        internal void SpawnBuilding(BuildingTemplate kitchen, Troop controller, Tile tile)
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
            this.AllBuildings.Add(b);
        }
    }
}
