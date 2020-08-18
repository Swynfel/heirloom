using System;
using Godot;

namespace Combat {
    public class Board : Node2D {

        public static int TILE_WIDTH = 32;
        public static int TILE_HEIGHT = 24;

        private Tile[] tiles = null;

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
                tile.QueueFree();
            }
            tiles = null;
        }

        public void CreateTerrain() {
            Clear();
            tiles = new Tile[width * height];
            for (int x = 0 ; x < width ; x++) {
                for (int y = 0 ; y < height ; y++) {
                    tiles[y * width + x] = Tile.Create(this, x, y);
                }
            }
        }

        public Tile GetTile(int x, int y) {
            if (tiles == null || x < 0 || x >= width || y < 0 || y >= height) {
                return null;
            }
            return tiles[y * width + x];
        }
    }
}
