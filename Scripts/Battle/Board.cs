using System;
using Godot;

public class Board : Node2D {

    public static int TILE_WIDTH = 64;
    public static int TILE_HEIGHT = 64;

    public enum Direction {
        NONE,
        RIGHT,
        UP,
        LEFT,
        DOWN
    }

    private Tile[] tiles = null;

    [Export] public int width;

    [Export] public int height;

    public override void _Ready() {
        //Temp
        CreateTerrain();
        Piece.Create(this, GetTile(2, 2));
        Piece.Create(this, null).MoveOn(GetTile(2, 4));
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
