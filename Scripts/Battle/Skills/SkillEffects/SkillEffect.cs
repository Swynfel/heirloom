using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public abstract class SkillEffect : Resource {
        public abstract void Apply(Element element, Piece launcher, SkillArea area);
        public abstract float Heuristic(Element element, Piece launcher, SkillArea area);
    }
}