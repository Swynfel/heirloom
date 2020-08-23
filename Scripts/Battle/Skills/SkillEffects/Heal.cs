using System;
using System.Collections.Generic;
using Godot;

namespace Combat.SkillEffects {
    class Heal : SkillEffect {
        [Export] public int heal = 5;
        public override void Apply(Element element, Piece launcher, SkillArea area) {
            foreach (Piece piece in area.AllPieces()) {
                if (piece.entity.alignment != launcher.entity.alignment) {
                    continue;
                }
                float modifier = 1f;
                switch (piece.entity.affinity[element]) {
                    case (ElementAffinity.RESISTANT):
                        modifier = 2f;
                        break;
                    default:
                        modifier = 1f;
                        break;
                }
                float final_floating_heal = modifier * heal;
                int final_healing = (int) final_floating_heal;
                piece.entity.ModifyHealth(final_healing);
                Visual.Effects.FloatingLabel.CreateHealing(piece, final_healing);
            }
        }
    }
}
