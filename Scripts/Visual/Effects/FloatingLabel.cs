using System;
using Godot;

namespace Visual.Effects {
    public class FloatingLabel : Node2D {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Effects/FloatingLabel.tscn");

        public static void Create(Node node, string text) {
            Create(node, Vector2.Zero, text, Colors.White);
        }
        public static void Create(Node node, Vector2 position, string text, Color color) {
            FloatingLabel instance = (FloatingLabel) template.Instance();
            instance.Load(node, position, text, color);
        }

        public static void CreateDamage(Combat.Piece piece, int damage) {
            Create(piece, new Vector2(0, -6), damage.ToString(), Colors.Red);
        }

        private Label label;
        private Vector2 startingPosition;

        private void Load(Node node, Vector2 position, string text, Color color) {
            float duration = 0.4f;
            label = GetNode<Label>("Label");
            label.Text = text;
            node.AddChild(this);
            Position = position;
            Tween tween = new Tween();
            AddChild(tween);
            tween.InterpolateProperty(this, "modulate:a", 1f, 0f, duration, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.InterpolateProperty(this, "position:y", startingPosition.y, startingPosition.y - 16, duration, Tween.TransitionType.Sine, Tween.EaseType.Out);
            tween.Connect("tween_all_completed", this, nameof(on_TweenAllCompleted));
            tween.Start();
        }

        private void on_TweenAllCompleted() {
            QueueFree();
        }
    }
}