using System;
using Godot;

namespace UI {
    public class BattleUI : Control {

        public static BattleUI current = null;
        internal enum BattleState {
            OBSERVE,
            SKILL,
            LAUNCHER,
        }

        [Export] internal BattleState currentState = BattleState.OBSERVE;

        internal SkillPanel skillPanel;
        internal LauncherPanel launcherPanel;
        internal Control freezePanel;

        public override void _Ready() {
            current = this;
            Game.instance.Connect(nameof(Game.busy_switch), this, nameof(on_BusySwitch));
            Global.battle.Connect(nameof(Combat.Battle.next_turn), this, nameof(on_NextTurn));
            skillPanel = GetNode<SkillPanel>("SkillPanel");
            launcherPanel = GetNode<LauncherPanel>("LauncherPanel");
            freezePanel = GetNode<Control>("FreezePanel");
            skillPanel.Disable();
            launcherPanel.Disable();
            BattleState firstState = currentState;
            currentState = BattleState.OBSERVE;
            SwitchState(firstState);
        }

        internal void SwitchState(BattleState state) {
            if (currentState != state) {
                switch (currentState) {
                    case BattleState.SKILL:
                        skillPanel.Disable();
                        break;
                    case BattleState.LAUNCHER:
                        launcherPanel.Disable();
                        break;
                }
                currentState = state;
                switch (currentState) {
                    case BattleState.SKILL:
                        skillPanel.Enable();
                        break;
                    case BattleState.LAUNCHER:
                        launcherPanel.Enable();
                        break;
                }
            }
        }

        public void EndTurn() {
            Global.battle.NextTurn();
        }

        private void on_BusySwitch(bool busy) {
            if (busy) {
                current.freezePanel.Show();
            } else {
                current.freezePanel.Hide();
            }
        }

        private void on_NextTurn(Combat.Piece piece) {
            if (piece.entity.alignment == Combat.Alignment.FRIENDLY) {
                SwitchState(BattleState.SKILL);
            } else {
                SwitchState(BattleState.OBSERVE);
                // HACK
                Global.battle.NextTurn();
            }
        }
    }
}
