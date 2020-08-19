using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public abstract class SkillEffect : Resource {
        public Element element;
        public Piece launcher;

        public void Setup(Element element, Piece launcher) {
            this.element = element;
            this.launcher = launcher;
        }
        public abstract void ApplyOn(SkillArea area);

    }
}