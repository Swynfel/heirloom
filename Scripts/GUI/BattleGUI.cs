using System;
using Godot;

namespace GUI {
    public class BattleGUI : Control {
        internal enum BattleState {
            OBSERVE,
            SKILL,
            LAUNCHER,
        }

        [Export] internal BattleState currentState = BattleState.OBSERVE;

        internal SkillPanel skillPanel;
        internal LauncherPanel launcherPanel;
        public override void _Ready() {
            skillPanel = GetNode<SkillPanel>("SkillPanel");
            launcherPanel = GetNode<LauncherPanel>("LauncherPanel");
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
    }
}
