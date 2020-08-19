using System;
using Combat;
using Godot;

namespace GUI {
    public class LauncherPanel : Panel {

        public Skill skill { get; private set; }
        public Combat.SkillAreaCreator area;

        public Piece launcher;

        public void Load(Skill skill) {
            this.skill = skill;
            area = skill.area.Clone();
            launcher = Battle.current.pieces[0];
            area.Start(launcher);
        }
        public override void _Process(float delta) {
            if (BattleGUI.busy) {
                return;
            }
            if (Input.IsActionJustPressed("ui_right")) {
                area.Key(Direction.RIGHT);
            }
            if (Input.IsActionJustPressed("ui_up")) {
                area.Key(Direction.UP);
            }
            if (Input.IsActionJustPressed("ui_left")) {
                area.Key(Direction.LEFT);
            }
            if (Input.IsActionJustPressed("ui_down")) {
                area.Key(Direction.DOWN);
            }
            if (Input.IsActionJustPressed("ui_accept") && area.IsValid()) {
                SkillArea skillArea = area.Done();
                skill.effect.Apply(skill.element, launcher, skillArea);
            }
            if (Input.IsActionJustPressed("ui_exit")) {
                Clear();
            }

        }

        public void Clear() {
            if (area != null) {
                area.Cancel();
                area = null;
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
