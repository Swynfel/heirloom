using System;
using System.Collections.Generic;
using Godot;

namespace Combat.SkillEffects {
    class Damage : SkillEffect {
        [Export] public int damage = 3;
        [Export] public bool noFriendlyFire = false;
        public override void Apply(Element element, Piece launcher, SkillArea area) {
            foreach (Piece piece in area.AllPieces()) {
                if (noFriendlyFire && piece.entity.alignment == launcher.entity.alignment) {
                    continue;
                }
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
                float final_floating_damage = modifier * damage;
                final_floating_damage += launcher.entity?.heldItem?.bonusDamage ?? 0;
                final_floating_damage -= piece.entity?.heldItem?.armor ?? 0;
                int final_damage = (int) Math.Max(0, Math.Ceiling(final_floating_damage));
                piece.entity.ModifyHealth(-final_damage);
                Visual.Effects.FloatingLabel.CreateDamage(piece, -final_damage);
            }
        }

        public override float Heuristic(Element element, Piece launcher, SkillArea area) {
            float heuristic = 0f;
            foreach (Piece piece in area.AllPieces()) {
                if (noFriendlyFire && piece.entity.alignment == launcher.entity.alignment) {
                    continue;
                }
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
                float final_floating_damage = modifier * damage;
                final_floating_damage += launcher.entity?.heldItem?.bonusDamage ?? 0;
                final_floating_damage -= piece.entity?.heldItem?.armor ?? 0;
                int final_damage = (int) Math.Max(0, Math.Ceiling(final_floating_damage));
                int h = piece.entity.ModifyHealthSimulation(-final_damage);
                heuristic += (piece.entity.alignment == launcher.entity.alignment) ? 2 * h : -h;
            }
            return heuristic;
        }
    }
}
