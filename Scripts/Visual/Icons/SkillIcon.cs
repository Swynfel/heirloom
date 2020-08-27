using System;
using System.Collections.Generic;
using Godot;

namespace Visual.Icons {
    public class SkillIcon : TextureRect {
        public enum SpriteTemplate {
            WALK, SWORD, CROWN, SHIELD,
            SCEPTRE, HEAL_MANY, HEART, DOUBLE_HEART,
            CAST_BALL, YINYANG, BUBBLE, DISK,
            ARROW_RAIN, FANGS, SKULL, SNAKE
        }
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/SkillIcon.tscn");
        public static SkillIcon Create(SpriteTemplate sprite, Element element = Element.NONE) {
            SkillIcon icon = (SkillIcon) template.Instance();
            icon.Configure(sprite, element);
            return icon;
        }

        private const int SIZE = 16;
        private static Vector2 DIM = new Vector2(SIZE, SIZE);
        public void Configure(SpriteTemplate sprite, Element element = Element.NONE) {
            int i = (int) sprite;
            ((AtlasTexture) Texture).Region = new Rect2((i % 4) * SIZE, (i / 4) * SIZE, DIM);
            Modulate = ElementUtils.GetColor(element);
        }
    }
}
