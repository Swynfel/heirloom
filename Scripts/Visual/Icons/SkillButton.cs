using System;
using Godot;

namespace Visual.Icons {
    public class SkillButton : Button {
        private static PackedScene condensedTemplate = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/SkillButtonCondensed.tscn");
        public static SkillButton CreateCondensed(Skill skill) {
            SkillButton skillButton = (SkillButton) condensedTemplate.Instance();
            skillButton.Set(skill, "");
            return skillButton;
        }
        public void Set(Skill skill, string i = null) {
            if (skill == null) {
                Hide();
                return;
            }
            Show();
            GetNode<ElementIcon>("Element").Set(skill.element, false);
            GetNode<SkillIcon>("SkillIcon").Set(skill.icon, skill.element);
            if (i != null) {
                GetNode<Label>("Label").Text = i;
            }
        }
    }
}