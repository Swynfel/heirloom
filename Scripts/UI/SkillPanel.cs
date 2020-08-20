using System;
using Godot;

namespace UI {
    public class SkillPanel : Panel {

        private Button[] spells;
        private int TOTAL_SPELLS = 5;
        public override void _Ready() {
            spells = new Button[TOTAL_SPELLS];
            for (int k = 0 ; k < TOTAL_SPELLS ; k++) {
                Button spell = GetNode<Button>("Skill-" + (k + 1));
                Godot.Collections.Array bind = Global.ArrayFrom(k);
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
            spells[id].ReleaseFocus();
            Global.battleUI.SwitchState(BattleUI.BattleState.LAUNCHER);
            Skill skill;
            if (id == 0) {
                skill = Skill.Load("spell_walk.tres");
            } else {
                skill = Skill.Load("spell_teleportation.tres");
            }
            Global.battleUI.launcherPanel.Load(skill);
        }

        private void on_SkillHovered(int id) {
            spells[id].GrabFocus();
            lastFocus = id;
        }

        private void on_SkillUnHovered(int id) {
            spells[id].ReleaseFocus();
        }
    }
}
