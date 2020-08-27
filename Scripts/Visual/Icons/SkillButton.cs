using System;
using Godot;

namespace Visual.Icons {
    public class SkillButton : Button {
        private static PackedScene condensedTemplate = (PackedScene) ResourceLoader.Load("res://Nodes/UI/Buttons/SkillButtonCondensed.tscn");
        public static SkillButton CreateCondensed(Skill skill) {
            SkillButton skillButton = (SkillButton) condensedTemplate.Instance();
            skillButton.Configure(skill, "");
            return skillButton;
        }
        public void Configure(Skill skill, string i = null) {
            if (skill == null) {
                Hide();
                return;
            }
            Show();
            GetNode<ElementIcon>("Element").Configure(skill.element, false);
            GetNode<SkillIcon>("SkillIcon").Configure(skill.icon, skill.element);
            if (i != null) {
                GetNode<Label>("Label").Text = i;
            }
        }
    }
}