using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Combat.SkillEffects {
    class Move : SkillEffect {
        [Export] public bool ignoreGround = false;
        public override async Task Apply(Element element, Piece launcher, SkillArea area) {
            int totalTiles = area.Count;
            float stepTime = totalTiles switch
            {
                0 => 0.4f,
                1 => 0.4f,
                2 => 0.4f,
                3 => 0.3f,
                4 => 0.25f,
                5 => 0.2f,
                _ => 0.15f,
            };
            foreach (Tile tile in area.AllTiles().Skip(1)) {
                await launcher.WalkTo(tile, stepTime);
                launcher.MoveOn(tile);
                // TODO: activate effects on landing tile
            }
        }

        public override float Heuristic(Element element, Piece launcher, SkillArea area) {
            return 0;
        }
    }
}