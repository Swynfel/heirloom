using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Combat.SkillEffects {
    class Heal : SkillEffect {
        [Save] [Export] public int heal = 5;

        public override Task Apply(Element element, Piece launcher, SkillArea area) {
            CommonApply(element, launcher, area, false);
            return Task.FromResult<object>(null);
        }
        public override float Heuristic(Element element, Piece launcher, SkillArea area) {
            return CommonApply(element, launcher, area, true);
        }
        private float CommonApply(Element element, Piece launcher, SkillArea area, bool simulate) {
            float heuristic = 0f;
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
                if (simulate) {
                    int h = piece.entity.ModifyHealthSimulation(final_healing);
                    heuristic += (piece.entity.alignment == launcher.entity.alignment) ? h : -h;
                } else {
                    piece.entity.ModifyHealth(final_healing);
                    Visual.Effects.FloatingLabel.CreateHealing(piece, final_healing);
                }
            }
            return heuristic;
        }
    }
}
