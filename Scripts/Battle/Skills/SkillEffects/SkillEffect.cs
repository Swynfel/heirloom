using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public abstract class SkillEffect : Resource {
        public abstract Task Apply(Element element, Piece launcher, SkillArea area);
        public abstract float Heuristic(Element element, Piece launcher, SkillArea area);
    }
}