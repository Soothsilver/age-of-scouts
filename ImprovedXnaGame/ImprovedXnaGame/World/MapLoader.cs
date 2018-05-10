﻿using Age.Core;
using Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledSharp;

namespace Age.World
{
    class MapLoader
    {
        private string filename;

        public MapLoader(string filename)
        {
            this.filename = filename;
        }
        internal void LoadInto(Session session, string pathFile)
        {
            TmxMap tmxMap = new TmxMap(filename);
            Dictionary<int, string> gidToTiletype = new Dictionary<int, string>();
            foreach (TmxTileset tileset in tmxMap.Tilesets)
            {
                foreach (TmxTilesetTile tile in tileset.Tiles)
                {
                    int gid = tileset.FirstGid + tile.Id;
                    if (tile.Properties.ContainsKey("A"))
                    {
                        string tiletype = tile.Properties["A"];
                        gidToTiletype.Add(gid, tiletype);
                    } else
                    {
                        gidToTiletype.Add(gid, null);
                    }
                }
            }
            Map map = new Map
            {
                Width = tmxMap.Width,
                Height = tmxMap.Height
            };
            map.Tiles = new Tile[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y< map.Height; y++)
                {
                    map.Tiles[x, y] = new Tile(x, y, TextureName.MudIcon);
                }
            }
            map.ForEachTile((x, y, tile) =>
            {
                if (x != 0 && y != 0) tile.Neighbours.Top = map.Tiles[x - 1, y - 1];
                if (y != 0) tile.Neighbours.TopRight = map.Tiles[x, y - 1];
                if (x != map.Width - 1 && y != 0) tile.Neighbours.Right = map.Tiles[x +1, y- 1];

                if (x != 0) tile.Neighbours.TopLeft = map.Tiles[x - 1, y];
                if (x != map.Width - 1) tile.Neighbours.BottomRight = map.Tiles[x + 1, y];

                if (x != 0 && y != map.Height - 1) tile.Neighbours.Left = map.Tiles[x - 1, y + 1];
                if (y != map.Height - 1) tile.Neighbours.BottomLeft = map.Tiles[x, y + 1];
                if (x != map.Width - 1 && y != map.Height - 1) tile.Neighbours.Bottom = map.Tiles[x + 1, y + 1];

                tile.Neighbours.FillAll();
            });
            foreach(TmxLayer layer in tmxMap.Layers)
            {
                foreach(TmxLayerTile layerTile in layer.Tiles)
                {
                    Tile tile = map.Tiles[layerTile.X, layerTile.Y];
                    if (layerTile.Gid == 0)
                    {
                        continue;
                    }
                    string tiletype = gidToTiletype[layerTile.Gid];
                    if (tiletype == null)
                    {
                        throw new Exception("The tile " + layerTile.X + ":" + layerTile.Y + " refers to a tile without an A tiletype.");
                    }
                    AssignDataFromTileType(session, tile, tiletype, layer.Name);
                }
            }
            map.ForEachTile((x, y, tile) =>
            {
                tile.Neighbours.FillAll();
            });
            session.Map = map;
        }

        private void AssignDataFromTileType(Session session, Tile tile, string tiletype, string layerName)
        {
            Troop controller = layerName == "Blue" ? session.PlayerTroop : session.Troops[1];
            switch (tiletype)
            {
                case "Grass":
                    tile.Icon = TextureName.IsoGrass2;
                    tile.Type = TileType.Grass;
                    break;
                case "Water":
                    tile.Icon = TextureName.IsoWater;
                    tile.Type = TileType.Water;
                    break;
                case "Flag":
                    tile.NaturalObjectOccupant = SpawnNaturalObject(TextureName.BasicFlag, EntityKind.TutorialFlag, tile);
                    break;
                case "Tree":
                    tile.NaturalObjectOccupant = SpawnNaturalObject(TextureName.BasicTree, EntityKind.UntraversableTree, tile);
                    break;
                case "TallGrass":
                    tile.NaturalObjectOccupant = SpawnNaturalObject(TextureName.TallGrass, EntityKind.TallGrass, tile);
                    break;
                case "BerryBush":
                    tile.NaturalObjectOccupant = SpawnNaturalObject(TextureName.BerryBush, EntityKind.BerryBush, tile);
                    break;
                case "Corn":
                    tile.NaturalObjectOccupant = SpawnNaturalObject(TextureName.Corn, EntityKind.Corn, tile);
                    break;
                case "BigSimpleTent":
                    tile.NaturalObjectOccupant = SpawnNaturalObject(TextureName.BigSimpleTent, EntityKind.UnalignedTent, tile);
                    tile.Neighbours.TopLeft.NaturalObjectOccupant = SpawnNaturalObject(TextureName.None, EntityKind.UnalignedTent, tile.Neighbours.TopLeft);
                    tile.Neighbours.Top.NaturalObjectOccupant = SpawnNaturalObject(TextureName.None, EntityKind.UnalignedTent, tile.Neighbours.Top);
                    tile.Neighbours.TopRight.NaturalObjectOccupant = SpawnNaturalObject(TextureName.None, EntityKind.UnalignedTent, tile.Neighbours.TopRight);
                    break;
                case "Kitchen":
                case "TentStandard":
                    // TODO to be implemented
                    break;
                case "Pracant":
                    session.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), controller, UnitTemplate.Pracant, Isomath.TileToStandard(tile.X + 0.5f, tile.Y + 0.5f)));
                    break;
                default:
                    throw new Exception("The tiletype '" + tiletype + "' does not have anything assigned in the project.");
            }
        }

        private NaturalObject SpawnNaturalObject(TextureName icon, EntityKind entityKind, Tile tile)
        {
            return NaturalObject.Create(icon, entityKind, tile.X, tile.Y);
        }

      
    }
}
