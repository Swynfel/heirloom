using System;
using Combat;
using Godot;
using Visual.Icons;

public class Skill : Resource {
    [Export] public string name;
    [Export] public Element element;
    [Export] public SkillIcon.SpriteTemplate icon;
    [Export] public SkillCondition condition;
    [Export] public SkillAreaCreator area;
    [Export] public SkillEffect effect;
    [Export] public string template;
    [Export] public string tags;
    [Export(PropertyHint.MultilineText)] public readonly string description;
    public string BBDescription => InfoText.BBfy(description);

    public static Skill Load(string name) {
        return (Skill) GD.Load("Data/Skills/skill_" + name + ".tres");
    }
}
