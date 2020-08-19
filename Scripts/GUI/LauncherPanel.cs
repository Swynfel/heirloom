using System;
using Godot;

namespace GUI {
    public class LauncherPanel : Panel {
        public override void _Ready() {

        }

        public void Load(Skill skill) {

        }
        public override void _Process(float delta) {
            if (Input.IsActionPressed("ui_right")) {

            }

        }

        public bool enabled { get; private set; }
        public void Disable() {
            Hide();
        }

        public void Enable() {
            Show();
        }
    }
}
