using System;
using System.Collections.Generic;
using Godot;

namespace Visual.Icons {
    public class ElementIcon : TextureRect {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/ElementIcon.tscn");
        public static ElementIcon Create(Element element = Element.NONE) {
            ElementIcon icon = (ElementIcon) template.Instance();
            icon.Configure(element);
            return icon;
        }

        private const int SIZE = 8;
        private static Vector2 DIM = new Vector2(SIZE, SIZE);
        public void Configure(Element element, bool colored = true) {
            ((AtlasTexture) Texture).Region = new Rect2(colored ? SIZE : 0, ((int) element) * SIZE, DIM);
        }
    }
}
