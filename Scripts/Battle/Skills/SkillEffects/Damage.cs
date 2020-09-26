using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Combat.SkillEffects {
    class Damage : SkillEffect {
        [Save] [Export] public int damage = 3;
        [Save] [Export] public bool noFriendlyFire = false;

        public override Task Apply(Element element, Piece launcher, SkillArea area) {
            CommonApply(element, launcher, area, false);
            return Task.FromResult<object>(null);
        }
        public override float Heuristic(Element element, Piece launcher, SkillArea area) {
            return CommonApply(element, launcher, area, true);
        }
        private float CommonApply(Element element, Piece launcher, SkillArea area, bool simulate = false) {
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
                final_floating_damage += launcher.entity.GetMod(Modifier.BONUS_DAMAGE);
                final_floating_damage -= piece.entity.GetMod(Modifier.ARMOR);
                int final_damage = (int) Math.Max(0, Math.Ceiling(final_floating_damage));
                if (simulate) {
                    int h = piece.entity.ModifyHealthSimulation(-final_damage);
                    heuristic += (piece.entity.alignment == launcher.entity.alignment) ? 2 * h : -h;
                } else {
                    piece.entity.ModifyHealth(-final_damage);
                    Visual.Effects.FloatingLabel.CreateDamage(piece, -final_damage);
                }
            }
            return heuristic;
        }
    }
}
