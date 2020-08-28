using System;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public class Piece : Node2D {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Battle/Piece.tscn");
        public Entity entity { get; private set; }
        public Tile on { get; private set; }

        public static Piece New(Entity entity, Tile tile = null) {
            Piece piece = (Piece) template.Instance();
            piece.entity = entity;
            piece.on = tile;
            return piece;
        }
        public Node2D display { get; private set; }

        public void Setup() {
            if (entity == null) {
                GD.PrintErr("Entity was null, cannot Setup");
                return;
            }
        }

        public void Delete() {
            Game.StartBusy();
            float duration = 1f;
            MoveOn(null);
            Tween tween = new Tween();
            AddChild(tween);
            tween.InterpolateProperty(this, "modulate:a", 1f, 0f, duration, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.Connect("tween_all_completed", this, nameof(DeleteCompleted));
            tween.Start();
        }
        private void DeleteCompleted() {
            if (Battle.current != null) {
                Battle.current.pieces.Remove(this);
                if (entity.actor) {
                    Battle.current.actors.Remove(this);
                }
                Battle.current.CheckIfFinished();
            }
            QueueFree();
            Game.EndBusy();
        }

        public void MoveOn(Tile tile, bool keepPosition = false) {
            if (on != null) {
                on.pieces.Remove(this);
            }
            on = tile;
            if (on != null) {
                on.pieces.Add(this);
                if (!keepPosition) {
                    Position = tile.Position;
                }
                ZIndex = (int) Math.Ceiling(tile.Position.y);
            }
        }

        private Tile startTile;
        private Tile endTile;

        public async Task WalkTo(Tile destination) {
            if (on == null || destination == null) {
                GD.PrintErr("Can't use WalkTo with null start/end tile");
            }
            Tween tween = new Tween();
            AddChild(tween);
            startTile = on;
            endTile = destination;
            ZIndex = (int) Math.Max(Math.Ceiling(on.Position.y), Math.Ceiling(destination.Position.y));
            tween.InterpolateMethod(this, "WalkToBetween", 0f, 1f, 0.2f, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            tween.Start();
            await ToSignal(tween, "tween_all_completed");
            return;
        }

        private void WalkToBetween(float alpha) {
            float mid = (alpha - 0.5f);
            mid = 4f * (0.25f - mid * mid);
            Position = (1f - alpha) * startTile.Position + alpha * endTile.Position + mid * Vector2.Up;
        }

        private static Color FRIENDLY_COLOR = new Color(0f, 0.1f, 0.1f);
        private static Color NEUTRAL_COLOR = new Color(0.05f, 0.05f, 0.05f);
        private static Color HOSTILE_COLOR = new Color(0.35f, 0f, 0f);

        public override void _Ready() {
            display = GetNode<Node2D>("Display");
            Battle.current.pieces.Add(this);
            if (entity.actor) {
                Battle.current.actors.Add(this);
            }
            GetNode<Visual.CharacterAppearance>("Display/Character").data = entity.appearance;
            entity.Connect(nameof(Entity.fallen), this, nameof(Delete));
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
            MoveOn(on);
        }
    }
}
