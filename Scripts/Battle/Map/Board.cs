using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
    public class Board : Node2D {

        public static int TILE_WIDTH = 32;
        public static int TILE_HEIGHT = 24;

        private Tile[] tiles = null;
        public IEnumerable<Tile> Tiles => tiles.Where(t => t != null);

        [Export] public int width;

        [Export] public int height;

        public Vector2 GetCenter() {
            return new Vector2(width * TILE_WIDTH / 2, height * TILE_HEIGHT / 2);
        }

        public void Clear() {
            if (tiles == null) {
                return;
            }
            foreach (Tile tile in tiles) {
                if (tile == null) {
                    continue;
                }
                foreach (Piece piece in tile.pieces) {
                    piece.Delete();
                }
                tile.QueueFree();
            }
            tiles = null;
        }

        public void CreateTerrain(Func<int, int, Tile.GroundType> tileType) {
            Clear();
            tiles = new Tile[width * height];
            for (int x = 0 ; x < width ; x++) {
                for (int y = 0 ; y < height ; y++) {
                    Tile.GroundType groundType = tileType(x, y);
                    if (groundType == Tile.GroundType.NONE) {
                        continue;
                    }
                    Tile tile = Tile.Create(this, x, y, groundType);
                    tile.GetNode("Control").Connect("mouse_entered", this, nameof(on_TileHovered), Global.ArrayFrom(tile));
                    tile.GetNode("Control").Connect("mouse_exited", this, nameof(on_TileExited), Global.ArrayFrom(tile));
                    tiles[y * width + x] = tile;
                }
            }
        }

        public Tile GetTile(int x, int y) {
            if (tiles == null || x < 0 || x >= width || y < 0 || y >= height) {
                return null;
            }
            return tiles[y * width + x];
        }

        [Signal] public delegate void tile_hovered(Tile tile);

        private Tile lastHovered = null;
        public bool hovered { get { return lastHovered != null; } }

        public void on_TileHovered(Tile tile) {
            lastHovered = tile;
            EmitSignal(nameof(tile_hovered), tile);
        }

        public void on_TileExited(Tile tile) {
            if (lastHovered == tile) {
                lastHovered = null;
            }
            EmitSignal(nameof(tile_hovered), tile);
        }

        public static Vector2 TileRootPosition(int x, int y) {
            return new Vector2(x * Board.TILE_WIDTH, y * Board.TILE_HEIGHT);
        }

        public static Vector2 TileRootPosition(float x, float y) {
            return new Vector2(x * Board.TILE_WIDTH, y * Board.TILE_HEIGHT);
        }
    }
}
