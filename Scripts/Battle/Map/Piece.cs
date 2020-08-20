using System;
using Godot;

namespace Combat {
    public class Piece : Node2D {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Battle/Piece.tscn");
        public static Piece Instance() {
            return (Piece) template.Instance();
        }
        public Node2D display { get; private set; }
        [Export] public Entity entity;

        public Battle battle { get; private set; }
        public Board board { get { return battle.board; } }
        public Tile on { get; private set; }

        public static Piece Create(Battle battle, Entity entity, Tile tile = null) {
            Piece piece = Instance();
            piece.Setup(battle, entity, tile);
            return piece;
        }

        public void Setup(Battle battle, Entity entity, Tile tile = null) {
            if (battle == null) {
                GD.PrintErr("Battle was null, cannot Setup");
                return;
            }
            if (entity == null) {
                GD.PrintErr("Entity was null, cannot Setup");
                return;
            }
            if (this.battle != null) {
                GD.PrintErr("Piece was already Setup");
                return;
            }
            this.battle = battle;
            this.entity = entity;
            battle.AddChild(this);
            battle.pieces.Add(this);
            if (entity.actor) {
                battle.actors.Add(this);
            }
            MoveOn(tile);
        }

        public void Delete() {
            MoveOn(null);
            if (battle != null) {
                battle.pieces.Remove(this);
                if (entity.actor) {
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
                ZIndex = (int) Math.Ceiling(tile.Position.y);
            }
        }

        private static Color FRIENDLY_COLOR = new Color(0f, 0.1f, 0.1f);
        private static Color NEUTRAL_COLOR = new Color(0.05f, 0.05f, 0.05f);
        private static Color HOSTILE_COLOR = new Color(0.35f, 0f, 0f);

        public override void _Ready() {
            display = GetNode<Node2D>("Display");
            Color c = NEUTRAL_COLOR;
            switch (entity.alignment) {
                case Alignment.FRIENDLY:
                    c = FRIENDLY_COLOR;
                    break;
                case Alignment.HOSTILE:
                    c = HOSTILE_COLOR;
                    break;
            }
            (display.GetNodeOrNull<Node2D>("Character")?.Material as ShaderMaterial)?.SetShaderParam("outline", c);
        }
    }
}
