using System;
using Godot;

public class SkillButton : Button {
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
