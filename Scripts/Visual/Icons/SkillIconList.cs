using System;
using System.Collections.Generic;
using Godot;

namespace Visual.Icons {
    public class SkillIconList : HBoxContainer {
        public void SetSkills(IEnumerable<Skill> skills) {
            this.QueueFreeChildren();
            foreach (Skill s in skills) {
                if (s != null) {
                    AddChild(SkillButton.CreateCondensed(s));
                }
            }
        }

        public void SetCoreSkills(Entity entity) {
            SetSkills(entity.coreSkills);
        }

        public void SetTalents(Entity entity) {
            // TODO
            this.QueueFreeChildren();
        }
    }
}
