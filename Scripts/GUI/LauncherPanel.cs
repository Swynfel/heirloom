using System;
using Combat;
using Godot;

namespace GUI {
    public class LauncherPanel : Panel {

        public Skill skill { get; private set; }
        public Combat.SkillAreaCreator area;

        public Piece launcher;

        private bool connectedToBoard = false;

        public void Load(Skill skill) {
            if (!connectedToBoard) {
                Battle.current.board.Connect(nameof(Board.tile_hovered), this, nameof(on_TileHovered));
                connectedToBoard = true;
            }
            this.skill = skill;
            area = skill.area.Clone();
            launcher = Battle.current.pieces[0];
            area.Start(launcher);
        }

        public void on_TileHovered(Tile tile) {
            if (active) {
                area.Hover(tile);
            }
        }

        public void Launch() {
            SkillArea skillArea = area.Done();
            skill.effect.Apply(skill.element, launcher, skillArea);
            // Some skills don't end turn
            BattleGUI.current.EndTurn();
        }

        public override void _Process(float delta) {
            if (!active) {
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
            if (Input.IsActionJustPressed("click") && Battle.current.board.hovered) {
                area.Click();
            }
            if (Input.IsActionJustPressed("undo")) {
                area.Undo();
            }
            if (Input.IsActionJustPressed("ui_accept") && area.IsValid()) {
                Launch();
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

        public bool active { get { return enabled && !BattleGUI.busy; } }
        public void Disable() {
            Hide();
            enabled = false;
        }

        public void Enable() {
            Show();
            enabled = true;
        }
    }
}
