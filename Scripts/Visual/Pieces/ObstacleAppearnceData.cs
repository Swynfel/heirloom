using System;
using Godot;

namespace Visual {
    public enum ObstacleAppearanceType {
        BOULDER,
        BUSH,
        ICEBALL,
        SIGNPOST,
    }
    public class ObstacleAppearanceData : AppearanceData {
        [Export] public ObstacleAppearanceType id;
        private static readonly Vector2 OFFSET = 8 * Vector2.Up;
        public override Node2D GenerateOn(CanvasItem node) {
            node.QueueFreeChildren();
            ObstacleAppearance appearance = ObstacleAppearance.New();
            node.AddChild(appearance);
            appearance.SetAppearance(id);
            return appearance;
        }
    }
}