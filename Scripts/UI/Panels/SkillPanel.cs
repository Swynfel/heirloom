using System;
using Godot;
using Visual.Icons;

namespace UI {
    public class SkillPanel : Panel {

        private SkillButton[] slots;
        private int TOTAL_SPELLS = 5;
        public override void _Ready() {
            slots = new SkillButton[TOTAL_SPELLS];
            for (int k = 0 ; k < TOTAL_SPELLS ; k++) {
                SkillButton skill = GetNode<SkillButton>("Skill-" + (k + 1));
                Godot.Collections.Array bind = Global.ArrayFrom(k);
                skill.Connect("pressed", this, nameof(on_SkillActivated), bind);
                skill.Connect("mouse_entered", this, nameof(on_SkillHovered), bind);
                skill.Connect("mouse_exited", this, nameof(on_SkillUnHovered), bind);
                slots[k] = skill;
            }
        }

        private Skill[] skills;

        private Entity entity;

        public void Load(Entity entity) {
            this.entity = entity;
            Refresh();
        }

        public void Refresh() {
            skills = entity.skills;
            int i = 0;
            foreach (Skill skill in skills) {
                if (skill == null) {
                    slots[i].Hide();
                    i++;
                    continue;
                }
                slots[i].Show();
                slots[i].Disabled = !skill.condition.IsValid();
                slots[i].Configure(skill, (i + 1).ToString());
                i++;
            }
            while (i < 5) {
                slots[i].Hide();
                i++;
            }
        }

        public bool enabled { get; private set; }
        public void Disable() {
            Hide();
        }

        public void Enable() {
            Refresh();
            Show();
        }

        private int lastFocus = -1;

        private void on_SkillActivated(int id) {
            slots[id].ReleaseFocus();
            if (!slots[id].Visible) {
                return;
            }
            Global.battleUI.SwitchState(BattleUI.BattleState.LAUNCHER);
            Global.battleUI.launcherPanel.Load(skills[id]);
        }

        private void on_SkillHovered(int id) {
            slots[id].GrabFocus();
            lastFocus = id;
        }

        private void on_SkillUnHovered(int id) {
            slots[id].ReleaseFocus();
        }
    }
}
