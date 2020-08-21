using System;
using System.Collections.Generic;
using Godot;

namespace Visual {
    public class CharacterAppearance : Node2D {

        public Sprite eyes;
        public Sprite head;
        public Sprite body;


        public CharacterAppearanceData data {
            get {
                return _data;
            }
            set {
                value.Paint(this);
                _data = value;
            }
        }
        [Export] private CharacterAppearanceData _data = null;

        public override void _Ready() {
            eyes = GetNode<Sprite>("Eyes");
            head = GetNode<Sprite>("Head");
            body = GetNode<Sprite>("Body");
            if (data == null) {
                data = CharacterAppearanceData.Random();
            }
        }
    }
}