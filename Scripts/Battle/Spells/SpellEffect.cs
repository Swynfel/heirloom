using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    abstract class SpellEffect {
        public virtual void ApplyOn(List<Tile> tiles) {
            foreach (Tile tile in tiles) {
                ApplyOn(tile);
            }
        }
        public virtual void ApplyOn(Tile tile) {
            foreach (Piece piece in tile.pieces) {
                ApplyOn(piece);
            }
        }
        public abstract void ApplyOn(Piece piece);
    }

    namespace SpellEffects {
        class Damage : SpellEffect {
            public Element element;
            public int damage;

            public override void ApplyOn(Piece piece) {
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