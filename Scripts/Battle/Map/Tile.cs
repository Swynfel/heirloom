using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Tile : Node2D {

        public enum GroundType {
            ERROR,
            NONE,
            GRASS,
            DIRT,
            DRY,
            SAND,
            STONE,
            WOOD,
            TEMPLE_PEBBLES,
            TEMPLE_FULL,
            TEMPLE_BORDER,
            TEMPLE_WATER,
        }
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Battle/Tile.tscn");
        private static Tile Instance() {
            return (Tile) template.Instance();
        }
        public int x { get; private set; }
        public int y { get; private set; }

        public Board board { get; private set; }

        public List<Piece> pieces { get; } = new List<Piece>();

        public static Tile Create(Board board, int x, int y, GroundType ground) {
            Tile tile = Instance();
            board.AddChild(tile);
            tile.board = board;
            tile.Name = string.Format("Tile_{0}_{1}", x, y);
            tile.x = x;
            tile.y = y;
            tile.Position = tile.RootPosition();
            tile.GetNode<Sprite>("Sprite").Frame = (int) ground;
            return tile;
        }

        public Vector2 RootPosition() {
            return Board.TileRootPosition(x, y);
        }

        public Tile GetNeighbor(Direction direction) {
            (int x, int y) = GetNeighborCoords(direction);
            return board.GetTile(x, y);
        }

        public (int, int) GetNeighborCoords(Direction direction) {
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
            return (_x, _y);
        }

        public List<Tile> GetNeighbors() {
            List<Tile> neighbors = new List<Tile>();
            foreach (Direction dir in DirectionUtils.DIRECTIONS) {
                Tile neighbor = GetNeighbor(dir);
                if (neighbor != null) {
                    neighbors.Add(neighbor);
                }
            }
            return neighbors;
        }

        public List<TileFlow> GetNeighborOutFlows() {
            List<TileFlow> neighbors = new List<TileFlow>();
            foreach (Direction dir in DirectionUtils.DIRECTIONS) {
                Tile neighbor = GetNeighbor(dir);
                if (neighbor != null) {
                    neighbors.Add(new TileFlow(neighbor, dir));
                }
            }
            return neighbors;
        }

        public List<TileFlow> GetNeighborInFlows() {
            List<TileFlow> neighbors = new List<TileFlow>();
            foreach (Direction dir in DirectionUtils.DIRECTIONS) {
                Tile neighbor = GetNeighbor(dir);
                if (neighbor != null) {
                    neighbors.Add(new TileFlow(neighbor, dir.Opposite()));
                }
            }
            return neighbors;
        }

        // Display

        private const int STRENGTH_OFFSET = 2;

        public void ResetDisplay() {
            SelectDisplay(0, TileColor.NONE);
        }

        private readonly Color NONE_COLOR = Colors.Transparent;
        private readonly Color VALID_COLOR = Color.Color8(0, 132, 16, 162);
        private readonly Color ERROR_COLOR = Color.Color8(140, 10, 0, 170);
        private readonly Color SECONDARY_COLOR = Color.Color8(0, 132, 16, 162);

        public enum TileColor {
            NONE,
            VALID,
            ERROR,
            SECONDARY,
        }

        public void SelectDisplay(int strength, TileColor c) {
            Color color = c == TileColor.NONE ? NONE_COLOR :
            c == TileColor.VALID ? VALID_COLOR :
            c == TileColor.ERROR ? ERROR_COLOR : SECONDARY_COLOR;
            SelectDisplay(strength, color);
        }

        public void SelectDisplay(int strength, Color color) {
            Position = RootPosition() + STRENGTH_OFFSET * strength * Vector2.Up;
            GetNode<Control>("Control").Modulate = color;
            foreach (Piece piece in pieces) {
                // TODO: let piece handle its position
                piece.Position = Position;
            }
        }

        public override string ToString() {
            return $"<{x},{y}>";
        }
    }
}
