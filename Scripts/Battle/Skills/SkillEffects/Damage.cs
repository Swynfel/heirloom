using System;
using System.Collections.Generic;
using Godot;

namespace Combat.SkillEffects {
    class Damage : SkillEffect {
        [Export] public int damage;
        public override void ApplyOn(SkillArea area) {
            foreach (Piece piece in area.AllPieces()) {
                float modifier = 1f;
                switch (piece.stats.affinity[element]) {
                    case (ElementAffinity.IMMUNE):
                        modifier = 0f;
                        break;
                    case (ElementAffinity.RESISTANT):
                        modifier = 0.5f;
                        break;
                    case (ElementAffinity.WEAK):
                        modifier = 2f;
                        break;
                }
                float final_damage = modifier * damage;
                piece.stats.ModifyHealth((int) (final_damage));
            }
        }
    }
}