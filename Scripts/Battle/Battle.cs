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
        }

        private bool boardLoaded = false;

        public override void _Process(float delta) {
            if (!boardLoaded) {
                LoadBoard();
            }
            if (pendingNextTurn && !Game.busy) {
                pendingNextTurn = false;
                int i = actors.IndexOf(currentActor);
                TurnOf(actors[(i + 1) % actors.Count]);
            }
        }

        [Signal] public delegate void next_turn(Piece actor);

        private void TurnOf(Piece actor) {
            EmitSignal(nameof(next_turn), actor);
            currentActor = actor;
        }

        private bool pendingNextTurn = false;

        public void NextTurn() {
            pendingNextTurn = true;
        }

        public void LoadBoard() {
            boardLoaded = true;
            board.width = 8;
            board.height = 5;
            board.CreateTerrain();
            Piece.Create(this,
                new Entity(Alignment.FRIENDLY, true, 10),
                board.GetTile(2, 2)
            );
            Piece.Create(this,
                new Entity(Alignment.FRIENDLY, true, 11),
                board.GetTile(2, 4)
            );
            Piece.Create(this,
                new Entity(Alignment.HOSTILE, true, 12),
                board.GetTile(3, 2)
            );
            TurnOf(actors[0]);
            camera.Position = board.GetCenter();
        }
    }
}
