using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public class SkillCondition : Resource {
        [Export] public int cooldown = 0;
        [Export] public int combatCharges = -1;
        [Export] public int turnCharges = -1;
        [Export] public bool passTurn = true;

        public int remainingCooldown { get; private set; } = -1;
        public int remainingCombatCharges { get; private set; } = -1;
        public int remainingTurnCharges { get; private set; } = -1;

        public bool IsValid() {
            return (combatCharges < 0 || remainingCombatCharges > 0)
                && (turnCharges < 0 || remainingTurnCharges > 0);
        }

        public void ResetTurn() {
            remainingTurnCharges = turnCharges;
            if (remainingCooldown > 0) {
                remainingCooldown--;
            }
        }

        public void ResetCombat() {
            remainingCooldown = 0;
            remainingCombatCharges = combatCharges;
            ResetTurn();
        }

        public void Used() {
            remainingCooldown = cooldown;
            if (remainingCombatCharges > 0) {
                remainingCombatCharges--;
            }
            if (remainingTurnCharges > 0) {
                remainingTurnCharges--;
            }
        }
    }
}
