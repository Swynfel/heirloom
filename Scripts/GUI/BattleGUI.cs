using System;
using Godot;

namespace GUI {
    public class BattleGUI : Control {

        public static BattleGUI current = null;
        internal enum BattleState {
            OBSERVE,
            SKILL,
            LAUNCHER,
        }

        [Export] internal BattleState currentState = BattleState.OBSERVE;

        internal SkillPanel skillPanel;
        internal LauncherPanel launcherPanel;
        internal CanvasItem freezePanel;

        public override void _Ready() {
            current = this;
            skillPanel = GetNode<SkillPanel>("SkillPanel");
            launcherPanel = GetNode<LauncherPanel>("LauncherPanel");
            freezePanel = GetNode<CanvasItem>("FreezePanel");
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

        private int busyCount = 0;

        public static bool busy { get { return current.busyCount > 0; } }

        public static void StartBusy() {
            if (!busy) {
                current.freezePanel.Show();
            }
            current.busyCount++;
        }

        public static void EndBusy() {
            current.busyCount--;
            if (!busy) {
                current.freezePanel.Hide();
            }
        }
    }
}
