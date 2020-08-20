using System;
using Godot;

namespace Combat {
    public class Piece : Node2D {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Battle/Piece.tscn");
        public static Piece Instance() {
            return (Piece) template.Instance();
        }
        public Node2D display { get; private set; }
        [Export] public PieceStats stats;

        public Battle battle { get; private set; }
        public Board board { get { return battle.board; } }
        public Tile on { get; private set; }

        public static Piece Create(Battle battle, Tile tile = null) {
            Piece piece = Instance();
            piece.Setup(battle, tile);
            return piece;
        }

        public void Setup(Battle battle, Tile tile = null) {
            if (battle == null) {
                GD.PrintErr("Battle was null, cannot Setup");
                return;
            }
            if (this.battle != null) {
                GD.PrintErr("Piece was already Setup");
                return;
            }
            this.battle = battle;
            battle.AddChild(this);
            battle.pieces.Add(this);
            if (stats.actor) {
                battle.actors.Add(this);
            }
            MoveOn(tile);
        }

        public void Delete() {
            MoveOn(null);
            if (battle != null) {
                battle.pieces.Remove(this);
                if (stats.actor) {
                    battle.actors.Remove(this);
                }
                battle = null;
            }
            QueueFree();
        }

        public void MoveOn(Tile tile) {
            if (on != null) {
                on.pieces.Remove(this);
            }
            on = tile;
            if (on != null) {
                on.pieces.Add(this);
                Position = tile.Position;
            }
        }

        public override void _Ready() {
            display = GetNode<Node2D>("Display");
            Color c = Colors.Gray;
            switch (stats.alignment) {
                case Alignment.FRIENDLY:
                    c = Colors.Green;
                    break;
                case Alignment.HOSTILE:
                    c = Colors.Red;
                    break;
            }
            (display.GetNodeOrNull<Node2D>("Character")?.Material as ShaderMaterial)?.SetShaderParam("outline", c);
        }
    }
}
