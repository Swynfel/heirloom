using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Tile : Node2D {

        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Battle/Tile.tscn");
        private static Tile Instance() {
            return (Tile) template.Instance();
        }
        public int x { get; private set; }
        public int y { get; private set; }

        public Board board { get; private set; }

        public List<Piece> pieces { get; } = new List<Piece>();

        [Export] public bool sprite;

        public static Tile Create(Board board, int x, int y) {
            Tile tile = Instance();
            board.AddChild(tile);
            tile.board = board;
            tile.Name = string.Format("Tile_{0}_{1}", x, y);
            tile.x = x;
            tile.y = y;
            tile.Position = new Vector2(x * Board.TILE_WIDTH, y * Board.TILE_HEIGHT);
            return tile;
        }

        public Tile GetNeighbor(Direction direction) {
            int _x = x;
            int _y = y;
            switch (direction) {
                case Direction.RIGHT:
                    _x++;
                    break;
                case Direction.UP:
                    _y--;
                    break;
                case Direction.LEFT:
                    _x--;
                    break;
                case Direction.DOWN:
                    _y++;
                    break;
            }
            return board.GetTile(_x, _y);
        }
    }
}
