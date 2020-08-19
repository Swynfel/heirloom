using System;
using Godot;

namespace GUI {
    public class SkillPanel : Panel {

        private Button[] spells;
        private int TOTAL_SPELLS = 5;
        public override void _Ready() {
            spells = new Button[TOTAL_SPELLS];
            for (int k = 0 ; k < TOTAL_SPELLS ; k++) {
                Button spell = GetNode<Button>("Skill-" + (k + 1));
                Godot.Collections.Array bind = new Godot.Collections.Array();
                bind.Add(k);
                spell.Connect("pressed", this, nameof(on_SkillActivated), bind);
                spell.Connect("mouse_entered", this, nameof(on_SkillHovered), bind);
                spell.Connect("mouse_exited", this, nameof(on_SkillUnHovered), bind);
                spells[k] = spell;
            }
        }

        public bool enabled { get; private set; }
        public void Disable() {
            Hide();
        }

        public void Enable() {
            Show();
        }

        private int lastFocus = -1;

        private void on_SkillActivated(int id) {
            GD.Print("Activate Skill-" + id);
            spells[id].ReleaseFocus();
        }

        private void on_SkillHovered(int id) {
            GD.Print("Hovered Skill-" + id);
            spells[id].GrabFocus();
            lastFocus = id;
        }

        private void on_SkillUnHovered(int id) {
            GD.Print("Un-hovered Skill-" + id);
            spells[id].ReleaseFocus();
        }
    }
}
