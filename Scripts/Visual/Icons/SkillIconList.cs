using System;
using System.Collections.Generic;
using Godot;

namespace Visual.Icons {
    public class SkillIconList : HBoxContainer {

        [Export] public bool lookable = true;
        public void SetSkills(IEnumerable<Skill> skills) {
            this.QueueFreeChildren();
            foreach (Skill s in skills) {
                if (s != null) {
                    SkillButton b = SkillButton.CreateCondensed(s);
                    AddChild(b);
                    if (lookable) {
                        b.Connect("pressed", this, nameof(Open), Global.ArrayFrom(s));
                    } else {
                        b.MouseFilter = MouseFilterEnum.Ignore;
                    }
                }
            }
        }

        public void SetCoreSkills(CharacterEntity entity) {
            SetSkills(entity.GetCoreSkills());
        }

        public void SetTalents(CharacterEntity entity) {
            // TODO
            this.QueueFreeChildren();
        }

        public void Open(Skill skill) {
            MetaPopup.instance.OpenSkill(skill);
        }
    }
}
