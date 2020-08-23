using System;
using Godot;

namespace Visual.Effects {
    public class FloatingLabel : Node2D {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Effects/FloatingLabel.tscn");

        public static void Create(Node node, string text) {
            Create(node, Vector2.Zero, text, Colors.White);
        }
        public static void Create(Node node, Vector2 position, string text, Color color, Color? outline = null) {
            FloatingLabel instance = (FloatingLabel) template.Instance();
            instance.Load(node, position, text, color, outline);
        }

        private static Color DAMAGE_COLOR = new Color(0.75f, 0, 0);
        private static Color HEALING_COLOR = new Color(0, 0, 0);
        private static Color OUTLINE_COLOR = new Color(1, 1, 1, 0.65f);
        public static void CreateDamage(Combat.Piece piece, int damage) {
            Create(piece, new Vector2(0, -12), damage.ToString(), DAMAGE_COLOR, OUTLINE_COLOR);
        }
        public static void CreateHealing(Combat.Piece piece, int healing) {
            Create(piece, new Vector2(0, -12), "+" + healing.ToString(), HEALING_COLOR, OUTLINE_COLOR);
        }

        private Label label;

        private void Load(Node node, Vector2 position, string text, Color color, Color? outline) {
            float duration = 0.4f;
            label = GetNode<Label>("Label");
            label.Text = text;
            label.AddColorOverride("font_color", color);
            if (outline.HasValue) {
                label.AddColorOverride("font_outline_modulate", outline.Value);
            }
            node.AddChild(this);
            Position = position;
            Tween tween = new Tween();
            AddChild(tween);
            tween.InterpolateProperty(this, "modulate:a", 1f, 0f, duration, Tween.TransitionType.Cubic, Tween.EaseType.In);
            tween.InterpolateProperty(this, "position:y", position.y, position.y - 16, duration, Tween.TransitionType.Sine, Tween.EaseType.Out);
            tween.Connect("tween_all_completed", this, nameof(on_TweenAllCompleted));
            tween.Start();
        }

        private void on_TweenAllCompleted() {
            QueueFree();
        }
    }
}