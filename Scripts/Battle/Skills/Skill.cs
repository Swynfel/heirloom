using System;
using Combat;
using Godot;

public class Skill : Resource {
    [Export] public string name;
    [Export] public SkillAreaCreator area;
    [Export] public SkillEffect effect;
}
