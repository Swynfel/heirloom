using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Battle : Node2D {
        public static Battle current = null;

        public Board board { get; private set; }
        public Camera2D camera { get; private set; }

        public List<Piece> pieces { get; } = new List<Piece>();

        public List<Piece> actors { get; } = new List<Piece>();

        public Piece currentActor { get; private set; } = null;


        public override void _Ready() {
            current = this;
            board = new Board();
            AddChild(board);
            camera = GetNode<Camera2D>("Camera");
            LoadBoard();
        }

        public void LoadBoard() {
            board.width = 8;
            board.height = 5;
            board.CreateTerrain();
            Piece.Create(this, board.GetTile(2, 2));
            Piece.Create(this, null).MoveOn(board.GetTile(2, 4));
            Piece.Create(this, board.GetTile(3, 2));
            camera.Position = board.GetCenter();
        }

    }
}
