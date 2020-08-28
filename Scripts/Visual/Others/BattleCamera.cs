using System;
using Combat;
using Godot;

namespace Visual {
    public class BattleCamera : Node2D {
        private Tween tween;
        public override void _Ready() {
            GetParent().Connect(nameof(Battle.next_turn), this, nameof(Focus));
            tween = new Tween();
            AddChild(tween);
        }

        private Piece currentFocus;
        public void Focus(Piece piece) {
            currentFocus = piece;
            tween.InterpolateProperty(this, "position", Position, piece.RootPosition, 1f, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            tween.Start();
        }

        public override void _Process(float delta) {
            if (!tween.IsActive() && currentFocus != null) {
                Position = currentFocus.RootPosition;
            }
        }
    }
}
