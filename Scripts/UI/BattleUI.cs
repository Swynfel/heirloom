using System;
using System.Threading.Tasks;
using Godot;

namespace UI {
    public class BattleUI : CanvasLayer {

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
        }

        public void Start() {
            BattleState firstState = currentState;
            currentState = BattleState.OBSERVE;
            SwitchState(firstState);
            GetNode<TurnPanel>("TurnPanel").Start();
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
                skillPanel.Load(piece.entity);
            } else {
                SwitchState(BattleState.OBSERVE);
                new EnemyAI(piece).Play(); // TODO: Make nice async stuff
            }
        }
    }
}
