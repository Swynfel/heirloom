using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillEffects {
    class Move : SkillEffect {
        [Export] public bool ignoreGround;
        public override void Apply(Element element, Piece launcher, SkillArea area) {
            launcher.MoveOn(area.AllTiles().Last());
        }
    }
}