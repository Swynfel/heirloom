using System;
using System.Collections.Generic;
using Godot;

namespace Visual {
    public class ObstacleAppearance : Node2D {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Appearances/Obstacle.tscn");
        public static ObstacleAppearance New() {
            return (ObstacleAppearance) template.Instance();
        }
        public void SetAppearance(ObstacleAppearanceType id) {
            GetNode<Sprite>("Body").Frame = (int) id;
        }
    }
}
