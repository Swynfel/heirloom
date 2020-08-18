using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Battle : Node2D {
        public static Battle instance = null;

        public Board board { get; private set; }

        public List<Piece> pieces { get; } = new List<Piece>();

        public List<Piece> actors { get; } = new List<Piece>();

        public Piece currentActor { get; private set; } = null;


        public override void _Ready() {
            board = new Board();
            AddChild(board);
            board.width = 5;
            board.height = 10;
            board.CreateTerrain();
            Piece.Create(this, board.GetTile(2, 2));
            Piece.Create(this, null).MoveOn(board.GetTile(2, 4));
        }

    }
}
