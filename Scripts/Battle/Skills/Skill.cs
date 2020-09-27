using System;
using System.Threading.Tasks;
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
    [Save] [Export(PropertyHint.MultilineText)] public readonly string description;
    public string BBDescription => InfoText.BBfy(description);

    public async Task Apply(Piece launcher, SkillArea overideArea = null) {
        await effect.Apply(element, launcher, overideArea ?? area.Done());
        condition.Used();
        return;
    }
    public static Skill Load(string name) {
        return (Skill) GD.Load("Data/Skills/skill_" + name + ".tres");
    }

    public Skill Clone() {
        Skill copy = (Skill) Duplicate();
        copy.condition = (SkillCondition) condition.Duplicate();
        copy.area = (SkillAreaCreator) area.Duplicate();
        return copy;
    }
}
