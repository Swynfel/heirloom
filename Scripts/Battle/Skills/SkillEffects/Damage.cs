using System;
using System.Collections.Generic;
using Godot;

namespace Combat.SkillEffects {
    class Damage : SkillEffect {
        [Export] public int damage = 3;
        public override void Apply(Element element, Piece launcher, SkillArea area) {
            foreach (Piece piece in area.AllPieces()) {
                float modifier = 1f;
                switch (piece.entity.affinity[element]) {
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
                float final_floating_damage = -modifier * damage;
                int final_damage = (int) final_floating_damage;
                piece.entity.ModifyHealth(final_damage);
                Visual.Effects.FloatingLabel.CreateDamage(piece, final_damage);
            }
        }
    }
}
