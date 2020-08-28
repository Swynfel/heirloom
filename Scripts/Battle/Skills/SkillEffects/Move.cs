using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Combat.SkillEffects {
    class Move : SkillEffect {
        [Export] public bool ignoreGround = false;
        public override async Task Apply(Element element, Piece launcher, SkillArea area) {
            foreach (Tile tile in area.AllTiles().Skip(1)) {
                await launcher.WalkTo(tile);
                launcher.MoveOn(tile);
            }
        }

        public override float Heuristic(Element element, Piece launcher, SkillArea area) {
            return 0;
        }
    }
}