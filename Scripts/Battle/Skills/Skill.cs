using System;
using Combat;
using Godot;

public class Skill : Resource {
    [Export] public string name;
    [Export] public Element element;
    [Export] public SkillAreaCreator area;
    [Export] public SkillEffect effect;

    public static Skill Load(string name) {
        return (Skill) GD.Load("Assets/Skills/" + name);
    }
}
